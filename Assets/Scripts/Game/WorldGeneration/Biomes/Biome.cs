using System;
using Game.WorldGeneration.Biomes.Enum;
using UnityEngine;

namespace Game.WorldGeneration.Biomes
{
    [Serializable]
    public class Biome
    {
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public BiomeType BiomeType { get; private set; }
        
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public int CellCount { get; private set; }

        public Biome(string name, BiomeType biomeType, Color color, int cellCount)
        {
            Name = name;
            BiomeType = biomeType;
            Color = color;
            CellCount = cellCount;
        }
    }
}