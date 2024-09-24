using Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts;
using UnityEngine;

namespace Game.WorldGeneration.Voronoi
{
    public class VoronoiCells: MonoBehaviour
    {
        public int width = 256;           // Width of the Voronoi map
        public int height = 256;          // Height of the Voronoi map
        public int seed = 12345;          // Seed for randomness
        public int sitesNumber = 10;      // Number of sites for Voronoi generation
        public Material voronoiMaterial;  // Material to display the Voronoi texture
    
        private void Start()
        {
            float[,] voronoiMap = VoronoiGenerator.GenerateVoronoiMap(width, height, seed, sitesNumber);
            
            Texture2D voronoiTexture = CreateVoronoiTexture(voronoiMap);
            voronoiMaterial.mainTexture = voronoiTexture;
    
            CreateDisplayQuad(voronoiTexture);
        }
    
        private Texture2D CreateVoronoiTexture(float[,] voronoiMap)
        {
            Texture2D texture = new Texture2D(voronoiMap.GetLength(0), voronoiMap.GetLength(1));
            Color[] colors = new Color[voronoiMap.GetLength(0) * voronoiMap.GetLength(1)];
    
            for (int y = 0; y < voronoiMap.GetLength(1); y++)
            {
                for (int x = 0; x < voronoiMap.GetLength(0); x++)
                {
                    float value = voronoiMap[x, y];
                    colors[y * voronoiMap.GetLength(0) + x] = new Color(value, value, value); // Grayscale
                }
            }
    
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    
        private void CreateDisplayQuad(Texture2D texture)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = Vector3.zero; // Position the Quad at the origin
            quad.transform.localScale = new Vector3(width / 10f, height / 10f, 1); // Adjust size
            quad.GetComponent<Renderer>().material = voronoiMaterial; // Assign the material with the texture
        }
    }
}