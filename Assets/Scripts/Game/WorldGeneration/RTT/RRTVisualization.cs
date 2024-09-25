using System.Collections.Generic;
using Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts;
using UnityEngine;

namespace Game.WorldGeneration.RTT
{
    [System.Serializable]
    public class Biome
    {
        public Color color;
        public int nodeCount;

        public Biome(Color c, int count)
        {
            color = c;
            nodeCount = count;
        }
    }

    public class RRTVisualization : MonoBehaviour
    {
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 goalPoint;
        [SerializeField] private float stepSize;
        [SerializeField] private int maxIterations;
        [SerializeField] private float goalThreshold;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject edgePrefab;
        [SerializeField] private Color startColor = Color.green;
        [SerializeField] private Color goalColor = Color.red;
        [SerializeField] private List<Biome> biomes = new List<Biome>();
        
        private List<GameObject> visualObjects = new List<GameObject>();

        void Start()
        {
            GenerateAndVisualizeRRT();
        }

        public void GenerateAndVisualizeRRT()
        {
            ClearVisualization();
            List<RRTAlgorithmGenerator.Node> nodes = RRTAlgorithmGenerator.GenerateRRT(startPoint, goalPoint, stepSize, maxIterations, goalThreshold, biomes);
            VisualizeRRT(nodes);
        }

        private void VisualizeRRT(List<RRTAlgorithmGenerator.Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                RRTAlgorithmGenerator.Node node = nodes[i];
                GameObject nodeObj = Instantiate(nodePrefab, node.position, Quaternion.identity);
                nodeObj.transform.SetParent(transform);
                visualObjects.Add(nodeObj);

                Color nodeColor;
                if (i == 0)
                {
                    nodeColor = startColor;
                }
                else if (i == nodes.Count - 1)
                {
                    nodeColor = goalColor;
                }
                else
                {
                    nodeColor = biomes[node.biomeIndex].color;
                }
                nodeObj.GetComponent<Renderer>().material.color = nodeColor;

                if (node.parentIndex != -1)
                {
                    Vector3 parentPos = nodes[node.parentIndex].position;
                    GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
                    edgeObj.transform.SetParent(transform);
                    LineRenderer lineRenderer = edgeObj.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, parentPos);
                    lineRenderer.SetPosition(1, node.position);
                    lineRenderer.startColor = i == nodes.Count - 1 ? biomes[nodes[node.parentIndex].biomeIndex].color : nodeColor;
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
    }
}