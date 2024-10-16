using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public static class CombinedGenerator
    {
        public static float[,] GenerateCombinedMap(int size, float simplexWeight, float worleyWeight, float diamondSquareWeight,
                                                           float simplexScale, int simplexOctaves, float simplexPersistence,
                                                           float simplexLacunarity, float worleyScale, float diamondSquareRoughness,
                                                           int seed)
            {
                System.Random prng = new System.Random(seed);
                Vector2 randomOffset = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
                
                float[,] simplexMap = FBMGenerator.GenerateFBM(size, size, simplexScale, simplexOctaves,
                    simplexPersistence, simplexLacunarity, randomOffset, seed);
                float[,] worleyMap = WorleyNoise.GenerateWorleyNoise(size, size, worleyScale, seed);
                float[,] diamondSquareMap = DiamondSquareGenerator.GenerateDiamondSquareMap(size, diamondSquareRoughness, seed);
                float totalWeight = simplexWeight + worleyWeight + diamondSquareWeight;
                
                simplexWeight /= totalWeight;
                worleyWeight /= totalWeight;
                diamondSquareWeight /= totalWeight;
    
                float[,] combinedMap = new float[size, size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        combinedMap[x, y] = simplexMap[x, y] * simplexWeight +
                                            worleyMap[x, y] * worleyWeight +
                                            diamondSquareMap[x, y] * diamondSquareWeight;
                    }
                }
    
                return combinedMap;
            }
    }
}