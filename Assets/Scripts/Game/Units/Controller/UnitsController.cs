using System;
using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.Hex;
using Game.Units.Enum;
using Zenject;

namespace Game.Units.Controller
{
    public class UnitsController: IInitializable, IDisposable
    {
        private List<Unit> _allUnits = new List<Unit>();

        private GameTurnController _gameTurnController;

        private HexGridController _hexGridController;

        [Inject]
        private void Constructor(GameTurnController gameTurnController, HexGridController hexGridController)
        {
            _gameTurnController = gameTurnController;
            _hexGridController = hexGridController;
        }

        public void Initialize()
        {
            _gameTurnController.OnTurnEnded += RefreshUnitsMovement;
        }
        
        public void Dispose()
        {
            _gameTurnController.OnTurnEnded -= RefreshUnitsMovement;
        }

        private void RefreshUnitsMovement()
        {
            foreach (var unit in _allUnits)
            {
                unit.ResetMovementPoints();
            }
        }

        public void RegisterUnit(Unit unit)
        {
            _allUnits.Add(unit);
        }
        
        public void UnregisterUnit(Unit unit)
        {
            _allUnits.Remove(unit);
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