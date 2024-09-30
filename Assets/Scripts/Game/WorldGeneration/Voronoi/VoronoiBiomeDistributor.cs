using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.WorldGeneration.Voronoi
{
    [Serializable]
    public class Biome
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public int CellCount { get; private set; }

        public Biome(string name, Color color, int cellCount)
        {
            Name = name;
            Color = color;
            CellCount = cellCount;
        }
    }

    [Serializable]
    public class BiomeCell
    {
        public Vector2 SeedPoint { get; private set; }
        public Biome BiomeType { get; private set; }

        public BiomeCell(Vector2 seedPoint, Biome biomeType)
        {
            SeedPoint = seedPoint;
            BiomeType = biomeType;
        }
    }
    
    public class VoronoiBiomeDistributor
    {
        public static float[,] GenerateVoronoiMap(int width, int height, int seed, List<Biome> biomes)
        {
            int totalCells = biomes.Sum(b => b.CellCount);
            float[,] voronoiMap = new float[width, height];
            Vector2[] sites = new Vector2[totalCells];
            System.Random random = new System.Random(seed);

            int siteIndex = 0;
            foreach (var biome in biomes)
            {
                for (int i = 0; i < biome.CellCount; i++)
                {
                    sites[siteIndex] = new Vector2(random.Next(0, width), random.Next(0, height));
                    siteIndex++;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float closestDistance = float.MaxValue;
                    for (int i = 0; i < totalCells; i++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), sites[i]);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                        }
                    }

                    voronoiMap[x, y] = closestDistance;
                }
            }

            NormalizeVoronoiMap(voronoiMap);
            return voronoiMap;
        }

        private static void NormalizeVoronoiMap(float[,] map)
        {
            float maxVal = 0;
            foreach (float value in map)
            {
                if (value > maxVal) maxVal = value;
            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x, y] /= maxVal;
                }
            }
        }

        public static List<BiomeCell> GenerateVoronoiBiomes(int width, int height, int seed, List<Biome> biomes)
        {
            List<BiomeCell> biomeCells = new List<BiomeCell>();
            System.Random random = new System.Random(seed);

            foreach (var biome in biomes)
            {
                for (int i = 0; i < biome.CellCount; i++)
                {
                    Vector2 seedPoint = new Vector2(random.Next(0, width), random.Next(0, height));
                    biomeCells.Add(new BiomeCell(seedPoint, biome));
                }
            }

            return biomeCells;
        }

        public static Biome GetBiomeForPoint(Vector2 point, List<BiomeCell> biomeCells)
        {
            float minDistance = float.MaxValue;
            Biome closestBiome = null;

            foreach (var cell in biomeCells)
            {
                float distance = Vector2.Distance(point, cell.SeedPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBiome = cell.BiomeType;
                }
            }

            return closestBiome;
        }
    }
}