using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class RRTAlgorithmGenerator
    {
        public struct Node
        {
            public Vector3 position;
            public int parentIndex;

            public Node(Vector3 pos, int parent)
            {
                position = pos;
                parentIndex = parent;
            }
        }

        public static List<Node> GenerateRRT(Vector3 startPoint, float radius, float stepSize, float minDistance, int desiredNodeCount)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(startPoint, -1));

            while (nodes.Count < desiredNodeCount)
            {
                Vector3 randomPoint = GetRandomPointWithinRadius(startPoint, radius);

                Node nearestNode = FindNearestNode(nodes, randomPoint);

                Vector3 direction = (randomPoint - nearestNode.position).normalized;
                Vector3 newPosition = nearestNode.position + direction * stepSize;

                if (IsFarEnoughFromExistingNodes(nodes, newPosition, minDistance))
                {
                    nodes.Add(new Node(newPosition, nodes.IndexOf(nearestNode)));
                }
            }

            return nodes;
        }

        private static bool IsFarEnoughFromExistingNodes(List<Node> nodes, Vector3 newPosition, float minDistance)
        {
            foreach (Node node in nodes)
            {
                if (Vector3.Distance(newPosition, node.position) < minDistance)
                {
                    return false;
                }
            }
            return true;
        }

        private static Node FindNearestNode(List<Node> nodes, Vector3 point)
        {
            Node nearestNode = nodes[0];
            float minDistance = Vector3.Distance(point, nearestNode.position);

            for (int i = 1; i < nodes.Count; i++)
            {
                float distance = Vector3.Distance(point, nodes[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = nodes[i];
                }
            }

            return nearestNode;
        }

        private static Vector3 GetRandomPointWithinRadius(Vector3 center, float radius)
        {
            Vector2 randomPoint2D = Random.insideUnitCircle * radius;
            return new Vector3(randomPoint2D.x, 0, randomPoint2D.y) + center;
        }
    }
}
