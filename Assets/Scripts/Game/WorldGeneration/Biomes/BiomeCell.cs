using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldGeneration.Biomes
{
    [Serializable]
    public class BiomeCell
    {
        public Vector2 SeedPoint { get; set; }
        public Biome BiomeType { get; set; }
        public List<Vector2> Points { get; set; }

        public BiomeCell(Vector2 seedPoint, Biome biomeType)
        {
            SeedPoint = seedPoint;
            BiomeType = biomeType;
            Points = new List<Vector2>();
        }
    }
}