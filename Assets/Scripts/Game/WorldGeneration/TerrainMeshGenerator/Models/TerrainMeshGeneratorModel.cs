using UnityEngine;

namespace Game.WorldGeneration.TerrainMeshGenerator.Models
{
    public class TerrainMeshGeneratorModel: MonoBehaviour
    {
        [field: SerializeField] public Material TerrainMaterial { get; set; }
        
        [field: SerializeField] public int ChunkSize { get; set; }
        
        [field: SerializeField] public float HeightMultiplier { get; set; }
        
        [field: SerializeField] public AnimationCurve HeightCurve { get; set; }
    }
}