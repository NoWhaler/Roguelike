using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.Voronoi
{
    public class VoronoiTextureGenerator
    {
        private VoronoiBiomeDistributor _voronoiBiomeDistributor;
        
        [Inject]
        private void Constructor(VoronoiBiomeDistributor voronoiBiomeDistributor)
        {
            _voronoiBiomeDistributor = voronoiBiomeDistributor;
        }
        
        public Texture2D GenerateVoronoiTexture(int width, int height, int seed, List<Biome> biomes, int relaxationIterations)
        {
            float[,] voronoiMap = _voronoiBiomeDistributor.GenerateVoronoiMap(width, height, seed, biomes, relaxationIterations);
            List<BiomeCell> biomeCells = _voronoiBiomeDistributor.GenerateVoronoiBiomes(width, height, seed, biomes, relaxationIterations);
            return CreateVoronoiTexture(voronoiMap, biomeCells);
        }

        private Texture2D CreateVoronoiTexture(float[,] voronoiMap, List<BiomeCell> biomeCells)
        {
            int width = voronoiMap.GetLength(0);
            int height = voronoiMap.GetLength(1);
            Texture2D texture = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float voronoiValue = voronoiMap[x, y];
                    Biome biome = _voronoiBiomeDistributor.GetBiomeForPoint(new Vector2(x, y), biomeCells);
                    Color biomeColor = biome.Color;

                    float gradient = 1f - voronoiValue;
                    
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.2f - 0.1f;

                    Color modifiedColor = new Color(
                        Mathf.Clamp01(biomeColor.r * gradient + noise),
                        Mathf.Clamp01(biomeColor.g * gradient + noise),
                        Mathf.Clamp01(biomeColor.b * gradient + noise),
                        1f
                    );

                    colors[y * width + x] = modifiedColor;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}