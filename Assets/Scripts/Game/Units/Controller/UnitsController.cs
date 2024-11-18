using System;
using System.Collections.Generic;
using System.Linq;
using Core.Builder;
using Core.ObjectPooling.Pools;
using Core.Services;
using Core.TurnBasedSystem;
using Cysharp.Threading.Tasks;
using Game.Buildings.BuildingsType;
using Game.Hex;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.Units.Controller
{
    public class UnitsController: IInitializable, IDisposable
    {
        private readonly Dictionary<UnitType, UnitsPool> _playerUnitPools = new();
        private readonly Dictionary<UnitType, UnitsPool> _enemyUnitPools = new();
        
        private List<Unit> _playerUnits = new List<Unit>();
        private List<Unit> _enemyUnits = new List<Unit>();

        private GameTurnController _gameTurnController;
        private HexGridController _hexGridController;

        private UnitsConfigurationsService _unitsConfigurationsService;

        private DiContainer _diContainer;
        
        private UnitsPool _playerPoolPrefab;
        private UnitsPool _enemyPoolPrefab;
        private Dictionary<UnitType, Unit> _unitPrefabs;
        
        private const float MOVE_DURATION = 0.3f;

        public event Action<Unit> OnUnitHired;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, HexGridController hexGridController,
            UnitsConfigurationsService unitsConfigurationsService, DiContainer diContainer,
            [Inject(Id = "PlayerPool")] UnitsPool playerPoolPrefab,
            [Inject(Id = "EnemyPool")] UnitsPool enemyPoolPrefab,
            [Inject(Id = UnitType.Archer)] Unit archerPrefab,
            [Inject(Id = UnitType.Crossbowman)] Unit crossbowmanPrefab,
            [Inject(Id = UnitType.Swordsman)] Unit infantrymanPrefab,
            [Inject(Id = UnitType.Horseman)] Unit cavalryPrefab)
        {
            _gameTurnController = gameTurnController;
            _hexGridController = hexGridController;
            _unitsConfigurationsService = unitsConfigurationsService;
            _diContainer = diContainer;
            
            _playerPoolPrefab = playerPoolPrefab;
            _enemyPoolPrefab = enemyPoolPrefab;
            
            _unitPrefabs = new Dictionary<UnitType, Unit>
            {
                { UnitType.Archer, archerPrefab },
                { UnitType.Crossbowman, crossbowmanPrefab },
                { UnitType.Swordsman, infantrymanPrefab },
                { UnitType.Horseman, cavalryPrefab }
            };
        }

        public void Initialize()
        {
            _gameTurnController.OnTurnEnded += RefreshUnitsMovement;
            
            InitializeUnitPools();
        }
        
        public void Dispose()
        {
            _gameTurnController.OnTurnEnded -= RefreshUnitsMovement;
        }
        
        private void InitializeUnitPools()
        {
            foreach (UnitType unitType in System.Enum.GetValues(typeof(UnitType)))
            {
                var playerPool = _diContainer.InstantiatePrefabForComponent<UnitsPool>(_playerPoolPrefab);
                playerPool.name = $"PlayerPool_{unitType}";
                playerPool.ObjectPrefab = _unitPrefabs[unitType];
                _playerUnitPools[unitType] = playerPool;
                playerPool.InitPool();

                var enemyPool = _diContainer.InstantiatePrefabForComponent<UnitsPool>(_enemyPoolPrefab);
                enemyPool.name = $"EnemyPool_{unitType}";
                enemyPool.ObjectPrefab = _unitPrefabs[unitType];
                _enemyUnitPools[unitType] = enemyPool;
                enemyPool.InitPool();
            }
        }

        private void RefreshUnitsMovement()
        {
            foreach (var unit in _playerUnits)
            {
                unit.ResetMovementPoints();
            }
            
            foreach (var unit in _enemyUnits)
            {
                unit.ResetMovementPoints();
            }
        }
        
        public void UnregisterUnit(Unit unit)
        {
            if (unit.TeamOwner == TeamOwner.Player)
            {
                _playerUnits.Remove(unit);
            }
            else if (unit.TeamOwner == TeamOwner.Enemy)
            {
                _enemyUnits.Remove(unit);
            }
        }
        
        public List<Unit> GetPlayerUnits()
        {
            return _playerUnits;
        }

        public List<Unit> GetEnemyUnits()
        {
            return _enemyUnits;
        }
        
        
        public Unit SpawnPlayerUnit(UnitType unitType, HexModel targetHex)
        {
            if (!_playerUnitPools.TryGetValue(unitType, out var pool))
            {
                Debug.LogError($"No pool found for player unit type: {unitType}");
                return null;
            }

            var unit = pool.Get();
            if (unit == null)
            {
                Debug.LogWarning($"Pool for player unit type {unitType} is empty");
                return null;
            }

            var config = _unitsConfigurationsService.GetConfig(unitType);
            
            unit = new UnitBuilder(unit, config)
                .WithHealth()
                .WithDamage()
                .WithType()
                .WithAttackRange()
                .WithMovementPoints()
                .WithTeam(TeamOwner.Player)
                .AtPosition(targetHex)
                .Build();
            
            _playerUnits.Add(unit);
            
            OnUnitHired?.Invoke(unit);
            
            return unit;
        }

        public Unit SpawnEnemyUnit(UnitType unitType, HexModel targetHex)
        {
            if (!_enemyUnitPools.TryGetValue(unitType, out var pool))
            {
                Debug.LogError($"No pool found for enemy unit type: {unitType}");
                return null;
            }

            var unit = pool.Get();
            if (unit == null)
            {
                Debug.LogWarning($"Pool for enemy unit type {unitType} is empty");
                return null;
            }
            
            var config = _unitsConfigurationsService.GetConfig(unitType);

            unit = new UnitBuilder(unit, config)
                .WithHealth()
                .WithDamage()
                .WithType()
                .WithAttackRange()
                .WithMovementPoints()
                .WithTeam(TeamOwner.Enemy)
                .AtPosition(targetHex)
                .Build();
            
            _enemyUnits.Add(unit);
            return unit;
        }
        
        public void ReturnUnitToPool(Unit unit)
        {
            var pools = unit.TeamOwner == TeamOwner.Player ? _playerUnitPools : _enemyUnitPools;
            
            if (pools.TryGetValue(unit.UnitType, out var pool))
            {
                if (unit.TeamOwner == TeamOwner.Player)
                {
                    _playerUnits.Remove(unit);
                }
                else
                {
                    _enemyUnits.Remove(unit);
                }

                unit.CurrentHex.CurrentUnit = null;
                pool.ReturnToPool(unit);
            }
            else
            {
                Debug.LogError($"No pool found for unit type: {unit.UnitType}");
            }
        }
        
        public HashSet<HexModel> GetAvailableHexes(Unit unit)
        {
            var reachableHexes = new HashSet<HexModel>();
            var reachableNotEmptyHexes = new HashSet<HexModel>();
            var visited = new HashSet<HexModel>();
            var queue = new Queue<(HexModel hex, int remainingMovement)>();
            
            queue.Enqueue((unit.CurrentHex, unit.CurrentMovementPoints));
            visited.Add(unit.CurrentHex);

            while (queue.Count > 0)
            {
                var (currentHex, remainingMovement) = queue.Dequeue();

                if (currentHex != unit.CurrentHex && (
                    (currentHex.CurrentBuilding != null) ||
                    (currentHex.CurrentUnit != null && currentHex.CurrentUnit.TeamOwner != unit.TeamOwner)))
                {
                    reachableNotEmptyHexes.Add(currentHex);
                    continue;
                }
                if (currentHex.CurrentUnit == null || currentHex == unit.CurrentHex)
                {
                    reachableHexes.Add(currentHex);
                }

                if (remainingMovement > 0)
                {
                    foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                    {
                        if (!visited.Contains(neighbor) && neighbor.IsVisible)
                        {
                            bool canTraverse = neighbor.CurrentUnit == null || neighbor == unit.CurrentHex;
                            
                            if (neighbor.CurrentBuilding != null || 
                                (neighbor.CurrentUnit != null && neighbor.CurrentUnit.TeamOwner != unit.TeamOwner))
                            {
                                reachableNotEmptyHexes.Add(neighbor);
                                visited.Add(neighbor);
                                continue;
                            }

                            if (canTraverse)
                            {
                                queue.Enqueue((neighbor, remainingMovement - 1));
                                visited.Add(neighbor);
                            }
                        }
                    }
                }
            }

            reachableHexes.Remove(unit.CurrentHex);
            reachableHexes.UnionWith(reachableNotEmptyHexes);
            
            return reachableHexes;
        }
        
        public async UniTask MoveUnitWithAnimation(Unit unit, HexModel targetHex, int moveCost)
        {
            if (unit == null || targetHex == null) return;
    
            var previousHex = unit.CurrentHex;
            previousHex.CurrentUnit = null;
    
            Vector3 startPosition = unit.transform.position;
            Vector3 endPosition = new Vector3(targetHex.HexPosition.x, targetHex.HexPosition.y + 5f, targetHex.HexPosition.z);
            
            float elapsedTime = 0f;
            while (elapsedTime < MOVE_DURATION)
            {
                unit.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / MOVE_DURATION);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }
    
            unit.transform.position = endPosition;
    
            unit.Move(targetHex, moveCost);
            targetHex.CurrentUnit = unit;
    
            var targetHexCurrentBuilding = targetHex.CurrentBuilding;
            if (targetHexCurrentBuilding != null)
            {
                targetHex.CurrentUnit = null;
                targetHexCurrentBuilding.IncreaseUnitCount(unit.UnitType);
                UnregisterUnit(unit);
                unit.DisableUnit();
            }
        }
    
        public async UniTask MoveUnitAlongPath(Unit unit, List<HexModel> path)
        {
            if (unit == null || path == null || path.Count <= 1) return;
    
            foreach (var hex in path.Skip(1))
            {
                await MoveUnitWithAnimation(unit, hex, 1);
                if (unit.CurrentMovementPoints <= 0 || hex.CurrentBuilding != null)
                    break;
            }
        }
        
        public HashSet<HexModel> GetAttackableHexes(Unit unit)
        {
            var attackableHexes = new HashSet<HexModel>();
            var movementHexes = GetAvailableHexes(unit);
            
            foreach (var movementHex in movementHexes)
            {
                var hexesInRange = GetHexesInAttackRange(movementHex, unit.AttackRange);
                foreach (var hex in hexesInRange)
                {
                    if (hex.IsVisible && (hex.CurrentUnit == null || 
                        (hex.CurrentUnit != null && hex.CurrentUnit.TeamOwner != unit.TeamOwner)))
                    {
                        attackableHexes.Add(hex);
                    }
                }
            }
            
            var currentPosAttackHexes = GetHexesInAttackRange(unit.CurrentHex, unit.AttackRange);
            foreach (var hex in currentPosAttackHexes)
            {
                if (hex.IsVisible && (hex.CurrentUnit == null || 
                    (hex.CurrentUnit != null && hex.CurrentUnit.TeamOwner != unit.TeamOwner)))
                {
                    attackableHexes.Add(hex);
                }
            }

            return attackableHexes;
        }

        private HashSet<HexModel> GetHexesInAttackRange(HexModel centerHex, int range)
        {
            var hexesInRange = new HashSet<HexModel>();
            var visited = new HashSet<HexModel>();
            var queue = new Queue<(HexModel hex, int distance)>();
            
            queue.Enqueue((centerHex, 0));
            visited.Add(centerHex);

            while (queue.Count > 0)
            {
                var (currentHex, distance) = queue.Dequeue();
                
                if (distance > 0)
                {
                    hexesInRange.Add(currentHex);
                }

                if (distance < range)
                {
                    foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            queue.Enqueue((neighbor, distance + 1));
                            visited.Add(neighbor);
                        }
                    }
                }
            }

            return hexesInRange;
        }
        
        public void ProcessCombat(Unit attackingUnit, Unit defendingUnit)
        {
            Debug.Log($"Attacker {attackingUnit.UnitType} deal damage to {defendingUnit.UnitType}");
            
            float damage = attackingUnit.Attack();
            defendingUnit.TakeDamage(damage);
            
            attackingUnit.SetMovementPointsToZero();
            
            if (defendingUnit.CurrentHealth <= 0)
            {
                UnregisterUnit(defendingUnit);
            }
        }
        
        public void ProcessBuildingAttack(Unit attackingUnit, Building defendingBuilding)
        {
            Debug.Log($"Attacker {attackingUnit.UnitType} deal damage to {defendingBuilding.BuildingType}");
            
            float damage = attackingUnit.Attack();
            defendingBuilding.TakeDamage(damage);
            
            attackingUnit.SetMovementPointsToZero();
        }
    }
}