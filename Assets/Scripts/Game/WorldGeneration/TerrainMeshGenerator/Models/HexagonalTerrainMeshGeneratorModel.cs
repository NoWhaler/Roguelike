using System.Collections.Generic;
using Game.Hex;
using Game.WorldGeneration.ChunkGeneration.Model;
using UnityEngine;

namespace Game.WorldGeneration.TerrainMeshGenerator.Models
{
    public class HexagonalTerrainMeshGeneratorModel: MonoBehaviour
    {
        [field: Header("Hexagon Settings")]
        [field: SerializeField] public float HexRadius { get; set; }
        
        [field: Header("Chunk Settings")]
        [field: SerializeField] public int HexagonsPerChunkSide { get; set; }
        
        [field: Header("Mesh Settings")]
        [field: SerializeField] public Material DefaultMaterial { get; set; }
        
        [field: SerializeField] public float HexWidth { get; set; }
        [field: SerializeField] public float HexHeight { get; set; }
        
        [field: Header("Prefabs")]
        [field: SerializeField] public HexTerrainChunkModel HexTerrainChunkPrefab { get; private set; }
        
        [field: SerializeField] public HexModel HexPrefab { get; private set; }

        public int GlobalCellIndex { get; set; }
        
        private void Awake()
        {
            CalculateHexMetrics();
        }
        
        private void CalculateHexMetrics()
        {
            HexWidth = HexRadius * 2f;
            HexHeight = HexWidth * Mathf.Sqrt(3f) / 2f;
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