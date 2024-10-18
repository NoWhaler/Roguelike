using System;
using System.Collections.Generic;
using System.Linq;
using Core.TurnBasedSystem;
using Game.Pathfinding;
using Game.WorldGeneration.Hex;
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

        public void RegisterUnit(ref Unit unit)
        {
            _allUnits.Add(unit);
        }
        
        public void UnregisterUnit(ref Unit unit)
        {
            _allUnits.Remove(unit);
        }
        
        public HashSet<HexModel> GetReachableHexes(Unit unit)
        {
            var reachableHexes = new HashSet<HexModel>();
            var queue = new Queue<(HexModel hex, int remainingMovement)>();
            queue.Enqueue((unit.CurrentHex, unit.CurrentMovementPoints));

            while (queue.Count > 0)
            {
                var (currentHex, remainingMovement) = queue.Dequeue();

                if (reachableHexes.Contains(currentHex))
                    continue;

                reachableHexes.Add(currentHex);

                if (remainingMovement > 0)
                {
                    foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                    {
                        if (neighbor.CurrentUnit == null && neighbor.CurrentBuilding == null)
                        {
                            queue.Enqueue((neighbor, remainingMovement - 1));
                        }
                    }
                }
            }

            reachableHexes.Remove(unit.CurrentHex);
            return reachableHexes;
        }
    }
}