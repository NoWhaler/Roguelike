using System.Collections.Generic;
using Game.WorldGeneration.ChunkGeneration.Model;
using Game.WorldGeneration.Hex;
using UnityEngine;

namespace Game.WorldGeneration.TerrainMeshGenerator.Models
{
    public class HexagonalTerrainMeshGeneratorModel: MonoBehaviour
    {
        [Header("Hexagon Settings")]
        [field: SerializeField] public float hexRadius;
        
        [Header("Chunk Settings")]
        [field: SerializeField] public int hexagonsPerChunkSide;
        
        [Header("Mesh Settings")]
        [field: SerializeField] public Material defaultMaterial;
        
        [field: SerializeField] public float hexWidth;
        [field: SerializeField] public float hexHeight;
        
        [field: Header("Prefabs")]
        [field: SerializeField] public HexTerrainChunkModel HexTerrainChunkPrefab { get; private set; }
        
        [field: SerializeField] public HexModel HexPrefab { get; private set; }
        
        private void Awake()
        {
            CalculateHexMetrics();
        }
        
        private void CalculateHexMetrics()
        {
            hexWidth = hexRadius * 2f;
            hexHeight = hexWidth * Mathf.Sqrt(3f) / 2f;
        }
        
        public class HexChunk
        {
            public Vector2Int ChunkCoord { get; private set; }
            public HexTerrainChunkModel ChunkObject { get; set; }
            public List<Vector3> Vertices { get; private set; }
            public List<int> Triangles { get; private set; }
            
            public List<Vector2> UVs { get; set; }
            public List<Color> Colors { get; private set; }
            
            public HexChunk(Vector2Int coord)
            {
                ChunkCoord = coord;
                Vertices = new List<Vector3>();
                Triangles = new List<int>();
                Colors = new List<Color>();
                UVs = new List<Vector2>();
            }
        }
    }
}