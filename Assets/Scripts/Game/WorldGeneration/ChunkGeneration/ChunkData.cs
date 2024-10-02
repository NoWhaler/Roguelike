using UnityEngine;

namespace Game.WorldGeneration.ChunkGeneration
{
    [System.Serializable]
    public class ChunkData
    {
        public float[,] heightMap;
        public Color[] colorMap;
    }
}