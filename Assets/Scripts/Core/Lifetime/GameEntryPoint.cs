using System.Collections.Generic;
using System.Linq;
using Game.Buildings.Controller;
using Game.Buildings.Enum;
using Game.Hex;
using Game.Pathfinding;
using Game.Units.Enum;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;
using Zenject;

namespace Core.Lifetime
{
    public class GameEntryPoint: IInitializable
    {
        private HexGridController _hexGridController;
        private BuildingsController _buildingsController;
        private HexModel _playerStartingHex;
        private const int MIN_BIOMES_DISTANCE = 2;

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
            _playerStartingHex = SelectStartingHex(selectedRegion);

            RevealRegionFog(selectedRegion);
            _buildingsController.SpawnBuilding(BuildingType.MainBuilding, _playerStartingHex);
            
            PlaceEnemyBase(hexGrid);
        }
        
        private void PlaceEnemyBase(Dictionary<(int, int, int), HexModel> hexGrid)
        {
            var validRegions = FindPotentialEnemyBaseRegions(hexGrid);
            if (validRegions.Count == 0) return;

            var selectedRegion = SelectEnemyBaseRegion(validRegions);
            var enemyStartingHex = SelectStartingHex(selectedRegion);
            _buildingsController.SpawnBuilding(BuildingType.MainBuilding, enemyStartingHex, TeamOwner.Enemy);
        }

        private List<List<HexModel>> FindPotentialEnemyBaseRegions(Dictionary<(int, int, int), HexModel> hexGrid)
        {
            var regions = new List<List<HexModel>>();
            var visitedHexes = new HashSet<HexModel>();
            var safeBiomes = GetValidEnemyBaseBiomes(hexGrid);

            foreach (var biomeGroup in safeBiomes)
            {
                foreach (var hex in biomeGroup)
                {
                    if (!visitedHexes.Contains(hex) && IsSafeDistanceFromPlayer(hex) && hex != _playerStartingHex)
                    {
                        var region = new List<HexModel>();
                        FloodFillBiomeInstance(hex, region, visitedHexes);
                        
                        if (region.Count >= 15 && !region.Contains(_playerStartingHex))
                        {
                            regions.Add(region);
                        }
                    }
                }
            }

            return regions;
        }

        private IEnumerable<IGrouping<(BiomeType, int), HexModel>> GetValidEnemyBaseBiomes(Dictionary<(int, int, int), HexModel> hexGrid)
        {
            return hexGrid.Values
                .Where(hex => hex.BiomeType != BiomeType.Mountain && hex.BiomeType != BiomeType.Swamp)
                .GroupBy(hex => (hex.BiomeType, hex.BiomeIndex));
        }

        private bool IsSafeDistanceFromPlayer(HexModel hex)
        {
            if (_playerStartingHex == null || hex == _playerStartingHex) return false;

            int biomeTransitions = 0;
            var visitedHexes = new HashSet<HexModel>();
            var hexesToCheck = new Queue<HexModel>();
            
            hexesToCheck.Enqueue(_playerStartingHex);
            visitedHexes.Add(_playerStartingHex);
            
            BiomeType currentBiomeType = _playerStartingHex.BiomeType;
            int currentBiomeIndex = _playerStartingHex.BiomeIndex;

            while (hexesToCheck.Count > 0)
            {
                var currentHex = hexesToCheck.Dequeue();
                
                if (currentHex == hex) return biomeTransitions >= MIN_BIOMES_DISTANCE;

                foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                {
                    if (!visitedHexes.Contains(neighbor))
                    {
                        visitedHexes.Add(neighbor);
                        hexesToCheck.Enqueue(neighbor);
                        
                        if (neighbor.BiomeType != currentBiomeType || neighbor.BiomeIndex != currentBiomeIndex)
                        {
                            biomeTransitions++;
                            currentBiomeType = neighbor.BiomeType;
                            currentBiomeIndex = neighbor.BiomeIndex;
                        }
                    }
                }
            }

            return false;
        }

        private List<HexModel> SelectEnemyBaseRegion(List<List<HexModel>> validRegions)
        {
            var maxBiomeTransitions = 0;
            List<HexModel> selectedRegion = null;

            foreach (var region in validRegions)
            {
                var centerHex = SelectStartingHex(region);
                var visitedHexes = new HashSet<HexModel>();
                var hexesToCheck = new Queue<HexModel>();
                
                hexesToCheck.Enqueue(_playerStartingHex);
                visitedHexes.Add(_playerStartingHex);
                
                var biomeTransitions = 0;
                var currentBiomeType = _playerStartingHex.BiomeType;
                var currentBiomeIndex = _playerStartingHex.BiomeIndex;

                while (hexesToCheck.Count > 0)
                {
                    var currentHex = hexesToCheck.Dequeue();
                    
                    if (currentHex == centerHex)
                    {
                        if (biomeTransitions > maxBiomeTransitions)
                        {
                            maxBiomeTransitions = biomeTransitions;
                            selectedRegion = region;
                        }
                        break;
                    }

                    foreach (var neighbor in _hexGridController.GetNeighbors(currentHex))
                    {
                        if (!visitedHexes.Contains(neighbor))
                        {
                            visitedHexes.Add(neighbor);
                            hexesToCheck.Enqueue(neighbor);
                            
                            if (neighbor.BiomeType != currentBiomeType || neighbor.BiomeIndex != currentBiomeIndex)
                            {
                                biomeTransitions++;
                                currentBiomeType = neighbor.BiomeType;
                                currentBiomeIndex = neighbor.BiomeIndex;
                            }
                        }
                    }
                }
            }

            return selectedRegion ?? validRegions.First();
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

        private void FloodFillBiomeInstance(HexModel startHex, List<HexModel> region, HashSet<HexModel> visitedHexes)
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