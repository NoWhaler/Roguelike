using Game.WorldGeneration.ProceduralGenerator.GeneratorsScripts;
using UnityEngine;

namespace Game.WorldGeneration.Voronoi
{
    public class VoronoiCells: MonoBehaviour
    {
        [SerializeField] private int width = 256;          
        [SerializeField] private int height = 256; 
        [SerializeField] private int seed = 12345;          
        [SerializeField] private int sitesNumber = 10;      
        [SerializeField] private Material voronoiMaterial;
        
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
                    colors[y * voronoiMap.GetLength(0) + x] = new Color(value, value, value);
                }
            }
    
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    
        private void CreateDisplayQuad(Texture2D texture)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = Vector3.zero;
            quad.transform.localScale = new Vector3(width / 10f, height / 10f, 1); 
            quad.GetComponent<Renderer>().material = voronoiMaterial;
        }
    }
}