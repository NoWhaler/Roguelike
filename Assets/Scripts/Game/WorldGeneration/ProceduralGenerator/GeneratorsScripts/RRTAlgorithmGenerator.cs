using System.Collections.Generic;
using System.Linq;
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

        public static List<Node> GenerateRRT(Vector3 centerPoint, float radius, float stepSize, List<Biome> biomes)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(centerPoint, -1, 0));
            int[] biomeNodeCounts = new int[biomes.Count];
            biomeNodeCounts[0] = 1;
            float minBiomeDistance = stepSize * 2f;
            int maxAttempts = 50; 

            while (biomeNodeCounts.Select((count, index) => count < biomes[index].nodeCount).Any(x => x))
            {
                int currentBiomeIndex = GetNextBiomeIndex(biomeNodeCounts, biomes);
                
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    Vector3 randomPoint = GetRandomPoint(centerPoint, radius);
                    int nearestIndex = GetNearestNodeIndex(nodes, randomPoint, currentBiomeIndex);

                    if (nearestIndex == -1) continue;

                    Vector3 newPoint = ExtendTowards(nodes[nearestIndex].position, randomPoint, stepSize);

                    if (IsValidNodePlacement(nodes, newPoint, stepSize * 0.5f, currentBiomeIndex, minBiomeDistance))
                    {
                        nodes.Add(new Node(newPoint, nearestIndex, currentBiomeIndex));
                        biomeNodeCounts[currentBiomeIndex]++;

                        if (currentBiomeIndex != 0 && biomeNodeCounts[currentBiomeIndex] < biomes[currentBiomeIndex].nodeCount)
                        {
                            Vector3 extendedPoint = ExtendTowards(newPoint, GetRandomPoint(centerPoint, radius), stepSize);
                            if (IsValidNodePlacement(nodes, extendedPoint, stepSize * 0.5f, currentBiomeIndex, minBiomeDistance))
                            {
                                nodes.Add(new Node(extendedPoint, nodes.Count - 1, currentBiomeIndex));
                                biomeNodeCounts[currentBiomeIndex]++;
                            }
                        }

                        break;
                    }
                }
            }

            return nodes;
        }

        private static bool IsValidNodePlacement(List<Node> nodes, Vector3 newPoint, float minDistance, int currentBiomeIndex, float minBiomeDistance)
        {
            foreach (var node in nodes)
            {
                float distance = Vector3.Distance(node.position, newPoint);
                if (distance < minDistance)
                {
                    return false;
                }
                if (node.biomeIndex != currentBiomeIndex && node.biomeIndex != 0 && currentBiomeIndex != 0 && distance < minBiomeDistance)
                {
                    return false;
                }
            }
            return true;
        }

        private static int GetNextBiomeIndex(int[] biomeNodeCounts, List<Biome> biomes)
        {
            for (int i = 0; i < biomeNodeCounts.Length; i++)
            {
                if (biomeNodeCounts[i] < biomes[i].nodeCount)
                {
                    return i;
                }
            }

            return 0;
        }

        private static Vector3 GetRandomPoint(Vector3 centerPoint, float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return new Vector3(centerPoint.x + randomCircle.x, centerPoint.y + randomCircle.y, centerPoint.z);
        }

        private static int GetNearestNodeIndex(List<Node> nodes, Vector3 point, int targetBiomeIndex)
        {
            float minDistance = float.MaxValue;
            int nearestIndex = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (targetBiomeIndex == 0 || nodes[i].biomeIndex == 0 || nodes[i].biomeIndex == targetBiomeIndex)
                {
                    float distance = Vector3.Distance(nodes[i].position, point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestIndex = i;
                    }
                }
            }

            return nearestIndex;
        }

        private static Vector3 ExtendTowards(Vector3 from, Vector3 to, float stepSize)
        {
            Vector3 direction = (to - from).normalized;
            return from + direction * stepSize;
        }

        private static bool IsCollision(List<Node> nodes, Vector3 newPoint, float minDistance)
        {
            foreach (var node in nodes)
            {
                if (Vector3.Distance(node.position, newPoint) < minDistance)
                {
                    return true;
                }
            }
            return false;
        }
    }
}