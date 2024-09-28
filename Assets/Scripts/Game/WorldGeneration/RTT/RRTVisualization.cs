using System.Collections.Generic;
using System.Linq;
using Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts;
using UnityEngine;

namespace Game.WorldGeneration.RTT
{
    [System.Serializable]
    public class Biome
    {
        public Color color;
        public string name;
        public int nodeCount;

        public Biome(Color c, int count, string n, int nodesCount)
        {
            color = c;
            name = n;
            nodeCount = nodesCount;
        }
    }

    public class RRTVisualization : MonoBehaviour
    {
        [SerializeField] private Vector3 _centerPoint;
        [SerializeField] private float _stepSize;
        
        [Range(0.5f, 3f)]
        [SerializeField] private float _radiusModifier;
        
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _edgePrefab;
        [SerializeField] private Color _startColor;
        [SerializeField] private List<Biome> biomes = new();
        [SerializeField] private bool debugMode = true;
        
        private List<GameObject> visualObjects = new();
        private Dictionary<int, int> biomeNodeCounts = new Dictionary<int, int>();

        private void Start()
        {
            GenerateAndVisualizeRRT();
        }

        private void GenerateAndVisualizeRRT()
        {
            ClearVisualization();
            ResetBiomeNodeCounts();

            int maxIterations = biomes.Sum(biome => biome.nodeCount);
            float radius = _radiusModifier * maxIterations;

            List<RRTAlgorithmGenerator.Node> nodes = RRTAlgorithmGenerator.GenerateRRT(_centerPoint, radius, _stepSize, biomes);
            VisualizeRRT(nodes);
            
            if (debugMode)
            {
                DebugBiomeNodeCounts();
            }
        }

        private void VisualizeRRT(List<RRTAlgorithmGenerator.Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                RRTAlgorithmGenerator.Node node = nodes[i];
                GameObject nodeObj = Instantiate(_nodePrefab, node.position, Quaternion.identity);
                nodeObj.transform.SetParent(transform);
                visualObjects.Add(nodeObj);

                Color nodeColor = (i == 0) ? _startColor : biomes[node.biomeIndex].color;
                nodeObj.GetComponent<Renderer>().material.color = nodeColor;

                IncrementBiomeNodeCount(node.biomeIndex);

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
                    visualObjects.Add(edgeObj);
                }
            }
        }

        private void ClearVisualization()
        {
            foreach (GameObject obj in visualObjects)
            {
                Destroy(obj);
            }
            visualObjects.Clear();
        }
        
        private void ResetBiomeNodeCounts()
        {
            biomeNodeCounts.Clear();
            for (int i = 0; i < biomes.Count; i++)
            {
                biomeNodeCounts[i] = 0;
            }
        }

        private void IncrementBiomeNodeCount(int biomeIndex)
        {
            if (biomeNodeCounts.ContainsKey(biomeIndex))
            {
                biomeNodeCounts[biomeIndex]++;
            }
        }

        private void DebugBiomeNodeCounts()
        {
            Debug.Log("RRT Node Count by Biome:");
            for (int i = 0; i < biomes.Count; i++)
            {
                string biomeType = i == 0 ? "Road" : "Biome";
                Debug.Log($"{biomeType}: {biomes[i].name}, Color: {biomes[i].color}, Nodes: {biomeNodeCounts[i]}");
            }
        }
    }
}