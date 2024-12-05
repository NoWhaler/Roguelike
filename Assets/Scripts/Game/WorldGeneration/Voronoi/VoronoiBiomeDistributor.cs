using System.Collections.Generic;
using Game.WorldGeneration.Biomes;
using UnityEngine;

namespace Game.WorldGeneration.Voronoi
{
    public class VoronoiBiomeDistributor
    {
        public float[,] GenerateVoronoiMap(int width, int height, int seed, List<Biome> biomes, int relaxationIterations)
        {
            List<BiomeCell> biomeCells = GenerateInitialBiomeCells(width, height, seed, biomes);
            
            for (int i = 0; i < relaxationIterations; i++)
            {
                RelaxCells(biomeCells, width, height);
            }

            return GenerateVoronoiMapFromCells(biomeCells, width, height);
        }

        private List<BiomeCell> GenerateInitialBiomeCells(int width, int height, int seed, List<Biome> biomes)
        {
            List<BiomeCell> cells = new List<BiomeCell>();
            System.Random random = new System.Random(seed);

            foreach (var biome in biomes)
            {
                Vector2 seedPoint = new Vector2(random.Next(0, width), random.Next(0, height));
                cells.Add(new BiomeCell(seedPoint, biome));
            }

            return cells;
        }

        private void RelaxCells(List<BiomeCell> cells, int width, int height)
        {
            foreach (var cell in cells)
            {
                cell.Points.Clear();
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 point = new Vector2(x, y);
                    BiomeCell closestCell = null;
                    float minDistance = float.MaxValue;

                    foreach (var cell in cells)
                    {
                        float distance = Vector2.Distance(point, cell.SeedPoint);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestCell = cell;
                        }
                    }

                    closestCell?.Points.Add(point);
                }
            }

            foreach (var cell in cells)
            {
                if (cell.Points.Count > 0)
                {
                    Vector2 centroid = Vector2.zero;
                    foreach (var point in cell.Points)
                    {
                        centroid += point;
                    }
                    centroid /= cell.Points.Count;
                    cell.SeedPoint = centroid;
                }
            }
        }

        private float[,] GenerateVoronoiMapFromCells(List<BiomeCell> cells, int width, int height)
        {
            float[,] voronoiMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float closestDistance = float.MaxValue;
                    foreach (var cell in cells)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), cell.SeedPoint);
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

        private void NormalizeVoronoiMap(float[,] map)
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

        public List<BiomeCell> GenerateVoronoiBiomes(int width, int height, int seed, List<Biome> biomes, int relaxationIterations)
        {
            List<BiomeCell> biomeCells = GenerateInitialBiomeCells(width, height, seed, biomes);
            
            for (int i = 0; i < relaxationIterations; i++)
            {
                RelaxCells(biomeCells, width, height);
            }

            return biomeCells;
        }

        public Biome GetBiomeForPoint(Vector2 point, List<BiomeCell> biomeCells)
        {
            float minDistance = float.MaxValue;
            Biome closestBiome = null;

            foreach (var cell in biomeCells)
            {
                float distance = Vector2.Distance(point, cell.SeedPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBiome = cell.Biome;
                }
            }

            return closestBiome;
        }
    }
}