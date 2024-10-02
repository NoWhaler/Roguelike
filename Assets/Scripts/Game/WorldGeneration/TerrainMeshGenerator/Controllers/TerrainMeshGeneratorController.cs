using System.Collections.Generic;
using Game.WorldGeneration.ChunkGeneration;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RTT
{
    public class TerrainMeshGeneratorController
    {
        private Dictionary<Vector2Int, TerrainChunk> terrainChunks = new Dictionary<Vector2Int, TerrainChunk>();
        private Dictionary<Vector2Int, ChunkData> chunkDataCache = new Dictionary<Vector2Int, ChunkData>();
        private TerrainMeshGeneratorModel _terrainMeshGeneratorModel;
        private readonly Color32 _waterColor = new (35,137,218, 255);

        [Inject]
        private void Constructor(TerrainMeshGeneratorModel terrainMeshGeneratorModel)
        {
            _terrainMeshGeneratorModel = terrainMeshGeneratorModel;
        }
        
        public void GenerateChunks(List<NodeModel> nodes, Vector3 startPoint, int chunksPerSide)
        {
            float worldSize = chunksPerSide * _terrainMeshGeneratorModel.ChunkSize;
            Vector2 centerOffset = new Vector2(worldSize / 2f, worldSize / 2f);

            for (int y = 0; y < chunksPerSide; y++)
            {
                for (int x = 0; x < chunksPerSide; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(x, y);
                    ChunkData chunkData = GenerateChunkData(chunkCoord, nodes, startPoint, worldSize, centerOffset);
                    chunkDataCache[chunkCoord] = chunkData;
                }
            }

            for (int y = 0; y < chunksPerSide; y++)
            {
                for (int x = 0; x < chunksPerSide; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(x, y);
                    CreateTerrainChunk(chunkCoord, chunksPerSide);
                }
            }
        }

        private ChunkData GetChunkData(Vector2Int coord)
        {
            return chunkDataCache.TryGetValue(coord, out ChunkData data) ? data : null;
        }

        private void CreateTerrainChunk(Vector2Int coord, int chunksPerSide)
        {
            GameObject meshObject = new GameObject($"Terrain Chunk {coord.x}, {coord.y}");
            meshObject.transform.parent = _terrainMeshGeneratorModel.transform;

            Vector3 position = new Vector3(coord.x * _terrainMeshGeneratorModel.ChunkSize, 0, coord.y * _terrainMeshGeneratorModel.ChunkSize);
            meshObject.transform.position = position;

            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _terrainMeshGeneratorModel.TerrainMaterial;

            Mesh mesh = GenerateTerrainMesh(coord, chunksPerSide);
            meshFilter.mesh = mesh;

            TerrainChunk chunk = new TerrainChunk
            {
                meshObject = meshObject,
                position = coord,
                bounds = new Bounds(position, Vector3.one * _terrainMeshGeneratorModel.ChunkSize)
            };

            terrainChunks[coord] = chunk;
        }

        private Mesh GenerateTerrainMesh(Vector2Int coord, int chunksPerSide)
        {
            Mesh mesh = new Mesh();
            ChunkData currentChunk = GetChunkData(coord);

            int vertexSize = _terrainMeshGeneratorModel.ChunkSize + 3;
            Vector3[] vertices = new Vector3[vertexSize * vertexSize];
            Color[] colors = new Color[vertices.Length];
            int[] triangles = new int[_terrainMeshGeneratorModel.ChunkSize * _terrainMeshGeneratorModel.ChunkSize * 6];
            Vector2[] uvs = new Vector2[vertices.Length];

            ChunkData northChunk = coord.y < chunksPerSide - 1 ? GetChunkData(coord + Vector2Int.up) : null;
            ChunkData southChunk = coord.y > 0 ? GetChunkData(coord + Vector2Int.down) : null;
            ChunkData eastChunk = coord.x < chunksPerSide - 1 ? GetChunkData(coord + Vector2Int.right) : null;
            ChunkData westChunk = coord.x > 0 ? GetChunkData(coord + Vector2Int.left) : null;

            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int y = -1; y <= _terrainMeshGeneratorModel.ChunkSize + 1; y++)
            {
                for (int x = -1; x <= _terrainMeshGeneratorModel.ChunkSize + 1; x++)
                {
                    float height;
                    Color vertexColor;

                    if (x >= 0 && x < _terrainMeshGeneratorModel.ChunkSize && y >= 0 && y < _terrainMeshGeneratorModel.ChunkSize)
                    {
                        height = currentChunk.heightMap[x, y];
                        vertexColor = currentChunk.colorMap[y * _terrainMeshGeneratorModel.ChunkSize + x];
                    }
                    else
                    {
                        height = GetHeightFromNeighbor(x, y, currentChunk, northChunk, southChunk, eastChunk, westChunk);
                        vertexColor = GetColorFromNeighbor(x, y, currentChunk, northChunk, southChunk, eastChunk, westChunk);
                    }

                    vertices[vertexIndex] = new Vector3(x, _terrainMeshGeneratorModel.HeightCurve.Evaluate(height) * _terrainMeshGeneratorModel.HeightMultiplier, y);
                    uvs[vertexIndex] = new Vector2(x / (float)_terrainMeshGeneratorModel.ChunkSize, y / (float)_terrainMeshGeneratorModel.ChunkSize);
                    colors[vertexIndex] = vertexColor;

                    if (x >= 0 && x < _terrainMeshGeneratorModel.ChunkSize && y >= 0 && y < _terrainMeshGeneratorModel.ChunkSize)
                    {
                        int a = vertexIndex;
                        int b = vertexIndex + vertexSize;
                        int c = vertexIndex + 1;
                        int d = vertexIndex + vertexSize + 1;

                        triangles[triangleIndex] = a;
                        triangles[triangleIndex + 1] = b;
                        triangles[triangleIndex + 2] = c;
                        triangles[triangleIndex + 3] = c;
                        triangles[triangleIndex + 4] = b;
                        triangles[triangleIndex + 5] = d;

                        triangleIndex += 6;
                    }

                    vertexIndex++;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors;

            mesh.RecalculateNormals();
            SmoothNormals(mesh);

            return mesh;
        }

        private float GetHeightFromNeighbor(int x, int y, ChunkData currentChunk, 
            ChunkData northChunk, ChunkData southChunk, ChunkData eastChunk, ChunkData westChunk)
        {
            if (x == -1 && westChunk != null)
                return westChunk.heightMap[_terrainMeshGeneratorModel.ChunkSize - 1, Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];
            if (x == _terrainMeshGeneratorModel.ChunkSize && eastChunk != null)
                return eastChunk.heightMap[0, Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];
            if (y == -1 && southChunk != null)
                return southChunk.heightMap[Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1), _terrainMeshGeneratorModel.ChunkSize - 1];
            if (y == _terrainMeshGeneratorModel.ChunkSize && northChunk != null)
                return northChunk.heightMap[Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1), 0];

            return currentChunk.heightMap[
                Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1), 
                Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];
        }

        private Color GetColorFromNeighbor(int x, int y, ChunkData currentChunk,
            ChunkData northChunk, ChunkData southChunk, ChunkData eastChunk, ChunkData westChunk)
        {
            if (x == -1 && westChunk != null)
                return westChunk.colorMap[Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1) * _terrainMeshGeneratorModel.ChunkSize + (_terrainMeshGeneratorModel.ChunkSize - 1)];
            if (x == _terrainMeshGeneratorModel.ChunkSize && eastChunk != null)
                return eastChunk.colorMap[Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1) * _terrainMeshGeneratorModel.ChunkSize];
            if (y == -1 && southChunk != null)
                return southChunk.colorMap[(_terrainMeshGeneratorModel.ChunkSize - 1) * _terrainMeshGeneratorModel.ChunkSize + Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];
            if (y == _terrainMeshGeneratorModel.ChunkSize && northChunk != null)
                return northChunk.colorMap[Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];

            return currentChunk.colorMap[
                Mathf.Clamp(y, 0, _terrainMeshGeneratorModel.ChunkSize - 1) * _terrainMeshGeneratorModel.ChunkSize + 
                Mathf.Clamp(x, 0, _terrainMeshGeneratorModel.ChunkSize - 1)];
        }

        private void SmoothNormals(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Dictionary<Vector3, List<int>> vertexGroups = new Dictionary<Vector3, List<int>>();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!vertexGroups.ContainsKey(vertices[i]))
                {
                    vertexGroups[vertices[i]] = new List<int>();
                }
                vertexGroups[vertices[i]].Add(i);
            }

            foreach (var group in vertexGroups)
            {
                Vector3 averageNormal = Vector3.zero;
                foreach (int index in group.Value)
                {
                    averageNormal += normals[index];
                }
                averageNormal.Normalize();

                foreach (int index in group.Value)
                {
                    normals[index] = averageNormal;
                }
            }

            mesh.normals = normals;
        }
        
        private ChunkData GenerateChunkData(Vector2Int chunkCoord, List<NodeModel> nodes, 
            Vector3 startPoint, float worldSize, Vector2 centerOffset)
        {
            ChunkData chunkData = new ChunkData();
            chunkData.heightMap = GenerateHeightMap(chunkCoord, worldSize);
            chunkData.colorMap = GenerateColorMap(chunkCoord, nodes, centerOffset);
            
            return chunkData;
        }

        private float[,] GenerateHeightMap(Vector2Int chunkCoord, float worldSize)
        {
            float[,] heightMap = new float[_terrainMeshGeneratorModel.ChunkSize + 1, _terrainMeshGeneratorModel.ChunkSize + 1];
            
            heightMap[0, 0] = Random.Range(0f, 1f);
            heightMap[0, _terrainMeshGeneratorModel.ChunkSize] = Random.Range(0f, 1f);
            heightMap[_terrainMeshGeneratorModel.ChunkSize, 0] = Random.Range(0f, 1f);
            heightMap[_terrainMeshGeneratorModel.ChunkSize, _terrainMeshGeneratorModel.ChunkSize] = Random.Range(0f, 1f);

            int step = _terrainMeshGeneratorModel.ChunkSize;
            float scale = 1.0f;
            while (step > 1)
            {
                int halfStep = step / 2;

                for (int y = halfStep; y < _terrainMeshGeneratorModel.ChunkSize; y += step)
                {
                    for (int x = halfStep; x < _terrainMeshGeneratorModel.ChunkSize; x += step)
                    {
                        float average = heightMap[x - halfStep, y - halfStep] + 
                                       heightMap[x - halfStep, y + halfStep] +
                                       heightMap[x + halfStep, y - halfStep] +
                                       heightMap[x + halfStep, y + halfStep];
                        average /= 4.0f;
                        heightMap[x, y] = average + Random.Range(-scale, scale);
                    }
                }

                for (int y = 0; y <= _terrainMeshGeneratorModel.ChunkSize; y += halfStep)
                {
                    for (int x = (y + halfStep) % step; x <= _terrainMeshGeneratorModel.ChunkSize; x += step)
                    {
                        float average = 0;
                        int count = 0;

                        if (y >= halfStep) { average += heightMap[x, y - halfStep]; count++; }
                        if (y + halfStep <= _terrainMeshGeneratorModel.ChunkSize) { average += heightMap[x, y + halfStep]; count++; }
                        if (x >= halfStep) { average += heightMap[x - halfStep, y]; count++; }
                        if (x + halfStep <= _terrainMeshGeneratorModel.ChunkSize) { average += heightMap[x + halfStep, y]; count++; }

                        average /= count;
                        heightMap[x, y] = average + Random.Range(-scale, scale);
                    }
                }

                step /= 2;
                scale /= 2.0f;
            }

            return heightMap;
        }

        private Color[] GenerateColorMap(Vector2Int chunkCoord, List<NodeModel> nodes, Vector2 centerOffset)
        {
            Color[] colorMap = new Color[_terrainMeshGeneratorModel.ChunkSize * _terrainMeshGeneratorModel.ChunkSize];
            Vector2 chunkWorldPos = new Vector2(chunkCoord.x * _terrainMeshGeneratorModel.ChunkSize, 
                chunkCoord.y * _terrainMeshGeneratorModel.ChunkSize);

            float maxDistance = 40f;
            Color waterColor = _waterColor;

            for (int y = 0; y < _terrainMeshGeneratorModel.ChunkSize; y++)
            {
                for (int x = 0; x < _terrainMeshGeneratorModel.ChunkSize; x++)
                {
                    Vector2 worldPoint = chunkWorldPos + new Vector2(x, y) - centerOffset;
                    Vector3 worldPos3D = new Vector3(worldPoint.x, 0, worldPoint.y);

                    float minDistance = float.MaxValue;
                    NodeModel closestNode = null;

                    foreach (var node in nodes)
                    {
                        float dist = Vector3.Distance(worldPos3D, node.Position);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            closestNode = node;
                        }
                    }

                    Color finalColor;
                    if (minDistance > maxDistance)
                    {
                        finalColor = waterColor;
                    }
                    else
                    {
                        Color biomeColor = GetBiomeColor(closestNode);
                        
                        float blendFactor = minDistance / maxDistance;
                        finalColor = Color.Lerp(biomeColor, waterColor, blendFactor);
                    }

                    colorMap[y * _terrainMeshGeneratorModel.ChunkSize + x] = finalColor;
                }
            }

            return colorMap;
        }

        private Color GetBiomeColor(NodeModel node)
        {
            Color biomeColor = node.NodeColor;
            biomeColor.a = 1f;
            
            return biomeColor;
        }
    }
}