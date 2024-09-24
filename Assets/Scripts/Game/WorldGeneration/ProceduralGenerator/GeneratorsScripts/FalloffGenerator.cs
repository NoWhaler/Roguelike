using UnityEngine;

namespace Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts
{
    public class FalloffGenerator
    {
        public static float[,] GenerateFalloffMap(int size, float radius)
        {
            float[,] map = new float[size, size];
            float halfSize = size / 2f;
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = (i - halfSize) / halfSize;
                    float y = (j - halfSize) / halfSize;
                    
                    float distanceFromCenter = Mathf.Sqrt(x * x + y * y);
                    float value = Evaluate(distanceFromCenter, radius);
                    
                    map[i, j] = value;
                }
            }
            
            return map;
        }
        
        private static float Evaluate(float value, float radius)
        {
            float a = 3;
            float b = 2.2f;
            
            value = Mathf.Clamp01(value * radius);
            
            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}