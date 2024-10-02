using Game.WorldGeneration.Nodes;
using UnityEngine;

namespace Game.WorldGeneration.RTT.Models
{
    public class RRTAlgorithModel : MonoBehaviour
    {
        [field: SerializeField] public Vector3 CenterPoint { get; set; }
        [field: SerializeField] public float StepSize { get; set; }
        [field: SerializeField] public float Radius { get; set; }
        [field: SerializeField] public float MinDistance { get; set; }
        [field: SerializeField] public int Iterations { get; set; }
        
        [field: SerializeField] public NodeModel NodePrefab { get; set; }
        [field: SerializeField] public GameObject EdgePrefab { get; set; }
        
        [field: SerializeField] public int Seed { get; set; }
        [field: SerializeField] public int TextureResolution { get; set; }
        [field: SerializeField] public Material VoronoiMaterial { get; set; }

        [field: SerializeField] public int VoronoiRelaxationIterations { get; set; }
        
        [field: SerializeField] public int ChunksPerSide { get; set; }

        public void DestroyVisualObject(GameObject visualGameObject)
        {
            Destroy(visualGameObject);
        }
    }
}