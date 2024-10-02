using UnityEngine;

namespace Game.WorldGeneration.Nodes
{
    public struct Node
    {
        public Vector3 Position { get; set; }
        public int ParentIndex { get; set; }

        public Node(Vector3 pos, int parent)
        {
            Position = pos;
            ParentIndex = parent;
        }
    }
}