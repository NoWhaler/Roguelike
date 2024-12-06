using System.Collections.Generic;
using System.Linq;
using Game.ProductionResources.Enum;
using Game.WorldGeneration.Biomes;
using Game.WorldGeneration.Biomes.Enum;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.RTT.Models;
using Game.WorldGeneration.TerrainMeshGenerator.Controllers;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using Game.WorldGeneration.Voronoi;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RTT.Controllers
{
    public class RRTAlgorithmController: IInitializable
    {
        private DiContainer _diContainer;
        
        private HexagonalTerrainMeshGeneratorController _hexagonalTerrainMeshGenerator;

        private RRTAlgorithmModel _rrtAlgorithmModel;

        private VoronoiBiomeDistributor _voronoiBiomeDistributor;

        private VoronoiTextureGenerator _voronoiTextureGenerator;

        private IslandBoundaryController _islandBoundary;
        
        private readonly List<NodeModel> _nodeModels = new List<NodeModel>();
        
        private List<GameObject> _visualObjects = new();
        
        [Inject]
        private void Constructor(DiContainer diContainer,
            RRTAlgorithmModel rrtAlgorithmModel, VoronoiBiomeDistributor voronoiBiomeDistributor,
            VoronoiTextureGenerator voronoiTextureGenerator,
            HexagonalTerrainMeshGeneratorController hexagonalTerrainMeshGeneratorController)
        {
            _diContainer = diContainer;
            _rrtAlgorithmModel = rrtAlgorithmModel;
            _voronoiBiomeDistributor = voronoiBiomeDistributor;
            _voronoiTextureGenerator = voronoiTextureGenerator;
            _hexagonalTerrainMeshGenerator = hexagonalTerrainMeshGeneratorController;
            
            _islandBoundary = new IslandBoundaryController(
                    rrtAlgorithmModel.CenterPoint,
                    rrtAlgorithmModel.Radius * 0.8f
                );
        }
        
        public void Initialize()
        {
            InitializeBiomes();
            GenerateAndVisualizeRRTWithBiomes();
        }
        
        private void InitializeBiomes()
        {
            int currentIndex = 0;
            var biomes = new List<Biome>();

            for (int i = 0; i < 3; i++)
            {
                var desert = new Biome("Desert", BiomeType.Desert, new Color(1f, 0.84f, 0.4f), currentIndex);
                desert.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Stone, 1), 
                    new BiomeResourceInfo(ResourceType.Food, 1)    
                });
                biomes.Add(desert);
                currentIndex++;
            }

            for (int i = 0; i < 3; i++)
            {
                var grassland = new Biome("Grassland", BiomeType.Grassland, new Color(0.5f, 0.8f, 0.3f), currentIndex);
                grassland.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Food, 1),   
                    new BiomeResourceInfo(ResourceType.Wood, 1)    
                });
                biomes.Add(grassland);
                currentIndex++;
            }

            for (int i = 0; i < 3; i++)
            {
                var forest = new Biome("Forest", BiomeType.Forest, new Color(0.13f, 0.55f, 0.13f), currentIndex);
                forest.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Wood, 1),
                    new BiomeResourceInfo(ResourceType.Food, 1)
                });
                biomes.Add(forest);
                currentIndex++;
            }

            for (int i = 0; i < 2; i++)
            {
                var tundra = new Biome("Tundra", BiomeType.Tundra, new Color(0.8f, 0.9f, 0.95f), currentIndex);
                tundra.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Stone, 1),
                    new BiomeResourceInfo(ResourceType.Food, 1)
                });
                biomes.Add(tundra);
                currentIndex++;
            }

            for (int i = 0; i < 2; i++)
            {
                var swamp = new Biome("Swamp", BiomeType.Swamp, new Color(0.4f, 0.3f, 0.2f), currentIndex);
                swamp.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Wood, 1),
                    new BiomeResourceInfo(ResourceType.Food, 1)
                });
                biomes.Add(swamp);
                currentIndex++;
            }

            for (int i = 0; i < 2; i++)
            {
                var savanna = new Biome("Savanna", BiomeType.Savanna, new Color(0.96f, 0.64f, 0.38f), currentIndex);
                savanna.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Food, 1),
                    new BiomeResourceInfo(ResourceType.Wood, 1)
                });
                biomes.Add(savanna);
                currentIndex++;
            }

            for (int i = 0; i < 2; i++)
            {
                var jungle = new Biome("Jungle", BiomeType.Jungle, new Color(0f, 0.4f, 0f), currentIndex);
                jungle.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Wood, 1),
                    new BiomeResourceInfo(ResourceType.Food, 1)
                });
                biomes.Add(jungle);
                currentIndex++;
            }

            for (int i = 0; i < 2; i++)
            {
                var mountain = new Biome("Mountain", BiomeType.Mountain, new Color(0.5f, 0.5f, 0.5f), currentIndex);
                mountain.SetResourceInfo(new List<BiomeResourceInfo> 
                {
                    new BiomeResourceInfo(ResourceType.Stone, 1),
                    new BiomeResourceInfo(ResourceType.Food, 1) 
                });
                biomes.Add(mountain);
                currentIndex++;
            }

            _rrtAlgorithmModel.Biomes = biomes;
        }

        private void GenerateAndVisualizeRRTWithBiomes()
        {
            ClearVisualization();
            
            Texture2D voronoiTexture = _voronoiTextureGenerator.GenerateVoronoiTexture(_rrtAlgorithmModel.TextureResolution,
                _rrtAlgorithmModel.TextureResolution, _rrtAlgorithmModel.Seed, _rrtAlgorithmModel.Biomes, _rrtAlgorithmModel.VoronoiRelaxationIterations);
            _rrtAlgorithmModel.VoronoiMaterial.mainTexture = voronoiTexture;

            if (_rrtAlgorithmModel.IsDisplayingVoronoi)
            {
                CreateDisplayQuad();
            }

            List<Node> nodes = GenerateRRT(_rrtAlgorithmModel.CenterPoint, _rrtAlgorithmModel.Radius, _rrtAlgorithmModel.StepSize,
                _rrtAlgorithmModel.MinDistance, _rrtAlgorithmModel.Iterations);
            
            _hexagonalTerrainMeshGenerator.GenerateChunks(_nodeModels, _rrtAlgorithmModel.ChunksPerSide);

            if (_rrtAlgorithmModel.IsDisplaingRRT) return;
            
            foreach (var visualObject in _visualObjects)
            {
                visualObject.SetActive(false);
            }
        }
        
        private void CreateDisplayQuad()
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(_rrtAlgorithmModel.transform);
            quad.transform.localScale = new Vector3(_rrtAlgorithmModel.Radius * 2, _rrtAlgorithmModel.Radius * 2, 1);
            quad.transform.position = new Vector3(_rrtAlgorithmModel.CenterPoint.x, _rrtAlgorithmModel.CenterPoint.y - 0.01f, _rrtAlgorithmModel.CenterPoint.z);
            quad.transform.rotation = Quaternion.Euler(90, 0, 0);
            quad.GetComponent<Renderer>().material = _rrtAlgorithmModel.VoronoiMaterial;
            _visualObjects.Add(quad);
        }
        
        private void VisualizeRRTWithBiomes(List<Node> nodes, List<BiomeCell> biomeCells)
        {
            foreach (var node in nodes)
            {
                NodeModel nodeObj = _diContainer.InstantiatePrefabForComponent<NodeModel>
                    (_rrtAlgorithmModel.NodePrefab, node.Position, Quaternion.identity, _rrtAlgorithmModel.transform);
                _visualObjects.Add(nodeObj.gameObject);

                nodeObj.Position = node.Position;
                nodeObj.ParentIndex = node.ParentIndex;

                Vector2 position2D = new Vector2(nodeObj.Position.x, nodeObj.Position.z);
                
                Vector2 scaledPosition = new Vector2(
                    (position2D.x - (_rrtAlgorithmModel.CenterPoint.x - _rrtAlgorithmModel.Radius)) / (_rrtAlgorithmModel.Radius * 2) * _rrtAlgorithmModel.TextureResolution,
                    (position2D.y - (_rrtAlgorithmModel.CenterPoint.z - _rrtAlgorithmModel.Radius)) / (_rrtAlgorithmModel.Radius * 2) * _rrtAlgorithmModel.TextureResolution
                );

                Biome biome = _voronoiBiomeDistributor.GetBiomeForPoint(scaledPosition, biomeCells);
                Color nodeColor = biome.Color;
                
                nodeObj.NodeRenderer.material.color = nodeColor;
                nodeObj.NodeColor = nodeColor;
                nodeObj.BiomeType = biome.BiomeType;
                nodeObj.BiomeIndex = biome.BiomeIndex;

                _nodeModels.Add(nodeObj);

                if (nodeObj.ParentIndex != -1)
                {
                    Vector3 parentPos = nodes[nodeObj.ParentIndex].Position;
                    GameObject edgeObj = _diContainer.InstantiatePrefab
                        (_rrtAlgorithmModel.EdgePrefab, Vector3.zero, Quaternion.identity, _rrtAlgorithmModel.transform);
                    LineRenderer lineRenderer = edgeObj.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, parentPos);
                    lineRenderer.SetPosition(1, nodeObj.Position);
                    lineRenderer.startColor = nodeColor;
                    lineRenderer.endColor = nodeColor;
                    lineRenderer.startWidth = 0.2f;
                    lineRenderer.endWidth = 0.2f;
                    _visualObjects.Add(edgeObj);
                }
            }

            Debug.Log($"Generated {nodes.Count} nodes in {_rrtAlgorithmModel.Biomes.Count} biomes");
        }
        
        private void ClearVisualization()
        {
            foreach (GameObject obj in _visualObjects)
            {
                _rrtAlgorithmModel.DestroyVisualObject(obj);
            }
            _visualObjects.Clear();
        }
        
        private List<Node> GenerateRRT(Vector3 startPoint, float radius, float stepSize, float minDistance, int desiredNodeCount)
        {
            List<Node> nodes = new List<Node>();
            List<BiomeCell> biomeCells = _voronoiBiomeDistributor.GenerateVoronoiBiomes(
                _rrtAlgorithmModel.TextureResolution,
                _rrtAlgorithmModel.TextureResolution,
                _rrtAlgorithmModel.Seed,
                _rrtAlgorithmModel.Biomes,
                _rrtAlgorithmModel.VoronoiRelaxationIterations
            );

            HashSet<int> coveredBiomeCells = new HashSet<int>();
            
            nodes.Add(new Node(startPoint, -1));

            List<Vector3> targetPositions = new List<Vector3>();
            Dictionary<Vector3, int> targetToBiomeIndex = new Dictionary<Vector3, int>();
            
            for (int i = 0; i < biomeCells.Count; i++)
            {
                var cell = biomeCells[i];
                Vector2 worldPos = new Vector2(
                    (cell.SeedPoint.x / _rrtAlgorithmModel.TextureResolution) * (radius * 2) + (startPoint.x - radius),
                    (cell.SeedPoint.y / _rrtAlgorithmModel.TextureResolution) * (radius * 2) + (startPoint.z - radius)
                );
                Vector3 targetPos = new Vector3(worldPos.x, 0, worldPos.y);
                
                if (_islandBoundary.IsWithinBoundary(targetPos))
                {
                    targetPositions.Add(targetPos);
                    targetToBiomeIndex[targetPos] = i;
                }
            }

            int failedAttempts = 0;
            int maxFailedAttempts = 1000;
            float branchProbability = 0.8f;

            while (nodes.Count < desiredNodeCount && failedAttempts < maxFailedAttempts)
            {
                bool growTowardsTarget = Random.value < 0.9f;
                Vector3 targetPoint;
                int? targetBiomeIndex = null;

                if (growTowardsTarget && targetPositions.Count > 0)
                {
                    int randomIndex = Random.Range(0, targetPositions.Count);
                    targetPoint = targetPositions[randomIndex];
                    if (targetToBiomeIndex.ContainsKey(targetPoint))
                    {
                        targetBiomeIndex = targetToBiomeIndex[targetPoint];
                    }
                }
                else
                {
                    targetPoint = GetRandomPointWithinRadius(startPoint, radius);
                }

                Node nearestNode = FindNearestNode(nodes, targetPoint);
                
                if (Random.value < branchProbability)
                {
                    nearestNode = nodes[0];
                }

                Vector3 direction = (targetPoint - nearestNode.Position).normalized;
                Vector3 newPosition = nearestNode.Position + direction * stepSize;

                if (_islandBoundary.IsWithinBoundary(newPosition) && 
                    IsFarEnoughFromExistingNodes(nodes, newPosition, minDistance))
                {
                    nodes.Add(new Node(newPosition, nodes.IndexOf(nearestNode)));
                    
                    for (int i = targetPositions.Count - 1; i >= 0; i--)
                    {
                        if (Vector3.Distance(newPosition, targetPositions[i]) < minDistance * 2)
                        {
                            if (targetToBiomeIndex.ContainsKey(targetPositions[i]))
                            {
                                coveredBiomeCells.Add(targetToBiomeIndex[targetPositions[i]]);
                            }
                            targetPositions.RemoveAt(i);
                        }
                    }
                    
                    failedAttempts = 0;
                }
                else
                {
                    failedAttempts++;
                }
            }

            List<BiomeType> uncoveredBiomes = new List<BiomeType>();
            for (int i = 0; i < biomeCells.Count; i++)
            {
                if (!coveredBiomeCells.Contains(i))
                {
                    uncoveredBiomes.Add(biomeCells[i].Biome.BiomeType);
                }
            }

            Debug.Log($"Generated {nodes.Count} nodes, covered {coveredBiomeCells.Count}/{biomeCells.Count} biome cells");
            if (uncoveredBiomes.Count > 0)
            {
                Debug.LogWarning($"Uncovered biomes: {string.Join(", ", uncoveredBiomes)}");
            }

            VisualizeRRTWithBiomes(nodes, biomeCells);
            
            return nodes;
        }

        private bool IsFarEnoughFromExistingNodes(List<Node> nodes, Vector3 newPosition, float minDistance)
        {
            foreach (Node node in nodes)
            {
                if (Vector3.Distance(newPosition, node.Position) < minDistance)
                {
                    return false;
                }
            }
            return true;
        }

        private Node FindNearestNode(List<Node> nodes, Vector3 point)
        {
            Node nearestNode = nodes[0];
            float minDistance = Vector3.Distance(point, nearestNode.Position);

            for (int i = 1; i < nodes.Count; i++)
            {
                float distance = Vector3.Distance(point, nodes[i].Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = nodes[i];
                }
            }

            return nearestNode;
        }

        private Vector3 GetRandomPointWithinRadius(Vector3 center, float radius)
        {
            Vector3 point;
            int maxAttempts = 100;
            int attempts = 0;
            
            do
            {
                Vector2 randomPoint2D = Random.insideUnitCircle * radius;
                point = new Vector3(randomPoint2D.x, 0, randomPoint2D.y) + center;
                attempts++;
            } while (!_islandBoundary.IsWithinBoundary(point) && attempts < maxAttempts);
            
            return point;
        }
    }
}
