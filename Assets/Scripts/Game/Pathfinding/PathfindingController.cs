using System;
using System.Collections.Generic;
using System.Linq;
using Game.WorldGeneration.Hex;
using UnityEngine;
using Zenject;

namespace Game.Pathfinding
{
    public class PathfindingController
    {
        private HexGridController _hexGridController;

        [Inject]
        private void Constructor(HexGridController hexGridController)
        {
            _hexGridController = hexGridController;
        }

        public List<HexModel> FindPath(HexModel start, HexModel goal)
        {
            var openSet = new List<HexModel> { start };
            var cameFrom = new Dictionary<HexModel, HexModel>();
            var gScore = new Dictionary<HexModel, float> { { start, 0 } };
            var fScore = new Dictionary<HexModel, float> { { start, HexDistance(start, goal) } };

            while (openSet.Count > 0)
            {
                var current = openSet
                .OrderBy(x => fScore.GetValueOrDefault(x, float.MaxValue))
                .ThenBy(x => UnityEngine.Random.value)
                .First();

                if (current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                foreach (var neighbor in _hexGridController.GetNeighbors(current))
                {
                    if (neighbor.CurrentUnit != null || neighbor.CurrentBuilding != null || !neighbor.IsVisible)
                        continue;

                    var tentativeGScore = gScore[current] + 1;

                    if (tentativeGScore < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + HexDistance(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }
        

        private float HexDistance(HexModel a, HexModel b)
        {
            var ac = CubeCoordinates(a);
            var bc = CubeCoordinates(b);
            
            float baseDistance = (Math.Abs(ac.x - bc.x) + Math.Abs(ac.y - bc.y) + Math.Abs(ac.z - bc.z)) / 2f;
            
            float straightPenalty = (Math.Abs(ac.x - bc.x) == 0 || Math.Abs(ac.z - bc.z) == 0) ? 0.3f : 0f;
            
            return baseDistance + straightPenalty;
        }

        private Vector3 CubeCoordinates(HexModel hex)
        {
            var x = hex.Q;
            var z = hex.R;
            var y = -x - z;
            return new Vector3(x, y, z);
        }

        private List<HexModel> ReconstructPath(Dictionary<HexModel, HexModel> cameFrom, HexModel current)
        {
            var path = new List<HexModel> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}