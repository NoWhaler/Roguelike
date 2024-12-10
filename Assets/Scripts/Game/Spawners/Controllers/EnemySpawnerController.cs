using System;
using System.Collections.Generic;
using System.Linq;
using Core.TurnBasedSystem;
using Game.Hex;
using Game.Spawners.Models;
using Game.Units;
using Game.Units.Controller;
using Game.Units.Enum;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Spawners.Controllers
{
    public class EnemySpawnerController: IInitializable, IDisposable
    {
        private EnemySpawnerModel _enemySpawnerModel;
        
        private HexGridController _hexGridController;
        private GameTurnController _gameTurnController;
        private UnitsController _unitsController;

        private int _nextWaveTurn;
        
        private const int MIN_SPAWN_DISTANCE = 2;
        private const int MAX_SPAWN_DISTANCE = 10;

        [Inject]
        private void Constructor(EnemySpawnerModel enemySpawnerModel, HexGridController hexGridController,
            GameTurnController gameTurnController, UnitsController unitsController)
        {
            _enemySpawnerModel = enemySpawnerModel;
            _hexGridController = hexGridController;
            _gameTurnController = gameTurnController;
            _unitsController = unitsController;
        }

        public void Initialize()
        {
            SetNextWaveTurn();
            _gameTurnController.OnTurnEnded += CheckForWaveSpawn;
        }
        
        public void Dispose()
        {
            _gameTurnController.OnTurnEnded -= CheckForWaveSpawn;
        }
        
        private void SetNextWaveTurn()
        {
            int turnsUntilNextWave = Random.Range(_enemySpawnerModel.MinTurnsBetweenWaves, _enemySpawnerModel.MaxTurnsBetweenWaves + 1);
            _nextWaveTurn = _gameTurnController.GetCurrentTurn() + turnsUntilNextWave;
        }
        
        private void CheckForWaveSpawn()
        {
            if (_gameTurnController.GetCurrentTurn() >= _nextWaveTurn)
            {
                SpawnEnemyWave();
                SetNextWaveTurn();
            }
        }
        
        private void SpawnEnemyWave()
        {
            var allHexes = _hexGridController.GetAllHexes();
            var playerBuildings = allHexes.Values
                .Where(hex => hex.CurrentBuilding != null && hex.CurrentBuilding.BuildingOwner == TeamOwner.Player)
                .ToList();

            if (playerBuildings.Count == 0) return;

            var shuffledBuildings = playerBuildings.OrderBy(x => Random.value).ToList();

            foreach (var targetHex in shuffledBuildings)
            {
                BiomeType targetBiome = targetHex.BiomeType;

                for (int spawnRadius = MIN_SPAWN_DISTANCE; spawnRadius <= MAX_SPAWN_DISTANCE; spawnRadius++)
                {
                    var spawnRing = GetSpawnRing(targetHex, spawnRadius);
                    var validSpawnLocations = spawnRing
                        .Where(hex => 
                            hex.BiomeType == targetBiome && 
                            hex.CurrentUnit == null && 
                            hex.CurrentBuilding == null &&
                            !hex.IsProtectedByAltar &&
                            IsValidSpawnLocation(hex))
                        .ToList();

                    if (validSpawnLocations.Count > 0)
                    {
                        SpawnUnitsAtLocations(validSpawnLocations);
                        Debug.Log($"Spawning enemy wave near player building: {targetHex.CurrentBuilding.BuildingType} at {targetHex.Q},{targetHex.R},{targetHex.S}");
                        return;
                    }
                }
                
                Debug.Log($"Could not spawn near {targetHex.CurrentBuilding.BuildingType}, trying next building...");
            }
            
            Debug.LogWarning("Could not find valid spawn locations near any player buildings");
        }

        private List<HexModel> GetSpawnRing(HexModel center, int radius)
        {
            var innerRadius = _hexGridController.GetHexesInRadius(center, radius);
            var outerRadius = radius > 0 ? 
                _hexGridController.GetHexesInRadius(center, radius - 1) : 
                new List<HexModel>();
            
            return innerRadius.Except(outerRadius).ToList();
        }

        private bool IsValidSpawnLocation(HexModel hex)
        {
            if (!hex.IsVisible) return false;

            var surroundingHexes = _hexGridController.GetHexesInRadius(hex, 2);
            return !surroundingHexes.Any(h => h.IsProtectedByAltar);
        }

        private void SpawnUnitsAtLocations(List<HexModel> validLocations)
        {
            int unitsToSpawn = Random.Range(_enemySpawnerModel.MinUnitsPerWave, _enemySpawnerModel.MaxUnitsPerWave + 1);
            unitsToSpawn = Mathf.Min(unitsToSpawn, validLocations.Count);

            var shuffledLocations = validLocations.OrderBy(x => Random.value).ToList();

            for (int i = 0; i < unitsToSpawn; i++)
            {
                var spawnHex = shuffledLocations[i];
                Unit unitPrefab = _enemySpawnerModel.EnemyUnitPrefabs[Random.Range(0, _enemySpawnerModel.EnemyUnitPrefabs.Length)];
                _unitsController.SpawnEnemyUnit(unitPrefab.UnitType, spawnHex);
            }
        }
    }
}