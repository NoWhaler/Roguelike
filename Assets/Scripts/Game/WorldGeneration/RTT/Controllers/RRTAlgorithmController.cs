using System.Collections.Generic;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.RTT.Models;
using Game.WorldGeneration.Voronoi;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RTT.Controllers
{
    public class RRTAlgorithmController: IInitializable
    {
        private DiContainer _diContainer;
        
        private TerrainMeshGeneratorController _terrainMeshGeneratorController;

        private RRTAlgorithModel _rrtAlgorithmModel;

        private VoronoiBiomeDistributor _voronoiBiomeDistributor;

        private VoronoiTextureGenerator _voronoiTextureGenerator;

        private readonly List<NodeModel> _nodeModels = new List<NodeModel>();
        
        private List<GameObject> _visualObjects = new();
        
        [Inject]
        private void Constructor(DiContainer diContainer, 
            TerrainMeshGeneratorController terrainMeshGeneratorController,
            RRTAlgorithModel rrtAlgorithModel, VoronoiBiomeDistributor voronoiBiomeDistributor, VoronoiTextureGenerator voronoiTextureGenerator)
        {
            _diContainer = diContainer;
            _terrainMeshGeneratorController = terrainMeshGeneratorController;
            _rrtAlgorithmModel = rrtAlgorithModel;
            _voronoiBiomeDistributor = voronoiBiomeDistributor;
            _voronoiTextureGenerator = voronoiTextureGenerator;
        }
        
        public void Initialize()
        {
            InitializeBiomes();
            GenerateAndVisualizeRRTWithBiomes();
        }
        
        private void InitializeBiomes()
        {
            _rrtAlgorithmModel.Biomes = new List<Biome>
            {
                new Biome("Desert", new Color(1f, 0.84f, 0.4f), 3),
                new Biome("Grassland", new Color(0.5f, 0.8f, 0.3f), 3),
                new Biome("Forest", new Color(0.13f, 0.55f, 0.13f), 3),
                new Biome("Tundra", new Color(0.8f, 0.9f, 0.95f), 2),
                new Biome("Swamp", new Color(0.4f, 0.3f, 0.2f), 2),
                new Biome("Savanna", new Color(0.96f, 0.64f, 0.38f), 2),
                new Biome("Jungle", new Color(0f, 0.4f, 0f), 2),
                new Biome("Mountain", new Color(0.5f, 0.5f, 0.5f), 2)
            };
        }

        private void GenerateAndVisualizeRRTWithBiomes()
        {
            ClearVisualization();
            
            Texture2D voronoiTexture = _voronoiTextureGenerator.GenerateVoronoiTexture(_rrtAlgorithmModel.TextureResolution,
                _rrtAlgorithmModel.TextureResolution, _rrtAlgorithmModel.Seed, _rrtAlgorithmModel.Biomes, _rrtAlgorithmModel.VoronoiRelaxationIterations);
            _rrtAlgorithmModel.VoronoiMaterial.mainTexture = voronoiTexture;
            
            // CreateDisplayQuad();
            
            List<Node> nodes = GenerateRRT(_rrtAlgorithmModel.CenterPoint, _rrtAlgorithmModel.Radius, _rrtAlgorithmModel.StepSize,
                _rrtAlgorithmModel.MinDistance, _rrtAlgorithmModel.Iterations);
            List<BiomeCell> biomeCells = _voronoiBiomeDistributor.GenerateVoronoiBiomes(_rrtAlgorithmModel.TextureResolution,
                _rrtAlgorithmModel.TextureResolution, _rrtAlgorithmModel.Seed, _rrtAlgorithmModel.Biomes, _rrtAlgorithmModel.VoronoiRelaxationIterations);
            
            VisualizeRRTWithBiomes(nodes, biomeCells);
            
            _terrainMeshGeneratorController.GenerateChunks(_nodeModels, _rrtAlgorithmModel.CenterPoint, _rrtAlgorithmModel.ChunksPerSide);
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
            nodes.Add(new Node(startPoint, -1));

            while (nodes.Count < desiredNodeCount)
            {
                Vector3 randomPoint = GetRandomPointWithinRadius(startPoint, radius);

                Node nearestNode = FindNearestNode(nodes, randomPoint);

                Vector3 direction = (randomPoint - nearestNode.Position).normalized;
                Vector3 newPosition = nearestNode.Position + direction * stepSize;

                if (IsFarEnoughFromExistingNodes(nodes, newPosition, minDistance))
                {
                    nodes.Add(new Node(newPosition, nodes.IndexOf(nearestNode)));
                }
            }

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
            Vector2 randomPoint2D = Random.insideUnitCircle * radius;
            return new Vector3(randomPoint2D.x, 0, randomPoint2D.y) + center;
        }

    }
}
