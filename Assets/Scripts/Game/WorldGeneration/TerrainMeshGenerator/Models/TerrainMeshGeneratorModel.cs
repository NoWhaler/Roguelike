using UnityEngine;

namespace Game.WorldGeneration.TerrainMeshGenerator.Models
{
    public class TerrainMeshGeneratorModel: MonoBehaviour
    {
        [field: SerializeField] public Material TerrainMaterial { get; set; }
        
        [field: SerializeField] public int ChunkSize { get; set; }
        
        [field: SerializeField] public float HeightMultiplier { get; set; }
        
        [field: SerializeField] public AnimationCurve HeightCurve { get; set; }
        
        [field: SerializeField] public Color32 WaterColor { get; set; } = new (35,137,218, 255);

        [field: SerializeField] public float SmoothFactor { get; set; }
        
        [field: SerializeField] public float Saturation { get; set; }
        
        [field: SerializeField] public float MaxDistance { get; set; }
        
        [field: SerializeField] public float TransitionDistance { get; set; }
        
        [field: SerializeField] public int NearestNodesAmount { get; set; }
        
        [field: SerializeField] public float OutlineTransitionWidth { get; set; }
        
        [field: SerializeField] public float InternalTransitionWidth { get; set; }
        [field: SerializeField] public float BiomeBlendSharpness { get; set; } 
    }
}