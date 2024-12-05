using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;

namespace Game.WorldGeneration.Nodes
{
    public class NodeModel: MonoBehaviour
    {
        public Vector3 Position { get; set; }
        public int ParentIndex { get; set; }
        public Color NodeColor { get; set; }
        
        public BiomeType BiomeType { get; set; }
        
        public int BiomeIndex { get; set; }
        
        public Renderer NodeRenderer { get; set; }

        private void OnEnable()
        {
            NodeRenderer = GetComponent<Renderer>();
        }
    }
}