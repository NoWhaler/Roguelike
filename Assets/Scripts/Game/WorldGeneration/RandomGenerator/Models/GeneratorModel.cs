using System;
using Game.WorldGeneration.ChunkGeneration.Model;
using UnityEngine;

namespace Game.WorldGeneration.RandomGenerator.Models
{
    public class GeneratorModel: MonoBehaviour
    {
        [field: Range(1, 255)]
        [field: SerializeField] public int ChunkSize { get; set; }
        
        [field: Range(1, 16)]
        [field: SerializeField] public int ChunksPerSide { get; set; }
        
        [field: Range(0, 50f)]
        [field: SerializeField] public float NoiseScale { get; set; }
        
        [field: Range(0, 7f)]
        [field: SerializeField] public int Octaves { get; set; }
        
        [field: Range(0, 1f)]
        [field: SerializeField] public float Persistence { get; set; }
        
        [field: Range(0, 10f)]
        [field: SerializeField] public float Lacunarity { get; set; }
        
        [field: Range(0, 30f)]
        [field: SerializeField] public float HeightMultiplier { get; set; } 
        
        [field: Range(0, 30)]
        [field: SerializeField] public int CoastlineSmoothPasses { get; set; }
        
        [field: Range(2, 20)]
        [field: SerializeField] public int SmoothNormalsPasses { get; set; }
        
        [field: Range(1, 255)]
        [field: SerializeField] public int SeedValue { get; set; }
        
        [field: SerializeField] public AnimationCurve HeightCurve { get; set; }
        
        [field: SerializeField] public TerrainType[] Regions { get; set; }
        
        [field: SerializeField] public bool UseFalloffMap { get; set; } = true;
        
        [field: SerializeField] public Material ChunkMaterial { get; set; }


        [field: SerializeField] public ChunkModel ChunkPrefab { get; set; }

        public Transform GenerationModelTransform => transform;
        
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

