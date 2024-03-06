using System.Collections.Generic;
using System.Linq;
using Game.Spawners.EnemySpawnerModels;
using UnityEngine;
using Zenject;

namespace Game.Spawners.EnemySpawnerControllers
{
    public class EnemySpawnerController : IInitializable
    {
        private DiContainer _diContainer;
        private EnemySpawnerModel _enemySpawnerModel;

        [Inject]
        private void Constructor(EnemySpawnerModel enemySpawnerModel, DiContainer diContainer)
        {
            _enemySpawnerModel = enemySpawnerModel;
            _diContainer = diContainer;
        }

        public void Initialize()
        {
            SpawnObstacles();
        }
        
        private void SpawnObstacles()
        {
            var spawnPositions = GenerateSpawnPositions(_enemySpawnerModel.GroundCollider.bounds, _enemySpawnerModel.EnemiesAmount);

            for (var i = 0; i < spawnPositions.Count; i++)
            {
                var position = new Vector3(spawnPositions[i].x, 1.5f, spawnPositions[i].z);
                _diContainer.InstantiatePrefab(_enemySpawnerModel.GameObjectPrefab, position, Quaternion.identity, _enemySpawnerModel.transform);
            }
        }

        private List<Vector3> GenerateSpawnPositions(Bounds bounds, int numPositions)
        {
            var spawnPositions = new List<Vector3>();

            for (var i = 0; i < numPositions; i++)
            {
                var isValidPosition = false;
                var newPosition = Vector3.zero;

                var maxAttempts = 1000;
                var attempts = 0;

                while (!isValidPosition && attempts < maxAttempts)
                {
                    newPosition = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    isValidPosition = IsPositionValid(newPosition, spawnPositions);
                    attempts++;
                }

                if (isValidPosition)
                {
                    spawnPositions.Add(newPosition);
                }
            }

            return spawnPositions;
        }

        private bool IsPositionValid(Vector3 position, IEnumerable<Vector3> existingPositions)
        {
            var colliders = Physics.OverlapSphere(position, 2f);
            
            foreach (var collider in colliders)
            {
                foreach (var obstaclesLayer in _enemySpawnerModel.ObstaclesLayers)
                {
                    if ((collider != null && collider.gameObject.layer == obstaclesLayer) 
                                         || !existingPositions.All(existingPosition => Vector3.Distance(position, existingPosition) > 2f))
                    { 
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}