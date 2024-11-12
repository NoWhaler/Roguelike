using System;
using System.Collections.Generic;
using Core.TurnBasedSystem;
using Game.Hex;
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
        
        public HashSet<HexModel> GetReachableHexes(Unit unit)
        {
            var reachableHexes = new HashSet<HexModel>();
            var reachableBuildingHexes = new HashSet<HexModel>();
            var visited = new HashSet<HexModel>();
            var queue = new Queue<(HexModel hex, int remainingMovement)>();
            
            queue.Enqueue((unit.CurrentHex, unit.CurrentMovementPoints));
            visited.Add(unit.CurrentHex);

            while (queue.Count > 0)
            {
                var (currentHex, remainingMovement) = queue.Dequeue();

                if (currentHex.CurrentBuilding != null)
                {
                    reachableBuildingHexes.Add(currentHex);
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
                            
                            if (neighbor.CurrentBuilding != null)
                            {
                                reachableBuildingHexes.Add(neighbor);
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
            reachableHexes.UnionWith(reachableBuildingHexes);
            
            return reachableHexes;
        }
    }
}