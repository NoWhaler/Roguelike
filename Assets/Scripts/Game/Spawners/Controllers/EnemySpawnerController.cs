using System;
using System.Collections.Generic;
using System.Linq;
using Core.TurnBasedSystem;
using Game.Hex;
using Game.Spawners.Models;
using Game.Units;
using Game.Units.Controller;
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
                .Where(hex => hex.CurrentBuilding != null )
                .ToList();

            if (playerBuildings.Count == 0) return;

            HexModel buildingHex = playerBuildings[Random.Range(0, playerBuildings.Count)];
            BiomeType targetBiome = buildingHex.BiomeType;

            var spawnArea = _hexGridController.GetHexesInRadius(buildingHex, 2)
                .Where(hex => hex.BiomeType == targetBiome && hex.CurrentUnit == null && hex.CurrentBuilding == null)
                .ToList();

            if (spawnArea.Count == 0) return;

            int unitsToSpawn = Random.Range(_enemySpawnerModel.MinUnitsPerWave, _enemySpawnerModel.MaxUnitsPerWave + 1);
            unitsToSpawn = Mathf.Min(unitsToSpawn, spawnArea.Count);

            for (int i = 0; i < unitsToSpawn; i++)
            {
                if (spawnArea.Count == 0) break;

                int spawnIndex = Random.Range(0, spawnArea.Count);
                HexModel spawnHex = spawnArea[spawnIndex];
                spawnArea.RemoveAt(spawnIndex);

                Unit unitPrefab = _enemySpawnerModel.EnemyUnitPrefabs[Random.Range(0, _enemySpawnerModel.EnemyUnitPrefabs.Length)];
                _unitsController.SpawnEnemyUnit(unitPrefab.UnitType, spawnHex);
            }
        }
    }
}