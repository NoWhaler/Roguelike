using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldGeneration.Biomes
{
    [Serializable]
    public class BiomeCell
    {
        public Vector2 SeedPoint { get; set; }
        public Biome Biome { get; set; }
        public List<Vector2> Points { get; set; }

        public BiomeCell(Vector2 seedPoint, Biome biome)
        {
            SeedPoint = seedPoint;
            Biome = biome;
            Points = new List<Vector2>();
        }
    }
}