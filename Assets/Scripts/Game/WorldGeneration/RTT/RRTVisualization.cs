using System.Collections.Generic;
using Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts;
using Game.WorldGeneration.Voronoi;
using UnityEngine;

namespace Game.WorldGeneration.RTT
{
    public class RRTVisualization : MonoBehaviour
    {
        [SerializeField] private Vector3 _centerPoint;
        [SerializeField] private float _stepSize;
        [SerializeField] private float _radius;
        [SerializeField] private float _minDistance;
        [SerializeField] private int _iterations;
        
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _edgePrefab;
        
        [SerializeField] private int _seed;
        [SerializeField] private int _textureResolution;
        [SerializeField] private Material _voronoiMaterial;

        private List<GameObject> _visualObjects = new();
        [SerializeField] private List<Biome> _biomes;

        [SerializeField] private int _voronoiRelaxationIterations;

        private void Awake()
        {
            InitializeBiomes();
        }

        private void Start()
        {
            GenerateAndVisualizeRRTWithBiomes();
        }
        
        private void InitializeBiomes()
        {
            _biomes = new List<Biome>
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
            
            Texture2D voronoiTexture = VoronoiTextureGenerator.GenerateVoronoiTexture(_textureResolution,
                _textureResolution, _seed, _biomes, _voronoiRelaxationIterations);
            _voronoiMaterial.mainTexture = voronoiTexture;
            
            CreateDisplayQuad(voronoiTexture);
            
            List<RRTAlgorithmGenerator.Node> nodes = RRTAlgorithmGenerator.GenerateRRT(_centerPoint, _radius, _stepSize,
                _minDistance, _iterations);
            List<BiomeCell> biomeCells = VoronoiBiomeDistributor.GenerateVoronoiBiomes(_textureResolution,
                _textureResolution, _seed, _biomes, _voronoiRelaxationIterations);
            
            VisualizeRRTWithBiomes(nodes, biomeCells);
        }
        
        private void CreateDisplayQuad(Texture2D texture)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(transform);
            quad.transform.localScale = new Vector3(_radius * 2, _radius * 2, 1);
            quad.transform.position = new Vector3(_centerPoint.x, _centerPoint.y - 0.01f, _centerPoint.z);
            quad.transform.rotation = Quaternion.Euler(90, 0, 0);
            quad.GetComponent<Renderer>().material = _voronoiMaterial;
            _visualObjects.Add(quad);
        }
        
        private void VisualizeRRTWithBiomes(List<RRTAlgorithmGenerator.Node> nodes, List<BiomeCell> biomeCells)
        {
            foreach (var node in nodes)
            {
                GameObject nodeObj = Instantiate(_nodePrefab, node.position, Quaternion.identity);
                nodeObj.transform.SetParent(transform);
                _visualObjects.Add(nodeObj);

                Vector2 position2D = new Vector2(node.position.x, node.position.z);
                
                Vector2 scaledPosition = new Vector2(
                    (position2D.x - (_centerPoint.x - _radius)) / (_radius * 2) * _textureResolution,
                    (position2D.y - (_centerPoint.z - _radius)) / (_radius * 2) * _textureResolution
                );

                Biome biome = VoronoiBiomeDistributor.GetBiomeForPoint(scaledPosition, biomeCells);
                Color nodeColor = biome.Color;
                
                Renderer nodeRenderer = nodeObj.GetComponent<Renderer>();
                nodeRenderer.material.color = nodeColor;

                if (node.parentIndex != -1)
                {
                    Vector3 parentPos = nodes[node.parentIndex].position;
                    GameObject edgeObj = Instantiate(_edgePrefab, Vector3.zero, Quaternion.identity);
                    edgeObj.transform.SetParent(transform);
                    LineRenderer lineRenderer = edgeObj.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, parentPos);
                    lineRenderer.SetPosition(1, node.position);
                    lineRenderer.startColor = nodeColor;
                    lineRenderer.endColor = nodeColor;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    _visualObjects.Add(edgeObj);
                }
            }

            Debug.Log($"Generated {nodes.Count} nodes in {_biomes.Count} biomes");
        }

        private void ClearVisualization()
        {
            foreach (GameObject obj in _visualObjects)
            {
                Destroy(obj);
            }
            _visualObjects.Clear();
        }
    }
}