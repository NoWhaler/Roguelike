using System.Collections.Generic;
using System.Linq;
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
                    ChunkData chunkData = GenerateChunkData(chunkCoord, nodes, centerOffset);
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
        
        
        private Vector2[] GenerateTriplanarUVs(Vector3[] vertices)
        {
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 uvX = new Vector2(vertices[i].z, vertices[i].y);
                Vector2 uvY = new Vector2(vertices[i].x, vertices[i].z);
                Vector2 uvZ = new Vector2(vertices[i].x, vertices[i].y);
                
                Vector3 absNormal = new Vector3(
                    Mathf.Abs(vertices[i].normalized.x),
                    Mathf.Abs(vertices[i].normalized.y),
                    Mathf.Abs(vertices[i].normalized.z)
                );
                
                float maxNormal = Mathf.Max(absNormal.x, Mathf.Max(absNormal.y, absNormal.z));
                
                if (maxNormal == absNormal.x)
                    uvs[i] = uvX;
                else if (maxNormal == absNormal.y)
                    uvs[i] = uvY;
                else
                    uvs[i] = uvZ;
            }
            return uvs;
        }

        private Mesh GenerateTerrainMesh(Vector2Int coord, int chunksPerSide)
        {
            Mesh mesh = new Mesh();
            ChunkData currentChunk = GetChunkData(coord);

            int vertexSize = _terrainMeshGeneratorModel.ChunkSize + 3;
            Vector3[] vertices = new Vector3[vertexSize * vertexSize];
            Color[] colors = new Color[vertices.Length];
            int[] triangles = new int[_terrainMeshGeneratorModel.ChunkSize * _terrainMeshGeneratorModel.ChunkSize * 6];
            Vector2[] uvs = GenerateTriplanarUVs(vertices);

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
        
        private ChunkData GenerateChunkData(Vector2Int chunkCoord, List<NodeModel> nodes, Vector2 centerOffset)
        {
            ChunkData chunkData = new ChunkData();
            chunkData.heightMap = GenerateHeightMap();
            chunkData.colorMap = GenerateColorMap(chunkCoord, nodes, centerOffset);
            
            return chunkData;
        }

        private float[,] GenerateHeightMap()
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

            float transitionDistance = _terrainMeshGeneratorModel.TransitionDistance;
            float maxDistance = _terrainMeshGeneratorModel.MaxDistance;
            float smoothingFactor = _terrainMeshGeneratorModel.SmoothFactor;
            Color waterColor = _terrainMeshGeneratorModel.WaterColor;
            
            float outlineTransitionWidth = _terrainMeshGeneratorModel.OutlineTransitionWidth * 0.1f;
            
            float internalTransitionWidth = _terrainMeshGeneratorModel.InternalTransitionWidth * 0.1f;

            for (int y = 0; y < _terrainMeshGeneratorModel.ChunkSize; y++)
            {
                for (int x = 0; x < _terrainMeshGeneratorModel.ChunkSize; x++)
                {
                    Vector2 worldPoint = chunkWorldPos + new Vector2(x, y) - centerOffset;
                    Vector3 worldPos3D = new Vector3(worldPoint.x, 0, worldPoint.y);

                    List<(NodeModel node, float distance)> nearestNodes = nodes
                        .Select(node => (node: node, distance: Vector3.Distance(worldPos3D, node.Position)))
                        .OrderBy(tuple => tuple.distance)
                        .Take(_terrainMeshGeneratorModel.NearestNodesAmount)
                        .ToList();

                    float distanceToEdge = maxDistance - nearestNodes[0].distance;
                    if (distanceToEdge < outlineTransitionWidth)
                    {
                        float edgeBlendFactor = Mathf.SmoothStep(0, 1, distanceToEdge / outlineTransitionWidth);
                        if (edgeBlendFactor < 0.01f)
                        {
                            colorMap[y * _terrainMeshGeneratorModel.ChunkSize + x] = waterColor;
                            continue;
                        }
                        
                        Color landColor = BlendBiomeColors(nearestNodes, worldPos3D, internalTransitionWidth, smoothingFactor);
                        Color finalColor = Color.Lerp(waterColor, landColor, edgeBlendFactor);
                        colorMap[y * _terrainMeshGeneratorModel.ChunkSize + x] = finalColor;
                        continue;
                    }

                    Color biomeColor = BlendBiomeColors(nearestNodes, worldPos3D, internalTransitionWidth, smoothingFactor);
                    colorMap[y * _terrainMeshGeneratorModel.ChunkSize + x] = biomeColor;
                }
            }

            return colorMap;
        }

        private Color BlendBiomeColors(List<(NodeModel node, float distance)> nearestNodes, Vector3 worldPos3D, float transitionWidth, float smoothingFactor)
        {
            float[] weights = new float[nearestNodes.Count];
            float weightSum = 0;

            for (int i = 0; i < nearestNodes.Count; i++)
            {
                float distance = nearestNodes[i].distance;
                
                float weight = Mathf.Exp(-distance * smoothingFactor / transitionWidth);
                weights[i] = weight;
                weightSum += weight;
            }

            Color blendedColor = Color.black;
            for (int i = 0; i < nearestNodes.Count; i++)
            {
                float normalizedWeight = weights[i] / weightSum;
                Color biomeColor = GetBiomeColor(nearestNodes[i].node);
                blendedColor += biomeColor * normalizedWeight;
            }

            return blendedColor;
        }

        private Color GetBiomeColor(NodeModel node)
        {
            Color biomeColor = node.NodeColor;
            biomeColor.a = 1f;
            
            float saturationIncrease = _terrainMeshGeneratorModel.Saturation; 
            Color.RGBToHSV(biomeColor, out float h, out float s, out float v);
            s = Mathf.Clamp01(s * saturationIncrease);
            return Color.HSVToRGB(h, s, v);
        }
    }
}