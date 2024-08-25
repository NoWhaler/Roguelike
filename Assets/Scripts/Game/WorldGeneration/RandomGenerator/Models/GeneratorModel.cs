using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.WorldGeneration.RandomGenerator.Models
{
    public class GeneratorModel: MonoBehaviour
    {
        [field: SerializeField] public int MapWidth { get; set; } = 100;
        [field: SerializeField] public int MapHeight { get; set; } = 100;
        
        [field: Range(0, 50f)]
        [field: SerializeField] public float NoiseScale { get; set; } = 0.3f;
        [field: Range(0, 7f)]
        [field: SerializeField] public int Octaves { get; set; } = 4;
        [field: Range(0, 1f)]
        [field: SerializeField] public float Persistence { get; set; } = 0.5f;
        [field: Range(0, 10f)]
        [field: SerializeField] public float Lacunarity { get; set; } = 2f;
        [field: Range(1, 20f)]
        [field: SerializeField] public float HeightMultiplier { get; set; } = 10f; 
        [field: SerializeField] public AnimationCurve HeightCurve { get; set; }
        [field: SerializeField] public TerrainType[] Regions { get; set; }
        
        [field: SerializeField] public bool UseFalloffMap { get; set; } = true;
        public bool isValidateAvailable;
        
        [field: SerializeField] public MeshFilter MapMeshFilter;

        [field: SerializeField]  public MeshCollider MapMeshCollider;

        public Action OnGenerateMap;
    }
    
    [Serializable]
    public struct TerrainType
    {
        public string name;
        [Range(0, 1)]
        public float height;
        public Color color;
    }
}

