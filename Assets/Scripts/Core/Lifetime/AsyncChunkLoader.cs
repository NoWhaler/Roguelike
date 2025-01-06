using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;

namespace Core.Lifetime
{
    public class AsyncChunkLoader
    {
        private readonly Queue<ChunkLoadOperation> _loadQueue;
        private readonly HashSet<Vector2Int> _loadingChunks;
        private readonly SemaphoreSlim _semaphore;
        private CancellationTokenSource _cancellationSource;

        public AsyncChunkLoader(int maxConcurrentChunks = 4)
        {
            _loadQueue = new Queue<ChunkLoadOperation>();
            _loadingChunks = new HashSet<Vector2Int>();
            _semaphore = new SemaphoreSlim(maxConcurrentChunks);
            _cancellationSource = new CancellationTokenSource();
        }

        public void QueueChunkLoad(Vector2Int chunkCoord, List<NodeModel> nodes)
        {
            if (_loadingChunks.Contains(chunkCoord))
                return;

            _loadQueue.Enqueue(new ChunkLoadOperation(chunkCoord, nodes));
            ProcessQueueAsync().Forget();
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            while (_loadQueue.Count > 0)
            {
                await _semaphore.WaitAsync();

                if (_loadQueue.Count == 0)
                {
                    _semaphore.Release();
                    break;
                }

                var operation = _loadQueue.Dequeue();
                _loadingChunks.Add(operation.ChunkCoord);

                LoadChunkAsync(operation).Forget();
            }
        }

        private async UniTask LoadChunkAsync(ChunkLoadOperation operation)
        {
            try
            {
                await UniTask.SwitchToThreadPool();
                
                var chunk = new HexagonalTerrainMeshGeneratorModel.HexChunk(operation.ChunkCoord);

                await UniTask.SwitchToMainThread();
                
                // Create game objects and set up mesh
                // ... chunk setup code ...

                await UniTask.Delay(10);
            }
            finally
            {
                _loadingChunks.Remove(operation.ChunkCoord);
                _semaphore.Release();
            }
        }

        public void CancelLoading()
        {
            _cancellationSource.Cancel();
            _cancellationSource = new CancellationTokenSource();
            _loadQueue.Clear();
            _loadingChunks.Clear();
        }

        private class ChunkLoadOperation
        {
            public Vector2Int ChunkCoord { get; }
            public List<NodeModel> Nodes { get; }

            public ChunkLoadOperation(Vector2Int chunkCoord, List<NodeModel> nodes)
            {
                ChunkCoord = chunkCoord;
                Nodes = nodes;
            }
        }    
    }
}