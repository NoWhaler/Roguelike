using System;
using System.Collections.Generic;
using System.Linq;
using Core.TurnBasedSystem;
using Cysharp.Threading.Tasks;
using Game.Hex;
using Game.Pathfinding;
using Game.Units;
using Game.Units.Controller;
using Game.Units.Enum;
using UnityEngine;
using Zenject;

namespace Game.AI
{
    public class EnemyAIController: IInitializable, IDisposable
    {
        private GameTurnController _gameTurnController;
        private UnitsController _unitsController;
        private PathfindingController _pathfindingController;
        
        private float _turnDelay = 1f;
        private float _actionDelay = 0.5f;
        
        [Inject]
        private void Constructor(GameTurnController gameTurnController, UnitsController unitsController,
            PathfindingController pathfindingController)
        {
            _gameTurnController = gameTurnController;
            _unitsController = unitsController;
            _pathfindingController = pathfindingController;
        }
        
        public void Initialize()
        {
            _gameTurnController.OnEnemyTurnStarted += StartTurn;
        }

        public void Dispose()
        {
            _gameTurnController.OnEnemyTurnStarted -= StartTurn;
        }

        private async void StartTurn()
        {
            await ProcessEnemyActions();
            
        }

        private async UniTask ProcessEnemyActions()
        {
            var enemyUnits = _unitsController.GetEnemyUnits();
            var playerUnits = _unitsController.GetPlayerUnits();
            
            foreach (var unit in enemyUnits)
            {
                var targets = FindPotentialTargets(unit);
                if (targets.Count == 0) continue;

                var target = SelectBestTarget(unit, targets);
                await HandleUnitAction(unit, target);
                await UniTask.Delay((int)(_actionDelay * 1000));
            }
            
            Debug.Log("Enemy turn ended");
            _gameTurnController.EndTurn();
        }
        
        private List<HexModel> FindPotentialTargets(Unit unit)
        {
            var attackableHexes = _unitsController.GetAttackableHexes(unit);
            return attackableHexes
                .Where(hex => (hex.CurrentUnit != null && hex.CurrentUnit.TeamOwner == TeamOwner.Player) || hex.CurrentBuilding != null)
                .ToList();
        }

        private HexModel SelectBestTarget(Unit unit, List<HexModel> targets)
        {
            var targetUnits = targets.Where(t => t.CurrentUnit != null).ToList();
            var targetBuildings = targets.Where(t => t.CurrentBuilding != null).ToList();

            if (targetUnits.Any())
            {
                return targetUnits
                    .OrderByDescending(target => {
                        var priority = CalculateTargetPriority(target);
                        var distance = _pathfindingController.CalculatePathDistance(unit.CurrentHex, target);
                        return priority - (distance * 0.1f);
                    })
                    .First();
            }

            return targetBuildings
                .OrderByDescending(target => {
                    var priority = CalculateTargetPriority(target);
                    var distance = _pathfindingController.CalculatePathDistance(unit.CurrentHex, target);
                    return priority - (distance * 0.1f);
                })
                .First();
        }

        private float CalculateTargetPriority(HexModel hex)
        {
            if (hex.CurrentUnit != null)
            {
                return 100f + (100f * (1f - hex.CurrentUnit.CurrentHealth / hex.CurrentUnit.MaxHealth));
            }
            
            if (hex.CurrentBuilding != null)
            {
                return 10f + (10f * (1f - hex.CurrentBuilding.CurrentHealth / hex.CurrentBuilding.MaxHealth));
            }

            return 0f;
        }

        private async UniTask HandleUnitAction(Unit unit, HexModel targetHex)
        {
            var availableHexes = _unitsController.GetAvailableHexes(unit);
            var attackRange = unit.AttackRange;

            if (IsInRange(unit.CurrentHex, targetHex, attackRange) && !unit.HasAttackedThisTurn)
            {
                PerformAttack(unit, targetHex);
                return;
            }

            var bestMovementHex = availableHexes
                .Where(hex => hex.CurrentUnit == null && 
                             IsInRange(hex, targetHex, attackRange) &&
                             _pathfindingController.CalculatePathDistance(hex, targetHex) == attackRange)
                .OrderBy(hex => _pathfindingController.CalculatePathDistance(unit.CurrentHex, hex))
                .FirstOrDefault();

            if (bestMovementHex == null)
            {
                bestMovementHex = FindBestMovementHex(unit, targetHex, availableHexes);
            }

            if (bestMovementHex != null && bestMovementHex.CurrentUnit == null)
            {
                var path = _pathfindingController.FindPath(unit.CurrentHex, bestMovementHex);
                if (path != null && path.Count > 1)
                {
                    await _unitsController.MoveUnitAlongPath(unit, path);
                    
                    if (IsInRange(bestMovementHex, targetHex, attackRange) && !unit.HasAttackedThisTurn)
                    {
                        PerformAttack(unit, targetHex);
                    }
                }
            }
        }
        
        private void PerformAttack(Unit unit, HexModel targetHex)
        {
            var distance = _pathfindingController.CalculatePathDistance(unit.CurrentHex, targetHex);
            if (distance > unit.AttackRange) return;

            if (targetHex.CurrentUnit != null)
            {
                _unitsController.ProcessCombat(unit, targetHex.CurrentUnit);
            }
            else if (targetHex.CurrentBuilding != null)
            {
                _unitsController.ProcessBuildingAttack(unit, targetHex.CurrentBuilding);
            }
        }

        private bool IsInRange(HexModel from, HexModel to, int range)
        {
            var distance = _pathfindingController.CalculatePathDistance(from, to);
            return distance <= range;
        }

        private HexModel FindBestMovementHex(Unit unit, HexModel targetHex, HashSet<HexModel> availableHexes)
        {
            return availableHexes
                .OrderBy(hex => _pathfindingController.CalculatePathDistance(hex, targetHex))
                .FirstOrDefault();
        }
    }
}