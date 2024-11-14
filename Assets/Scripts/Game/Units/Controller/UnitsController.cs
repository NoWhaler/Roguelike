using System;
using System.Collections.Generic;
using Core.ObjectPooling.Pools;
using Core.TurnBasedSystem;
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

        private DiContainer _diContainer;
        
        private UnitsPool _playerPoolPrefab;
        private UnitsPool _enemyPoolPrefab;
        private Dictionary<UnitType, Unit> _unitPrefabs;

        public event Action<Unit> OnUnitHired;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, HexGridController hexGridController,
            DiContainer diContainer,
            [Inject(Id = "PlayerPool")] UnitsPool playerPoolPrefab,
            [Inject(Id = "EnemyPool")] UnitsPool enemyPoolPrefab,
            [Inject(Id = UnitType.Archer)] Unit archerPrefab,
            [Inject(Id = UnitType.Crossbowman)] Unit crossbowmanPrefab,
            [Inject(Id = UnitType.Swordsman)] Unit infantrymanPrefab,
            [Inject(Id = UnitType.Horseman)] Unit cavalryPrefab)
        {
            _gameTurnController = gameTurnController;
            _hexGridController = hexGridController;
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
            if (unit.UnitTeamType == UnitTeamType.Player)
            {
                _playerUnits.Remove(unit);
            }
            else if (unit.UnitTeamType == UnitTeamType.Enemy)
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

            SetupUnit(unit, targetHex, UnitTeamType.Player);
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

            SetupUnit(unit, targetHex, UnitTeamType.Enemy);
            _enemyUnits.Add(unit);
            return unit;
        }
        
        public void ReturnUnitToPool(Unit unit)
        {
            var pools = unit.UnitTeamType == UnitTeamType.Player ? _playerUnitPools : _enemyUnitPools;
            
            if (pools.TryGetValue(unit.UnitType, out var pool))
            {
                if (unit.UnitTeamType == UnitTeamType.Player)
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

        private void SetupUnit(Unit unit, HexModel targetHex, UnitTeamType teamType)
        {
            unit.gameObject.SetActive(true);
            unit.transform.position = new Vector3(targetHex.HexPosition.x, targetHex.HexPosition.y + 5f, 
                targetHex.HexPosition.z);
            unit.UnitTeamType = teamType;
            unit.CurrentHex = targetHex;
            targetHex.CurrentUnit = unit;
            unit.Initialize();
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

                if (currentHex.CurrentBuilding != null)
                {
                    reachableNotEmptyHexes.Add(currentHex);
                }
                else
                {
                    reachableHexes.Add(currentHex);
                }

                if (remainingMovement > 0)
                {
                    foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                    {
                        if (!visited.Contains(neighbor) && neighbor.IsVisible)
                        {
                            bool canTraverse = neighbor.CurrentUnit == null;

                            if (neighbor.CurrentBuilding != null || (neighbor.CurrentUnit != null &&
                                                                     neighbor.CurrentUnit.UnitTeamType ==
                                                                     UnitTeamType.Enemy)) 
                            {
                                reachableNotEmptyHexes.Add(neighbor);
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
                        (hex.CurrentUnit != null && hex.CurrentUnit.UnitTeamType != unit.UnitTeamType)))
                    {
                        attackableHexes.Add(hex);
                    }
                }
            }
            
            var currentPosAttackHexes = GetHexesInAttackRange(unit.CurrentHex, unit.AttackRange);
            foreach (var hex in currentPosAttackHexes)
            {
                if (hex.IsVisible && (hex.CurrentUnit == null || 
                    (hex.CurrentUnit != null && hex.CurrentUnit.UnitTeamType != unit.UnitTeamType)))
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
            float damage = attackingUnit.Attack();
            defendingUnit.TakeDamage(damage);
            
            attackingUnit.SetMovementPointsToZero();
            
            if (defendingUnit.CurrentHealth <= 0)
            {
                UnregisterUnit(defendingUnit);
            }
        }
    }
}