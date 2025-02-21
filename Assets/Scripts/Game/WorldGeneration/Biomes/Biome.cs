using System;
using System.Collections.Generic;
using Game.ProductionResources.Enum;
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
        
        [field: SerializeField] public int BiomeIndex { get; private set; }
        
        public List<BiomeResourceInfo> Resources { get; private set; }
        
        public void SetResourceInfo(List<BiomeResourceInfo> resources)
        {
            Resources = resources;
        }

        public Biome(string name, BiomeType biomeType, Color color, int biomeIndex)
        {
            Name = name;
            BiomeType = biomeType;
            Color = color;
            BiomeIndex = biomeIndex;
        }
    }
    
    public class BiomeResourceInfo
    {
        public ResourceType ResourceType { get; private set; }
        public int Count { get; private set; }

        public BiomeResourceInfo(ResourceType resourceType, int count)
        {
            ResourceType = resourceType;
            Count = count;
        }
    }
}