using System;
using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.SimplexNoiseGeneration.Models
{
    public class SimplexNoiseGeneratorModel: MonoBehaviour
    {
        [field: Range(1, 512)]
        [field: SerializeField] public int SeedValue { get; set; }
        
        [field: Range(1, 25)]
        [field: SerializeField] public int ChunksPerSide { get; set; }
        
        [field: Range(1, 255)]
        [field: SerializeField] public int ChunkSize { get; set; }
        
        [field: Range(1, 100)]
        [field: SerializeField] public float NoiseScale { get; set; }
        
        [field: Range(1, 9)]
        [field: SerializeField] public int Octaves { get; set; }
        
        [field: Range(0, 1)]
        [field: SerializeField] public float Persistence { get; set; }
        
        [field: Range(1, 3)]
        [field: SerializeField] public float Lacunarity { get; set; }
        
        [field: Range(0, 50)]
        [field: SerializeField] public float HeightMultiplier { get; set; }
        
        [field: Range(1, 25)]
        [field: SerializeField] public int CoastlineSmoothPasses { get; set; }
        
        [field: Range(1, 15)]
        [field: SerializeField] public int SmoothNormalsPasses { get; set; }
        
        [field: Range(1, 100)]
        [field: SerializeField] public float WorleyNoiseScale { get; set; }
        
        [field: Range(0, 1)]
        [field: SerializeField] public float LakeThreshold { get; set; }
        
        [field: Range(1, 50)]
        [field: SerializeField] public int MinLakeSize { get; set; }
        
        [field: Range(1, 100)]
        [field: SerializeField] public float LakeMergeDistance { get; set; }
        
        [field: Range(0, 1)]
        [field: SerializeField] public float DiamondSquareRoughness { get; set; }
        
        [field: Range(0, 1f)]
        [field: SerializeField] public float SimplexWeight { get; set; }
        
        [field: Range(0, 1f)]
        [field: SerializeField] public float WorleyWeight { get; set; }
        
        [field: Range(0, 1f)]
        [field: SerializeField] public float DiamondSquareWeight { get; set; }
        
        [field: Range(0, 1f)]
        [field: SerializeField] public float VoronoiWeight { get; set; }
        
        [field: Range(10, 200)]
        [field: SerializeField] public int VoronoiSitesNumber { get; set; }
        
        [field: Range(0, 5)]
        [field: SerializeField] public float FalloffRadius { get; set; }
        [field: SerializeField] public bool UseFalloffMap { get; set; }
        
        [field: SerializeField]public AnimationCurve HeightCurve { get; set; }
        
        [field: SerializeField] public Region[] Regions { get; set; }
        
        [field: SerializeField] public GameObject ChunkPrefab { get; set; }
        
        [field: SerializeField] public Material ChunkMaterial { get; set; }
        
        public Transform GenerationModelTransform => transform;
        
        public Action OnGenerateMap;

        [Serializable]
        public struct Region
        {
            public string name;
            public float height;
            public Color color;
        }
    }
}