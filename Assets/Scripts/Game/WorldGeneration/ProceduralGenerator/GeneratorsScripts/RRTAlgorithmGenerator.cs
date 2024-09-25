using System.Collections.Generic;
using Game.WorldGeneration.RTT;
using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class RRTAlgorithmGenerator
    {
        public struct Node
        {
            public Vector3 position;
            public int parentIndex;
            public int biomeIndex;
            
            public Node(Vector3 pos, int parent, int biome)
            {
                position = pos;
                parentIndex = parent;
                biomeIndex = biome;
            }
        }

        public static List<Node> GenerateRRT(Vector3 startPoint, Vector3 goalPoint, float stepSize, int maxIterations,
            float goalThreshold, List<Biome> biomes)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(startPoint, -1, -1));

            int currentBiomeIndex = 0;
            int nodesInCurrentBiome = 0;

            for (int i = 0; i < maxIterations; i++)
            {
                Vector3 randomPoint = GetRandomPoint(startPoint, goalPoint);
                int nearestIndex = GetNearestNodeIndex(nodes, randomPoint);
                Vector3 newPoint = ExtendTowards(nodes[nearestIndex].position, randomPoint, stepSize);

                if (!IsCollision(nodes[nearestIndex].position, newPoint))
                {
                    nodes.Add(new Node(newPoint, nearestIndex, currentBiomeIndex));
                    nodesInCurrentBiome++;

                    if (Vector3.Distance(newPoint, goalPoint) < goalThreshold)
                    {
                        nodes.Add(new Node(goalPoint, nodes.Count - 1, currentBiomeIndex));
                        return nodes;
                    }

                    if (nodesInCurrentBiome >= biomes[currentBiomeIndex].nodeCount &&
                        currentBiomeIndex < biomes.Count - 1)
                    {
                        currentBiomeIndex++;
                        nodesInCurrentBiome = 0;
                    }
                }
            }

            return nodes;
        }

        private static Vector3 GetRandomPoint(Vector3 startPoint, Vector3 goalPoint)
        {
            float minX = Mathf.Min(startPoint.x, goalPoint.x);
            float maxX = Mathf.Max(startPoint.x, goalPoint.x);
            float minY = Mathf.Min(startPoint.y, goalPoint.y);
            float maxY = Mathf.Max(startPoint.y, goalPoint.y);
            return new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                startPoint.z
            );
        }

        private static int GetNearestNodeIndex(List<Node> nodes, Vector3 point)
        {
            float minDistance = float.MaxValue;
            int nearestIndex = -1;
            for (int i = 0; i < nodes.Count; i++)
            {
                float distance = Vector3.Distance(nodes[i].position, point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private static Vector3 ExtendTowards(Vector3 from, Vector3 to, float stepSize)
        {
            Vector3 direction = (to - from).normalized;
            return from + direction * Mathf.Min(stepSize, Vector3.Distance(from, to));
        }

        private static bool IsCollision(Vector3 from, Vector3 to)
        {
            return false;
        }
    }
}