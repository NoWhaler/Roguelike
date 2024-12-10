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
        
        private const int MIN_BIOMES_DISTANCE = 3;

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
            
            var hexesByBiomeInstance = _hexGridController.GetAllHexes().Values
                .GroupBy(hex => (hex.BiomeType, hex.BiomeIndex))
                .ToDictionary(g => g.Key, g => g.ToList());

            GenerateStartingPositions(hexesByBiomeInstance);

            foreach (var kvp in hexesByBiomeInstance)
            {
                var biomeCells = kvp.Value;
                
                bool containsMainBuilding = biomeCells.Any(hex => hex.CurrentBuilding?.BuildingType == BuildingType.MainBuilding);
                
                GenerateResources(biomeCells, kvp.Key.Item1);
                
                if (!containsMainBuilding)
                {
                    PlaceNeutralHouses(biomeCells);
                }
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
        
        
        private void GenerateStartingPositions(Dictionary<(BiomeType, int), List<HexModel>> hexesByBiomeInstance)
        {
            var grasslandRegions = hexesByBiomeInstance
                .Where(kvp => kvp.Key.Item1 == BiomeType.Grassland)
                .SelectMany(kvp => FindConnectedRegions(kvp.Value))
                .Where(region => region.Count >= 15)
                .ToList();

            if (grasslandRegions.Count == 0) return;

            var playerRegion = SelectBestStartingRegion(grasslandRegions);
            var playerStartHex = SelectStartingHex(playerRegion);
            _buildingsController.SpawnBuilding(BuildingType.MainBuilding, playerStartHex, TeamOwner.Player);
            RevealRegionFog(playerRegion);

            var enemyRegions = FindPotentialEnemyBaseRegions(hexesByBiomeInstance, playerStartHex);
            if (enemyRegions.Count > 0)
            {
                var enemyRegion = SelectEnemyBaseRegion(enemyRegions, playerStartHex);
                var enemyStartHex = SelectStartingHex(enemyRegion);
                _buildingsController.SpawnBuilding(BuildingType.MainBuilding, enemyStartHex, TeamOwner.Enemy);
            }
        }

        private List<List<HexModel>> FindConnectedRegions(List<HexModel> hexes)
        {
            var regions = new List<List<HexModel>>();
            var visited = new HashSet<HexModel>();

            foreach (var hex in hexes)
            {
                if (!visited.Contains(hex))
                {
                    var region = new List<HexModel>();
                    FloodFillRegion(hex, region, visited);
                    if (region.Count > 0)
                    {
                        regions.Add(region);
                    }
                }
            }

            return regions;
        }

        private void FloodFillRegion(HexModel startHex, List<HexModel> region, HashSet<HexModel> visited)
        {
            var queue = new Queue<HexModel>();
            queue.Enqueue(startHex);
            visited.Add(startHex);
            region.Add(startHex);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in _hexGridController.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor) && 
                        neighbor.BiomeType == startHex.BiomeType && 
                        neighbor.BiomeIndex == startHex.BiomeIndex)
                    {
                        visited.Add(neighbor);
                        region.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        private List<List<HexModel>> FindPotentialEnemyBaseRegions(
            Dictionary<(BiomeType, int), List<HexModel>> hexesByBiomeInstance, 
            HexModel playerStartHex)
        {
            var enemyRegions = new List<List<HexModel>>();
            var validBiomes = hexesByBiomeInstance
                .Where(kvp => kvp.Key.Item1 != BiomeType.Mountain && 
                            kvp.Key.Item1 != BiomeType.Swamp)
                .SelectMany(kvp => FindConnectedRegions(kvp.Value))
                .Where(region => region.Count >= 15)
                .ToList();

            foreach (var region in validBiomes)
            {
                if (IsSafeDistanceFromPlayer(region, playerStartHex))
                {
                    enemyRegions.Add(region);
                }
            }

            return enemyRegions;
        }

        private bool IsSafeDistanceFromPlayer(List<HexModel> region, HexModel playerStartHex)
        {
            var centerHex = SelectStartingHex(region);
            var visited = new HashSet<HexModel>();
            var queue = new Queue<HexModel>();
            queue.Enqueue(playerStartHex);
            visited.Add(playerStartHex);

            int biomeTransitions = 0;
            var currentBiomeType = playerStartHex.BiomeType;
            var currentBiomeIndex = playerStartHex.BiomeIndex;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (current == centerHex) 
                    return biomeTransitions >= MIN_BIOMES_DISTANCE;

                foreach (var neighbor in _hexGridController.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        
                        if (neighbor.BiomeType != currentBiomeType || 
                            neighbor.BiomeIndex != currentBiomeIndex)
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

        private void GenerateResources(List<HexModel> hexesInBiomeInstance, BiomeType biomeType)
        {
            var biome = _rrtAlgorithmModel.Biomes.Find(b => b.BiomeType == biomeType);
            if (biome?.Resources == null) return;

            var availableHexes = hexesInBiomeInstance
                .Where(hex => hex.CurrentBuilding == null && 
                            hex.ResourceDeposit == null)
                .ToList();

            foreach (var resourceInfo in biome.Resources)
            {
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

        private void PlaceNeutralHouses(List<HexModel> hexesInBiomeInstance)
        {
            var availableHexes = hexesInBiomeInstance
                .Where(hex => hex.CurrentBuilding == null && 
                            hex.ResourceDeposit == null)
                .ToList();

            if (availableHexes.Count > 0)
            {
                int randomIndex = Random.Range(0, availableHexes.Count);
                var selectedHex = availableHexes[randomIndex];
                _buildingsController.SpawnBuilding(BuildingType.House, selectedHex, TeamOwner.Neutral);
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

        private List<HexModel> SelectEnemyBaseRegion(List<List<HexModel>> validRegions, HexModel playerStartHex)
        {
            return validRegions
                .OrderByDescending(r => CalculateRegionScore(r, playerStartHex))
                .First();
        }

        private float CalculateRegionScore(List<HexModel> region, HexModel playerStartHex)
        {
            var centerHex = SelectStartingHex(region);
            float distance = Vector3.Distance(centerHex.HexPosition, playerStartHex.HexPosition);
            return distance * region.Count;
        }
    }
}