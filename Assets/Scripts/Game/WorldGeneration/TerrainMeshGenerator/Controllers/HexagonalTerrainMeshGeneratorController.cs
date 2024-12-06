using System.Collections.Generic;
using System.Linq;
using Game.Buildings.Controller;
using Game.Buildings.Enum;
using Game.Hex;
using Game.Hex.Struct;
using Game.ProductionResources;
using Game.ProductionResources.Controller;
using Game.Units.Enum;
using Game.WorldGeneration.Biomes.Enum;
using Game.WorldGeneration.ChunkGeneration.Model;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.RTT.Models;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.TerrainMeshGenerator.Controllers
{
    public class HexagonalTerrainMeshGeneratorController
    {
        private HexagonalTerrainMeshGeneratorModel _hexagonalTerrainMeshGeneratorModel;
        private RRTAlgorithmModel _rrtAlgorithmModel;
        private HexGridController _hexGridController;
        private ResourcesController _resourcesController;
        private BuildingsController _buildingsController;
        private IslandBoundaryController _islandBoundaryController;
        private Dictionary<Vector2Int, HexagonalTerrainMeshGeneratorModel.HexChunk> _chunks;

        private DiContainer _diContainer;

        [Inject]
        private void Constructor(HexagonalTerrainMeshGeneratorModel hexagonalTerrainMeshGeneratorModel,
            HexGridController hexGridController, RRTAlgorithmModel rrtAlgorithmModel,
            ResourcesController resourcesController, BuildingsController buildingsController, DiContainer diContainer)
        {
            _hexagonalTerrainMeshGeneratorModel = hexagonalTerrainMeshGeneratorModel;
            _rrtAlgorithmModel = rrtAlgorithmModel;
            _hexGridController = hexGridController;
            _resourcesController = resourcesController;
            _buildingsController = buildingsController;
            _islandBoundaryController = new IslandBoundaryController(
                    rrtAlgorithmModel.CenterPoint,
                    rrtAlgorithmModel.Radius * 0.8f
                );
            _diContainer = diContainer;
            _chunks = new Dictionary<Vector2Int, HexagonalTerrainMeshGeneratorModel.HexChunk>();
        }
        
        public void GenerateChunks(List<NodeModel> nodes, int chunksPerSide)
        {
            ClearExistingChunks();
            
            for (int x = -chunksPerSide; x <= chunksPerSide; x++)
            {
                for (int y = -chunksPerSide; y <= chunksPerSide; y++)
                {
                    Vector2Int chunkCoord = new Vector2Int(x, y);
                    GenerateChunk(chunkCoord, nodes);
                }
            }
            
            Dictionary<(BiomeType, int), List<HexModel>> hexesByBiomeInstance = new Dictionary<(BiomeType, int), List<HexModel>>();
            foreach (var hex in _hexGridController.GetAllHexes().Values)
            {
                var key = (hex.BiomeType, hex.BiomeIndex);
                if (!hexesByBiomeInstance.ContainsKey(key))
                {
                    hexesByBiomeInstance[key] = new List<HexModel>();
                }
                hexesByBiomeInstance[key].Add(hex);
            }

            foreach (var kvp in hexesByBiomeInstance)
            {
                GenerateResources(kvp.Value, kvp.Key.Item1);
                PlaceNeutralHouse(kvp.Value);
            }
        }
        
        private void GenerateChunk(Vector2Int chunkCoord, List<NodeModel> nodes)
        {
            var chunk = new HexagonalTerrainMeshGeneratorModel.HexChunk(chunkCoord);

            float chunkOffsetX = chunkCoord.x * (_hexagonalTerrainMeshGeneratorModel.HexWidth * 0.75f * _hexagonalTerrainMeshGeneratorModel.HexagonsPerChunkSide);
            float chunkOffsetZ = chunkCoord.y * (_hexagonalTerrainMeshGeneratorModel.HexHeight * _hexagonalTerrainMeshGeneratorModel.HexagonsPerChunkSide) + 
                         ((Mathf.Abs(chunkCoord.x) % 2) == 1 ? _hexagonalTerrainMeshGeneratorModel.HexHeight * 0.5f : 0);

            var chunkObject = CreateChunkGameObject(chunk);
            
            for (int x = 0; x < _hexagonalTerrainMeshGeneratorModel.HexagonsPerChunkSide; x++)
            {
                for (int y = 0; y < _hexagonalTerrainMeshGeneratorModel.HexagonsPerChunkSide; y++)
                {
                    Vector3 hexCenter = CalculateHexPosition(x, y, chunkOffsetX, chunkOffsetZ);
                    NodeModel closestNode = GetClosestNode(hexCenter, nodes);
                    if (closestNode != null)
                    {
                        GenerateHexagonMesh(chunk, hexCenter, closestNode.NodeColor, closestNode.BiomeType, closestNode);
                    }
                }
            }

            RenderChunkObject(chunk, ref chunkObject);
            _chunks[chunkCoord] = chunk;
        }
        
        private void PlaceNeutralHouse(List<HexModel> hexesInBiomeInstance)
        {
            var availableHexes = hexesInBiomeInstance
                .Where(hex => hex.CurrentBuilding == null && hex.ResourceDeposit == null)
                .ToList();

            if (availableHexes.Count > 0)
            {
                int randomIndex = Random.Range(0, availableHexes.Count);
                var selectedHex = availableHexes[randomIndex];
                _buildingsController.SpawnBuilding(BuildingType.House, selectedHex, TeamOwner.Neutral);
            }
        }
        
        private Vector3 CalculateHexPosition(int x, int y, float offsetX, float offsetZ)
        {
            float xPos = (x * _hexagonalTerrainMeshGeneratorModel.HexWidth * 0.75f) + offsetX;
            float zPos = ((y + (x % 2) * 0.5f) * _hexagonalTerrainMeshGeneratorModel.HexHeight) + offsetZ;
            return new Vector3(xPos, 0, zPos);
        }

        private void GenerateHexagonMesh(HexagonalTerrainMeshGeneratorModel.HexChunk chunk, Vector3 center, Color color,
            BiomeType biomeType, NodeModel closestNode)
        {
            if (!_islandBoundaryController.IsWithinBoundary(center))
            {
                return;
            }
            
            int vertexOffset = chunk.Vertices.Count;
            
            Matrix4x4 uvRotation = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 30, 0),
                    Vector3.one
                );
            
            if (chunk.UVs == null)
            {
                chunk.UVs = new List<Vector2>();
            }
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * _hexagonalTerrainMeshGeneratorModel.HexRadius,
                    0,
                    Mathf.Sin(angle) * _hexagonalTerrainMeshGeneratorModel.HexRadius
                );
                chunk.Vertices.Add(vertex);
                chunk.Colors.Add(color);
                
                Vector3 normalizedPos = vertex - center;
                normalizedPos = uvRotation.MultiplyPoint3x4(normalizedPos);
                
                Vector2 uv = new Vector2(
                    (normalizedPos.x / _hexagonalTerrainMeshGeneratorModel.HexRadius + 1f) * 0.5f,
                    (normalizedPos.z / _hexagonalTerrainMeshGeneratorModel.HexRadius + 1f) * 0.5f
                );
                
                chunk.UVs.Add(uv);
            }
            
            chunk.Vertices.Add(center);
            chunk.Colors.Add(color);
            chunk.UVs.Add(new Vector2(0.5f, 0.5f));
            
            int centerVertex = chunk.Vertices.Count - 1;
            for (int i = 0; i < 6; i++)
            {
                int nextI = (i + 1) % 6;
                
                chunk.Triangles.Add(centerVertex);
                chunk.Triangles.Add(vertexOffset + i);
                chunk.Triangles.Add(vertexOffset + nextI);
            }
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * _hexagonalTerrainMeshGeneratorModel.HexRadius,
                    -0.1f,
                    Mathf.Sin(angle) * _hexagonalTerrainMeshGeneratorModel.HexRadius
                );
                chunk.Vertices.Add(vertex);
                chunk.Colors.Add(color);
                
                Vector2 uv = chunk.UVs[i];
                chunk.UVs.Add(uv);
            }
            
            for (int i = 0; i < 4; i++)
            {
                chunk.Triangles.Add(vertexOffset);
                chunk.Triangles.Add(vertexOffset + i + 1);
                chunk.Triangles.Add(vertexOffset + i + 2);
            }
            
            int bottomOffset = vertexOffset + 6;
            for (int i = 0; i < 4; i++)
            {
                chunk.Triangles.Add(bottomOffset);
                chunk.Triangles.Add(bottomOffset + i + 2);
                chunk.Triangles.Add(bottomOffset + i + 1);
            }
            
            for (int i = 0; i < 6; i++)
            {
                int nextI = (i + 1) % 6;
                
                chunk.Triangles.Add(vertexOffset + i);
                chunk.Triangles.Add(bottomOffset + i);
                chunk.Triangles.Add(vertexOffset + nextI);
                
                chunk.Triangles.Add(bottomOffset + i);
                chunk.Triangles.Add(bottomOffset + nextI);
                chunk.Triangles.Add(vertexOffset + nextI);
            }
            
            HexModel hexObject = _diContainer.InstantiatePrefabForComponent<HexModel>(
                    _hexagonalTerrainMeshGeneratorModel.HexPrefab, 
                    center, 
                    Quaternion.identity, 
                    chunk.ChunkObject.transform
                );
            
            hexObject.HexPosition = center;
            
            HexCoordinate logicalCoords = WorldToHexCoordinate(center);
            hexObject.HexPosition = center;
            
            hexObject.BiomeType = biomeType;
            hexObject.BiomeIndex = closestNode.BiomeIndex;
            
            hexObject.SetFog(true);
            
            hexObject.SetLogicalCoordinates(logicalCoords.Q, logicalCoords.R, logicalCoords.S);

            _hexGridController.SetHex(hexObject);
            hexObject.name = $"Hex({logicalCoords.Q}, {logicalCoords.R}, {logicalCoords.S})";
        }
        
        private HexCoordinate WorldToHexCoordinate(Vector3 worldPosition)
        {
            float q = (2f/3 * worldPosition.x) / _hexagonalTerrainMeshGeneratorModel.HexRadius;
            float r = (-1f/3 * worldPosition.x + Mathf.Sqrt(3)/3 * worldPosition.z) / _hexagonalTerrainMeshGeneratorModel.HexRadius;
            
            float x = q;
            float z = r;
            float y = -x - z;
    
            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);
    
            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);
    
            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                rz = -rx - ry;
    
            return new HexCoordinate(rx, rz);
        }
        
        private HexTerrainChunkModel CreateChunkGameObject(HexagonalTerrainMeshGeneratorModel.HexChunk chunk)
        {
            HexTerrainChunkModel hexTerrainChunkModel =
                _diContainer.InstantiatePrefabForComponent<HexTerrainChunkModel>(
                    _hexagonalTerrainMeshGeneratorModel.HexTerrainChunkPrefab, Vector3.zero, Quaternion.identity,
                    _hexagonalTerrainMeshGeneratorModel.transform);

            hexTerrainChunkModel.gameObject.name = $"Chunk({chunk.ChunkCoord.x}, {chunk.ChunkCoord.y})";
            chunk.ChunkObject = hexTerrainChunkModel;
            
            return hexTerrainChunkModel;
        }
        
        private void GenerateResources(List<HexModel> hexesInBiomeInstance, BiomeType biomeType)
        {
            var biome = _rrtAlgorithmModel.Biomes.Find(b => b.BiomeType == biomeType);
            if (biome?.Resources == null) return;

            foreach (var resourceInfo in biome.Resources)
            {
                var availableHexes = hexesInBiomeInstance
                    .Where(hex => hex.CurrentBuilding == null && hex.ResourceDeposit == null)
                    .ToList();

                int resourcesToPlace = resourceInfo.Count;
                int resourcesPlaced = 0;

                while (resourcesPlaced < resourcesToPlace && availableHexes.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableHexes.Count);
                    HexModel selectedHex = availableHexes[randomIndex];

                    var resourcePrefab = _resourcesController.GetResourcePrefab(resourceInfo.ResourceType);
                    Vector3 spawnPosition = selectedHex.HexPosition + Vector3.up * 1f;
                    var resourceDeposit = _diContainer.InstantiatePrefabForComponent<ResourceDeposit>(
                        resourcePrefab,
                        spawnPosition,
                        Quaternion.identity,
                        selectedHex.transform
                    );

                    selectedHex.ResourceDeposit = resourceDeposit;
                    resourcesPlaced++;
                    availableHexes.RemoveAt(randomIndex);
                }
            }
        }

        private void RenderChunkObject(HexagonalTerrainMeshGeneratorModel.HexChunk chunk, ref HexTerrainChunkModel hexTerrainChunkModel)
        {
            Mesh mesh = new Mesh
            {
                vertices = chunk.Vertices.ToArray(),
                triangles = chunk.Triangles.ToArray(),
                colors = chunk.Colors.ToArray(),
                uv = chunk.UVs.ToArray()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            hexTerrainChunkModel.ChunkMeshFilter.mesh = mesh;
            
            hexTerrainChunkModel.ChunkMeshRenderer.material = _hexagonalTerrainMeshGeneratorModel.DefaultMaterial;
        }
        
        private NodeModel GetClosestNode(Vector3 position, List<NodeModel> nodes)
        {
            NodeModel closestNode = null;
            float closestDistance = float.MaxValue;
            
            foreach (var node in nodes)
            {
                float distance = Vector2.Distance(
                    new Vector2(position.x, position.z),
                    new Vector2(node.Position.x, node.Position.z));
                    
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }
            
            return closestNode;
        }
        
        private void ClearExistingChunks()
        {
            foreach (var chunk in _chunks.Values)
            {
                if (chunk.ChunkObject != null)
                {
                    Object.Destroy(chunk.ChunkObject);
                }
            }
            _chunks.Clear();
        }
    }
}