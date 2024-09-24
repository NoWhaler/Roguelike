using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class VoronoiGenerator
    {
        public static float[,] GenerateVoronoiMap(int width, int height, int seed, int sitesNumber)
        {
            float[,] voronoiMap = new float[width, height];
            int numSites = sitesNumber;
            Vector2[] sites = new Vector2[sitesNumber];

            System.Random random = new System.Random(seed);
            for (int i = 0; i < numSites; i++)
            {
                sites[i] = new Vector2(random.Next(0, width), random.Next(0, height));
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float closestDistance = float.MaxValue;
                    for (int i = 0; i < numSites; i++)
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
    }
}