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
        private HexGridController _hexGridController;
        private BuildingsController _buildingsController;
        
        [Inject]
        private void Constructor(HexGridController hexGridController, BuildingsController buildingsController)
        {
            _hexGridController = hexGridController;
            _buildingsController = buildingsController;
        }
        
        public void Initialize()
        {
            InitializeStartingPosition();
        }
        
        private void InitializeStartingPosition()
        {
            var hexGrid = _hexGridController.GetAllHexes();
            var allGrasslandRegions = FindConnectedGrasslandRegions(hexGrid);
            
            var selectedRegion = SelectBestStartingRegion(allGrasslandRegions);
            var startingHex = SelectStartingHex(selectedRegion);

            RevealRegionFog(selectedRegion);
            _buildingsController.SpawnBuilding(BuildingType.MainBuilding, startingHex);
        }
        
        private List<List<HexModel>> FindConnectedGrasslandRegions(Dictionary<(int, int, int), HexModel> hexGrid)
        {
            var regions = new List<List<HexModel>>();
            var visitedHexes = new HashSet<HexModel>();

            var grasslandGroups = hexGrid.Values
                .Where(hex => hex.BiomeType == BiomeType.Grassland)
                .GroupBy(hex => (hex.BiomeType, hex.BiomeIndex));

            foreach (var group in grasslandGroups)
            {
                foreach (var hex in group)
                {
                    if (!visitedHexes.Contains(hex))
                    {
                        var region = new List<HexModel>();
                        FloodFillGrasslandInstance(hex, region, visitedHexes);
                        if (region.Count > 0)
                        {
                            regions.Add(region);
                        }
                    }
                }
            }

            return regions;
        }

        private void FloodFillGrasslandInstance(HexModel startHex, List<HexModel> region, HashSet<HexModel> visitedHexes)
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
                    if (!visitedHexes.Contains(neighbor) && 
                        neighbor.BiomeType == startHex.BiomeType && 
                        neighbor.BiomeIndex == startHex.BiomeIndex)
                    {
                        visitedHexes.Add(neighbor);
                        region.Add(neighbor);
                        hexesToCheck.Enqueue(neighbor);
                    }
                }
            }
        }

        private List<HexModel> SelectBestStartingRegion(List<List<HexModel>> regions)
        {
            return regions
                .OrderByDescending(r => r.Count)
                .ThenBy(r => Vector3.Distance(CalculateRegionCenter(r), Vector3.zero))
                .First();
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

        private void RevealRegionFog(List<HexModel> region)
        {
            foreach (var hex in region)
            {
                hex.SetFog(false);
            }
        }
    }
}