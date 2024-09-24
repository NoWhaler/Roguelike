using System;
using System.Collections.Generic;
using Game.WorldGeneration.ChunkGeneration.Model;
using Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Controllers
{
    public class PerlinNoiseGeneratorController : IInitializable, IDisposable
    {
        private PerlinNoiseGeneratorModel _perlinNoiseGeneratorModel;
        private DiContainer _diContainer;
        private readonly Dictionary<Vector2Int, ChunkModel> _chunkMeshes = new Dictionary<Vector2Int, ChunkModel>();
        private float[,] _globalNoiseMap;

        public void Initialize()
        {
            GenerateMap();
            _perlinNoiseGeneratorModel.OnGenerateMap += GenerateMap;
        }

        public void Dispose()
        {
            _perlinNoiseGeneratorModel.OnGenerateMap -= GenerateMap;
        }

        [Inject]
        private void Constructor(PerlinNoiseGeneratorModel perlinNoiseGeneratorModel, DiContainer diContainer)
        {
            _perlinNoiseGeneratorModel = perlinNoiseGeneratorModel;
            _diContainer = diContainer;
        }

        private void GenerateMap()
        {
            ClearChunks();

            int totalSize = _perlinNoiseGeneratorModel.ChunksPerSide * _perlinNoiseGeneratorModel.ChunkSize + 1;
            _globalNoiseMap = GenerateGlobalNoiseMap(totalSize);
            
            for (int chunkY = 0; chunkY < _perlinNoiseGeneratorModel.ChunksPerSide; chunkY++)
            {
                for (int chunkX = 0; chunkX < _perlinNoiseGeneratorModel.ChunksPerSide; chunkX++)
                {
                    GenerateChunk(chunkX, chunkY);
                }
            }
            
            SmoothBorders();
        }

        private void GenerateChunk(int chunkX, int chunkY)
        {
            float[,] noiseMap = ExtractNoiseMapForChunk(_globalNoiseMap, chunkX, chunkY);
            
            noiseMap = SmoothCoastline(noiseMap, _perlinNoiseGeneratorModel.CoastlineSmoothPasses);
            
            Color[] colorMap = GenerateColorMap(noiseMap);
            MeshData meshData = GenerateTerrainMesh(noiseMap, chunkX, chunkY);
            
            Mesh mesh = meshData.CreateMesh();
            mesh.RecalculateNormals();
    
            ChunkModel chunkObject = _diContainer.InstantiatePrefabForComponent<ChunkModel>(_perlinNoiseGeneratorModel.ChunkPrefab,
                new Vector3(chunkX * _perlinNoiseGeneratorModel.ChunkSize, 0, chunkY * _perlinNoiseGeneratorModel.ChunkSize),
                Quaternion.identity, _perlinNoiseGeneratorModel.GenerationModelTransform);
            
            _chunkMeshes[new Vector2Int(chunkX, chunkY)] = chunkObject;
            chunkObject.gameObject.name = $"Chunk_{chunkX}_{chunkY}";
            chunkObject.ChunkMeshFilter.mesh = mesh;
            chunkObject.ChunkMeshCollider.sharedMesh = mesh;
            
            chunkObject.ChunkMeshRenderer.material = _perlinNoiseGeneratorModel.ChunkMaterial;
            ApplyColorToMesh(mesh, colorMap);
        }
        
        private void SmoothBorders()
        {
            int smoothingPasses = _perlinNoiseGeneratorModel.SmoothNormalsPasses;
            for (int pass = 0; pass < smoothingPasses; pass++)
            {
                foreach (var chunkEntry in _chunkMeshes)
                {
                    Vector2Int chunkCoord = chunkEntry.Key;
                    ChunkModel chunk = chunkEntry.Value;

                    ChunkModel leftChunk = _chunkMeshes.ContainsKey(new Vector2Int(chunkCoord.x - 1, chunkCoord.y))
                                           ? _chunkMeshes[new Vector2Int(chunkCoord.x - 1, chunkCoord.y)] : null;
                    ChunkModel rightChunk = _chunkMeshes.ContainsKey(new Vector2Int(chunkCoord.x + 1, chunkCoord.y))
                                            ? _chunkMeshes[new Vector2Int(chunkCoord.x + 1, chunkCoord.y)] : null;
                    ChunkModel topChunk = _chunkMeshes.ContainsKey(new Vector2Int(chunkCoord.x, chunkCoord.y + 1))
                                          ? _chunkMeshes[new Vector2Int(chunkCoord.x, chunkCoord.y + 1)] : null;
                    ChunkModel bottomChunk = _chunkMeshes.ContainsKey(new Vector2Int(chunkCoord.x, chunkCoord.y - 1))
                                             ? _chunkMeshes[new Vector2Int(chunkCoord.x, chunkCoord.y - 1)] : null;

                    Mesh chunkMesh = chunk.ChunkMeshFilter.mesh;
                    Vector3[] vertices = chunkMesh.vertices;
                    Vector3[] normals = chunkMesh.normals;

                    int chunkSize = _perlinNoiseGeneratorModel.ChunkSize;

                    if (leftChunk != null)
                    {
                        SmoothVerticalEdge(vertices, normals, leftChunk.ChunkMeshFilter.mesh.vertices, leftChunk.ChunkMeshFilter.mesh.normals, 0, chunkSize);
                    }

                    if (rightChunk != null)
                    {
                        SmoothVerticalEdge(vertices, normals, rightChunk.ChunkMeshFilter.mesh.vertices, rightChunk.ChunkMeshFilter.mesh.normals, chunkSize, 0);
                    }

                    if (topChunk != null)
                    {
                        SmoothHorizontalEdge(vertices, normals, topChunk.ChunkMeshFilter.mesh.vertices, topChunk.ChunkMeshFilter.mesh.normals, chunkSize, 0);
                    }

                    if (bottomChunk != null)
                    {
                        SmoothHorizontalEdge(vertices, normals, bottomChunk.ChunkMeshFilter.mesh.vertices, bottomChunk.ChunkMeshFilter.mesh.normals, 0, chunkSize);
                    }

                    HandleCornerVertices(chunkCoord, vertices, normals);

                    chunkMesh.vertices = vertices;
                    chunkMesh.normals = normals;
                    // chunkMesh.RecalculateNormals();
                }
            }
        }
        
        private void HandleCornerVertices(Vector2Int chunkCoord, Vector3[] vertices, Vector3[] normals)
        {
            int chunkSize = _perlinNoiseGeneratorModel.ChunkSize + 1;
        
            Vector2Int[] cornerOffsets = {
                new(-1, -1), new(1, -1),
                new(-1, 1), new(1, 1)
            };
        
            foreach (var offset in cornerOffsets)
            {
                Vector2Int neighborCoord = chunkCoord + offset;
        
                if (_chunkMeshes.ContainsKey(neighborCoord))
                {
                    ChunkModel neighbor = _chunkMeshes[neighborCoord];
                    Vector3[] neighborVertices = neighbor.ChunkMeshFilter.mesh.vertices;
                    Vector3[] neighborNormals = neighbor.ChunkMeshFilter.mesh.normals;
        
                    int index1 = GetCornerVertexIndex(offset, chunkSize);
                    int index2 = GetCornerVertexIndex(-offset, chunkSize);
        
                    float averageHeight = (vertices[index1].y + neighborVertices[index2].y) / 2;
                    vertices[index1].y = averageHeight;
                    neighborVertices[index2].y = averageHeight;
        
                    Vector3 averageNormal = (normals[index1] + neighborNormals[index2]).normalized;
                    normals[index1] = averageNormal;
                    neighborNormals[index2] = averageNormal;
                }
            }
        }
        
        private int GetCornerVertexIndex(Vector2Int offset, int chunkSize)
        {
            if (offset == new Vector2Int(-1, -1)) 
                return 0;
            else if (offset == new Vector2Int(1, -1)) 
                return chunkSize - 1;
            else if (offset == new Vector2Int(-1, 1)) 
                return (chunkSize - 1) * chunkSize;
            else 
                return chunkSize * chunkSize - 1;
        }
        
        private void SmoothVerticalEdge(Vector3[] vertices, Vector3[] normals, Vector3[] neighborVertices, Vector3[] neighborNormals, int localX, int neighborX)
        {
            int chunkSize = _perlinNoiseGeneratorModel.ChunkSize + 1;
            for (int i = 0; i < chunkSize; i++)
            {
                int index1 = i * chunkSize + localX;
                int index2 = i * chunkSize + neighborX;

                float averageHeight = (vertices[index1].y + neighborVertices[index2].y) / 2;
                vertices[index1].y = averageHeight;
                neighborVertices[index2].y = averageHeight;

                Vector3 averageNormal = (normals[index1] + neighborNormals[index2]).normalized;
                normals[index1] = averageNormal;
                neighborNormals[index2] = averageNormal;
            }
        }

        private void SmoothHorizontalEdge(Vector3[] vertices, Vector3[] normals, Vector3[] neighborVertices, Vector3[] neighborNormals, int localY, int neighborY)
        {
            int chunkSize = _perlinNoiseGeneratorModel.ChunkSize + 1;
            for (int i = 0; i < chunkSize; i++)
            {
                int index1 = localY * chunkSize + i;
                int index2 = neighborY * chunkSize + i;

                float averageHeight = (vertices[index1].y + neighborVertices[index2].y) / 2;
                vertices[index1].y = averageHeight;
                neighborVertices[index2].y = averageHeight;

                Vector3 averageNormal = (normals[index1] + neighborNormals[index2]).normalized;
                normals[index1] = averageNormal;
                neighborNormals[index2] = averageNormal;
            }
        }
        
        private float[,] GenerateGlobalNoiseMap(int size)
        {
            System.Random prng = new System.Random(_perlinNoiseGeneratorModel.SeedValue);
            Vector2 randomOffset = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
            
            float[,] noiseMap = PerlinNoise.GenerateNoiseMap(
                size, size,
                _perlinNoiseGeneratorModel.NoiseScale, 
                _perlinNoiseGeneratorModel.Octaves,
                _perlinNoiseGeneratorModel.Persistence, 
                _perlinNoiseGeneratorModel.Lacunarity, 
                randomOffset
            );
            
            // if (_perlinNoiseGeneratorModel.UseFalloffMap)
            // {
            //     float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(size);
            //     for (int y = 0; y < size; y++)
            //     {
            //         for (int x = 0; x < size; x++)
            //         {
            //             noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
            //         }
            //     }
            // }

            return noiseMap;
        }
        
        private float[,] ExtractNoiseMapForChunk(float[,] globalNoiseMap, int chunkX, int chunkY)
        {
            int chunkSize = _perlinNoiseGeneratorModel.ChunkSize;
            float[,] chunkNoiseMap = new float[chunkSize + 1, chunkSize + 1];

            for (int y = 0; y <= chunkSize; y++)
            {
                for (int x = 0; x <= chunkSize; x++)
                {
                    int globalX = x + chunkX * chunkSize;
                    int globalY = y + chunkY * chunkSize;
                    chunkNoiseMap[x, y] = globalNoiseMap[globalX, globalY];
                }
            }

            return chunkNoiseMap;
        }

        private void ClearChunks()
        {
            foreach (var chunk in _chunkMeshes)
            {
                chunk.Value.DestroyChunk();
            }
            
            _chunkMeshes.Clear();
        }

        private float[,] SmoothCoastline(float[,] heightMap, int smoothingPasses)
        {
            int mapSize = heightMap.GetLength(0);
            float[,] smoothedMap = new float[mapSize, mapSize];
            Array.Copy(heightMap, smoothedMap, heightMap.Length);
        
            float waterThreshold = _perlinNoiseGeneratorModel.Regions[0].height;
        
            for (int pass = 0; pass < smoothingPasses; pass++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    for (int x = 1; x < mapSize - 1; x++)
                    {
                        if (IsCoastalTile(smoothedMap, x, y, waterThreshold))
                        {
                            smoothedMap[x, y] = AverageNeighbors(smoothedMap, x, y);
                        }
                    }
                }
            }
        
            return smoothedMap;
        }

        private bool IsCoastalTile(float[,] heightMap, int x, int y, float waterThreshold)
        {
            bool isWater = heightMap[x, y] <= waterThreshold;
            
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0) continue;
                    
                    int checkX = x + offsetX;
                    int checkY = y + offsetY;
                    
                    if (checkX < 0 || checkX >= heightMap.GetLength(0) || checkY < 0 || checkY >= heightMap.GetLength(1))
                        continue;
        
                    if ((heightMap[checkX, checkY] <= waterThreshold) != isWater)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        private float AverageNeighbors(float[,] heightMap, int x, int y)
        {
            float sum = 0;
            int count = 0;
        
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    int checkX = x + offsetX;
                    int checkY = y + offsetY;
                    
                    if (checkX < 0 || checkX >= heightMap.GetLength(0) || checkY < 0 || checkY >= heightMap.GetLength(1))
                        continue;
        
                    sum += heightMap[checkX, checkY];
                    count++;
                }
            }
        
            return sum / count;
        }

        private Color[] GenerateColorMap(float[,] heightMap)
        {
            int mapSize = heightMap.GetLength(0);
            Color[] colorMap = new Color[mapSize * mapSize];

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    float currentHeight = heightMap[x, y];
                    colorMap[y * mapSize + x] = CalculateColor(currentHeight);
                }
            }

            return colorMap;
        }
        
        private Color CalculateColor(float height)
        {
            for (int i = 0; i < _perlinNoiseGeneratorModel.Regions.Length; i++)
            {
                if (height <= _perlinNoiseGeneratorModel.Regions[i].height)
                {
                    if (i == 0) return _perlinNoiseGeneratorModel.Regions[i].color;
    
                    float t = Mathf.InverseLerp(_perlinNoiseGeneratorModel.Regions[i - 1].height, _perlinNoiseGeneratorModel.Regions[i].height, height);
                    return Color.Lerp(_perlinNoiseGeneratorModel.Regions[i - 1].color, _perlinNoiseGeneratorModel.Regions[i].color, t);
                }
            }
    
            return _perlinNoiseGeneratorModel.Regions[_perlinNoiseGeneratorModel.Regions.Length - 1].color;
        }

        private MeshData GenerateTerrainMesh(float[,] heightMap, int chunkX, int chunkY)
        {
            int chunkSize = _perlinNoiseGeneratorModel.ChunkSize;
            int meshSize = chunkSize + 1;
            MeshData meshData = new MeshData(meshSize, meshSize);
            int vertexIndex = 0;

            for (int y = 0; y < meshSize; y++)
            {
                for (int x = 0; x < meshSize; x++)
                {
                    float height = CalculateVertexHeight(heightMap[x, y], chunkX, chunkY, x, y);
                    Vector3 vertex = new Vector3(x, height, y);

                    meshData.vertices[vertexIndex] = vertex;
                    meshData.uvs[vertexIndex] = new Vector2((x + chunkX * chunkSize) / (float)(chunkSize * _perlinNoiseGeneratorModel.ChunksPerSide), 
                                                           (y + chunkY * chunkSize) / (float)(chunkSize * _perlinNoiseGeneratorModel.ChunksPerSide));

                    if (x < chunkSize && y < chunkSize)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + meshSize + 1, vertexIndex + meshSize);
                        meshData.AddTriangle(vertexIndex + meshSize + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }

        private float CalculateVertexHeight(float noiseValue, int chunkX, int chunkY, int localX, int localY)
        {
            int globalX = chunkX * _perlinNoiseGeneratorModel.ChunkSize + localX;
            int globalY = chunkY * _perlinNoiseGeneratorModel.ChunkSize + localY;
            float globalNoiseValue = _globalNoiseMap[globalX, globalY];
            return _perlinNoiseGeneratorModel.HeightCurve.Evaluate(globalNoiseValue) * _perlinNoiseGeneratorModel.HeightMultiplier;
        }

        private void ApplyColorToMesh(Mesh mesh, Color[] colorMap)
        {
            mesh.colors = colorMap;
        }

    }
}