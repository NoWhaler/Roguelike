using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class WorleyNoise
    {
        public static float[,] GenerateWorleyNoise(int width, int height, float scale, int seed)
        {
            System.Random random = new System.Random(seed);
            Vector2[] points = GenerateRandomPoints(random, width, height, scale);
            float[,] noiseMap = new float[width, height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float minDist = float.MaxValue;
                    foreach (Vector2 point in points)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), point);
                        if (dist < minDist)
                        {
                            minDist = dist;
                        }
                    }
                    noiseMap[x, y] = Mathf.InverseLerp(0, width / scale, minDist);
                }
            }
            return noiseMap;
        }

        private static Vector2[] GenerateRandomPoints(System.Random random, int width, int height, float scale)
        {
            int pointCount = Mathf.CeilToInt((width * height) / (scale * scale));
            Vector2[] points = new Vector2[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float x = random.Next(0, width);
                float y = random.Next(0, height);
                points[i] = new Vector2(x, y);
            }
            return points;
        }
    }
}