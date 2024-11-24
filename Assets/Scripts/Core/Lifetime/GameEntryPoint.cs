using System.Collections.Generic;
using System.Linq;
using Game.Buildings.Controller;
using Game.Buildings.Enum;
using Game.Hex;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Core.Lifetime
{
    public class GameEntryPoint: IInitializable
    {
        private Random _random;

        private HexGridController _hexGridController;
        private BuildingsController _buildingsController;
        
        [Inject]
        private void Constructor(HexGridController hexGridController, BuildingsController buildingsController)
        {
            _hexGridController = hexGridController;
            _buildingsController = buildingsController;
            _random = new Random();
        }
        
        public void Initialize()
        {
            InitializeStartingPosition();
        }
        
        private void InitializeStartingPosition()
        {
            var hexGrid = _hexGridController.GetAllHexes();
            var allGrasslandRegions = FindConnectedGrasslandRegions(hexGrid);
            
            if (allGrasslandRegions.Count == 0)
            {
                // Debug.LogError("No grassland regions found");
                return;
            }

            foreach (var hex in hexGrid.Values)
            {
                hex.SetFog(false);
            }
            
            var selectedRegion = SelectRandomRegion(allGrasslandRegions);
            var startingHex = SelectStartingHex(selectedRegion);

            foreach (var hex in selectedRegion)
            {
                hex.SetFog(false);
            }

            _buildingsController.SpawnBuilding(BuildingType.MainBuilding, startingHex);
        }

        private List<List<HexModel>> FindConnectedGrasslandRegions(Dictionary<(int, int, int), HexModel> hexGrid)
        {
            var regions = new List<List<HexModel>>();
            var visitedHexes = new HashSet<HexModel>();

            foreach (var hex in hexGrid.Values)
            {
                if (hex.BiomeType == BiomeType.Grassland && !visitedHexes.Contains(hex))
                {
                    var region = new List<HexModel>();
                    FloodFillGrassland(hex, region, visitedHexes);
                    regions.Add(region);
                }
            }

            return regions;
        }

        private void FloodFillGrassland(HexModel startHex, List<HexModel> region, HashSet<HexModel> visitedHexes)
        {
            var hexesToCheck = new Queue<HexModel>();
            hexesToCheck.Enqueue(startHex);
            visitedHexes.Add(startHex);
            region.Add(startHex);

            while (hexesToCheck.Count > 0)
            {
                var currentHex = hexesToCheck.Dequeue();
                var neighbors = _hexGridController.GetNeighbors(currentHex);

                foreach (var neighbor in neighbors)
                {
                    if (!visitedHexes.Contains(neighbor) && neighbor.BiomeType == BiomeType.Grassland)
                    {
                        visitedHexes.Add(neighbor);
                        region.Add(neighbor);
                        hexesToCheck.Enqueue(neighbor);
                    }
                }
            }
        }

        private List<HexModel> SelectRandomRegion(List<List<HexModel>> regions)
        {
            int randomIndex = _random.Next(regions.Count);
            return regions[randomIndex];
        }

        private HexModel SelectStartingHex(List<HexModel> region)
        {
            var center = CalculateRegionCenter(region);
            return region.OrderBy(hex => Vector3.Distance(hex.HexPosition, center)).First();
        }

        private Vector3 CalculateRegionCenter(List<HexModel> region)
        {
            return new Vector3(
                region.Average(h => h.HexPosition.x),
                0,
                region.Average(h => h.HexPosition.z)
            );
        }
    }
}