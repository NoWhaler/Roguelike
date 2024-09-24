using System.Linq;
using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class DiamondSquareGenerator
    {
        public static float[,] GenerateDiamondSquareMap(int size, float squareRoughness, int seed, float maxHeight = 1f)
        {
            int mapSize = Mathf.NextPowerOfTwo(size - 1) + 1;
            float[,] map = new float[mapSize, mapSize];
            System.Random prng = new System.Random(seed);

            // Initialize corners
            map[0, 0] = (float)prng.NextDouble() * maxHeight * 0.5f;
            map[0, mapSize - 1] = (float)prng.NextDouble() * maxHeight * 0.5f;
            map[mapSize - 1, 0] = (float)prng.NextDouble() * maxHeight * 0.5f;
            map[mapSize - 1, mapSize - 1] = (float)prng.NextDouble() * maxHeight * 0.5f;

            int stepSize = mapSize - 1;
            float scale = maxHeight * 0.5f;

            while (stepSize > 1)
            {
                int halfStep = stepSize / 2;

                // Square step
                for (int y = halfStep; y < mapSize; y += stepSize)
                {
                    for (int x = halfStep; x < mapSize; x += stepSize)
                    {
                        SquareStep(map, x, y, halfStep, scale * squareRoughness, prng, maxHeight);
                    }
                }

                // Diamond step
                for (int y = 0; y < mapSize; y += halfStep)
                {
                    for (int x = (y + halfStep) % stepSize; x < mapSize; x += stepSize)
                    {
                        DiamondStep(map, x, y, halfStep, scale * squareRoughness, prng, maxHeight);
                    }
                }

                stepSize /= 2;
                scale *= 0.5f;
            }

            // Apply multiple passes of smoothing and spike removal
            for (int i = 0; i < 3; i++)
            {
                RemoveSpikes(map, maxHeight);
                SmoothMap(map);
            }

            // Normalize the map
            NormalizeMap(map, maxHeight);

            // Crop the map if necessary
            if (size < mapSize)
            {
                return CropMap(map, size);
            }

            return map;
        }

        private static void SquareStep(float[,] map, int x, int y, int halfStep, float scale, System.Random prng, float maxHeight)
        {
            int size = map.GetLength(0);
            float avg = (
                map[(x - halfStep + size) % size, (y - halfStep + size) % size] +
                map[(x + halfStep) % size, (y - halfStep + size) % size] +
                map[(x - halfStep + size) % size, (y + halfStep) % size] +
                map[(x + halfStep) % size, (y + halfStep) % size]
            ) / 4f;

            float offset = ((float)prng.NextDouble() * 2 - 1) * scale;
            map[x, y] = Mathf.Clamp(avg + offset, 0, maxHeight);
        }

        private static void DiamondStep(float[,] map, int x, int y, int halfStep, float scale, System.Random prng, float maxHeight)
        {
            int size = map.GetLength(0);
            float avg = (
                map[(x - halfStep + size) % size, y] +
                map[(x + halfStep) % size, y] +
                map[x, (y - halfStep + size) % size] +
                map[x, (y + halfStep) % size]
            ) / 4f;

            float offset = ((float)prng.NextDouble() * 2 - 1) * scale;
            map[x, y] = Mathf.Clamp(avg + offset, 0, maxHeight);
        }

        private static void RemoveSpikes(float[,] map, float maxHeight)
        {
            int size = map.GetLength(0);
            float threshold = maxHeight * 0.8f; // Adjust this value to control spike detection sensitivity

            for (int y = 1; y < size - 1; y++)
            {
                for (int x = 1; x < size - 1; x++)
                {
                    float currentHeight = map[x, y];
                    if (currentHeight > threshold)
                    {
                        float[] neighbors = new float[]
                        {
                            map[x-1, y], map[x+1, y], map[x, y-1], map[x, y+1],
                            map[x-1, y-1], map[x+1, y-1], map[x-1, y+1], map[x+1, y+1]
                        };

                        float avgNeighborHeight = neighbors.Average();
                        if (currentHeight > avgNeighborHeight * 1.5f) // Spike detection
                        {
                            map[x, y] = avgNeighborHeight * 1.1f; // Reduce spike height
                        }
                    }
                }
            }
        }

        private static void SmoothMap(float[,] map)
        {
            int size = map.GetLength(0);
            float[,] smoothedMap = new float[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float sum = 0;
                    int count = 0;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < size && ny >= 0 && ny < size)
                            {
                                sum += map[nx, ny];
                                count++;
                            }
                        }
                    }

                    smoothedMap[x, y] = sum / count;
                }
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    map[x, y] = smoothedMap[x, y];
                }
            }
        }

        private static void NormalizeMap(float[,] map, float maxHeight)
        {
            int size = map.GetLength(0);
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    minValue = Mathf.Min(minValue, map[x, y]);
                    maxValue = Mathf.Max(maxValue, map[x, y]);
                }
            }

            float range = maxValue - minValue;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    map[x, y] = ((map[x, y] - minValue) / range) * maxHeight;
                }
            }
        }

        private static float[,] CropMap(float[,] map, int newSize)
        {
            float[,] croppedMap = new float[newSize, newSize];
            for (int y = 0; y < newSize; y++)
            {
                for (int x = 0; x < newSize; x++)
                {
                    croppedMap[x, y] = map[x, y];
                }
            }
            return croppedMap;
        }
    }
}