using System;
using System.Collections.Generic;
using Game.WorldGeneration.RandomGenerator.Models;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.RandomGenerator.Controllers
{
    public class GeneratorController : IInitializable, IValidatable, IDisposable
    {
        private GeneratorModel _generatorModel;
        
        public void Initialize()
        {
            GenerateMap();
            _generatorModel.OnGenerateMap += GenerateMap;
        }
        
        public void Dispose()
        {
            _generatorModel.OnGenerateMap -= GenerateMap;
        }

        public void Validate()
        {
            if (_generatorModel.isValidateAvailable)
            {
                GenerateMap();
            }
        }

        [Inject]
        private void Constructor(GeneratorModel generatorModel)
        {
            _generatorModel = generatorModel;
        }
        
        public void GenerateMap()
        {
            float[,] noiseMap = GenerateNoiseMap();
            Color[] colorMap = GenerateColorMap(noiseMap);
            MeshData meshData = GenerateTerrainMesh(noiseMap);
            
            Mesh mesh = meshData.CreateMesh();
            mesh.RecalculateNormals();
            _generatorModel.MapMeshFilter.mesh = mesh;
            _generatorModel.MapMeshCollider.sharedMesh = mesh;
            
            ApplyColorToMesh(mesh, colorMap);
        }
        
        private Color[] GenerateColorMap(float[,] noiseMap)
        {
            Color[] colorMap = new Color[_generatorModel.MapWidth * _generatorModel.MapHeight];
    
            for (int y = 0; y < _generatorModel.MapHeight; y++)
            {
                for (int x = 0; x < _generatorModel.MapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < _generatorModel.Regions.Length; i++)
                    {
                        if (currentHeight <= _generatorModel.Regions[i].height)
                        {
                            colorMap[y * _generatorModel.MapWidth + x] = _generatorModel.Regions[i].color;
                            break;
                        }
                    }
                }
            }
    
            return colorMap;
        }
        
        private float[,] GenerateNoiseMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(_generatorModel.MapWidth, _generatorModel.MapHeight,
                _generatorModel.NoiseScale, _generatorModel.Octaves,
                _generatorModel.Persistence, _generatorModel.Lacunarity, Vector2.zero);
    
            if (_generatorModel.UseFalloffMap)
            {
                float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(_generatorModel.MapWidth);
                for (int y = 0; y < _generatorModel.MapHeight; y++)
                {
                    for (int x = 0; x < _generatorModel.MapWidth; x++)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }
                }
            }
            
            return noiseMap;
        }
        
        private MeshData GenerateTerrainMesh(float[,] heightMap)
        {
            MeshData meshData = new MeshData(_generatorModel.MapWidth, _generatorModel.MapHeight);
            int vertexIndex = 0;

            for (int y = 0; y < _generatorModel.MapHeight; y++)
            {
                for (int x = 0; x < _generatorModel.MapWidth; x++)
                {
                    float height = _generatorModel.HeightCurve.Evaluate(heightMap[x, y]) * _generatorModel.HeightMultiplier;
                    meshData.vertices[vertexIndex] = new Vector3(x, height, y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)_generatorModel.MapWidth, y / (float)_generatorModel.MapHeight);

                    if (x < _generatorModel.MapWidth - 1 && y < _generatorModel.MapHeight - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + _generatorModel.MapWidth + 1, vertexIndex + _generatorModel.MapWidth);
                        meshData.AddTriangle(vertexIndex + _generatorModel.MapWidth + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
        
        private void ApplyColorToMesh(Mesh mesh, Color[] colorMap)
        {
            mesh.colors = colorMap;
        }

    }
    
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            Vector2[] octaveOffsets = new Vector2[octaves];
            System.Random prng = new System.Random();
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
    
    public static class FalloffGenerator
    {
        public static float[,] GenerateFalloffMap(int size)
        {
            float[,] map = new float[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float xNorm = x / (float)size * 2 - 1;
                    float yNorm = y / (float)size * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(xNorm), Mathf.Abs(yNorm));
                    map[x, y] = Evaluate(value);
                }
            }

            return map;
        }
        
        private static float Evaluate(float value)
        {
            float a = 3;
            float b = 2.2f;
    
            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}