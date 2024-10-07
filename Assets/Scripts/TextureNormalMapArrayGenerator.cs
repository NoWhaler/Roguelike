using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class TextureNormalMapArrayGenerator: EditorWindow
    {
        private Texture2D[] sourceTextures;
        private string sourceFolderPath = "Assets/Resources/Textures/Diffuse";
        private string normalMapFolderPath = "Assets/Resources/Textures/NormalMaps";
        private string outputPath = "Assets/Resources/TextureArrays";
        private bool generateNormalMaps = true;
        private float normalStrength = 5f;

        [MenuItem("Tools/Texture Array Generator")]
        public static void ShowWindow()
        {
            GetWindow<TextureNormalMapArrayGenerator>("Texture Array Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Texture Array Generator", EditorStyles.boldLabel);
            
            sourceFolderPath = EditorGUILayout.TextField("Source Folder", sourceFolderPath);
            normalMapFolderPath = EditorGUILayout.TextField("Normal Map Folder", normalMapFolderPath);
            outputPath = EditorGUILayout.TextField("Output Folder", outputPath);
            
            generateNormalMaps = EditorGUILayout.Toggle("Generate Normal Maps", generateNormalMaps);
            
            if (generateNormalMaps)
            {
                normalStrength = EditorGUILayout.Slider("Normal Strength", normalStrength, 1f, 10f);
            }

            if (GUILayout.Button("Generate Texture Arrays"))
            {
                GenerateTextureArrays();
            }
        }

        private void GenerateTextureArrays()
        {
            // Ensure directories exist
            Directory.CreateDirectory(normalMapFolderPath);
            Directory.CreateDirectory(outputPath);

            // Get all textures from the source folder
            string[] textureFiles = Directory.GetFiles(sourceFolderPath, "*.png");
            sourceTextures = textureFiles.Select(path => AssetDatabase.LoadAssetAtPath<Texture2D>(path)).ToArray();

            if (sourceTextures.Length == 0)
            {
                Debug.LogError("No textures found in source folder!");
                return;
            }

            // Generate normal maps if needed
            if (generateNormalMaps)
            {
                GenerateNormalMaps();
            }

            // Create and save texture arrays
            CreateTextureArray("DiffuseArray", sourceFolderPath, "*.png", TextureFormat.RGBA32);
            CreateTextureArray("NormalArray", normalMapFolderPath, "*.png", TextureFormat.RGBA32);

            AssetDatabase.Refresh();
        }

        private void GenerateNormalMaps()
        {
            foreach (var texture in sourceTextures)
            {
                string normalMapPath = Path.Combine(normalMapFolderPath, texture.name + "_normal.png");
                
                // Skip if normal map already exists
                if (File.Exists(normalMapPath)) continue;

                // Generate normal map
                Texture2D normalMap = GenerateNormalMap(texture);
                
                // Save normal map
                byte[] bytes = normalMap.EncodeToPNG();
                File.WriteAllBytes(normalMapPath, bytes);
                
                Object.DestroyImmediate(normalMap);
            }
        }

        private Texture2D GenerateNormalMap(Texture2D source)
        {
            Texture2D normalMap = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            
            for (int y = 0; y < source.height; y++)
            {
                for (int x = 0; x < source.width; x++)
                {
                    Vector3 normal = CalculateNormal(source, x, y);
                    Color normalColor = new Color(normal.x * 0.5f + 0.5f, normal.y * 0.5f + 0.5f, normal.z, 1);
                    normalMap.SetPixel(x, y, normalColor);
                }
            }
            
            normalMap.Apply();
            return normalMap;
        }

        private Vector3 CalculateNormal(Texture2D source, int x, int y)
        {
            float[,] heightSamples = new float[3, 3];
            
            // Sample heights in 3x3 grid
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    int sampleX = Mathf.Clamp(x + offsetX, 0, source.width - 1);
                    int sampleY = Mathf.Clamp(y + offsetY, 0, source.height - 1);
                    
                    Color color = source.GetPixel(sampleX, sampleY);
                    heightSamples[offsetX + 1, offsetY + 1] = color.grayscale;
                }
            }
            
            float dx = (heightSamples[0, 0] + 2 * heightSamples[0, 1] + heightSamples[0, 2]) -
                       (heightSamples[2, 0] + 2 * heightSamples[2, 1] + heightSamples[2, 2]);
            
            float dy = (heightSamples[0, 0] + 2 * heightSamples[1, 0] + heightSamples[2, 0]) -
                       (heightSamples[0, 2] + 2 * heightSamples[1, 2] + heightSamples[2, 2]);
            
            Vector3 normal = new Vector3(dx * normalStrength, dy * normalStrength, 1).normalized;
            return normal;
        }

        private void CreateTextureArray(string arrayName, string sourcePath, string searchPattern, TextureFormat format)
        {
            string[] texturePaths = Directory.GetFiles(sourcePath, searchPattern);
            Texture2D[] textures = texturePaths.Select(path => AssetDatabase.LoadAssetAtPath<Texture2D>(path)).ToArray();
    
            if (textures.Length == 0)
            {
                Debug.LogError($"No textures found for {arrayName}");
                return;
            }
    
            Texture2D[] processedTextures = new Texture2D[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D sourceTexture = textures[i];
                
                Texture2D readableTexture = MakeTextureReadable(sourceTexture);
                
                processedTextures[i] = ConvertTextureFormat(readableTexture, format);
                
                if (readableTexture != sourceTexture)
                {
                    DestroyImmediate(readableTexture);
                }
            }
    
            Texture2DArray textureArray = new Texture2DArray(
                processedTextures[0].width,
                processedTextures[0].height,
                processedTextures.Length,
                format,
                true,
                false
            );
    
            for (int i = 0; i < processedTextures.Length; i++)
            {
                Graphics.CopyTexture(processedTextures[i], 0, 0, textureArray, i, 0);
                
                if (processedTextures[i] != textures[i])
                {
                    DestroyImmediate(processedTextures[i]);
                }
            }
    
            string outputFilePath = Path.Combine(outputPath, arrayName + ".asset");
            AssetDatabase.CreateAsset(textureArray, outputFilePath);
        }
    
        private Texture2D MakeTextureReadable(Texture2D source)
        {
            // If texture is already readable, return it
            if (source.isReadable)
                return source;
    
            // Create a temporary RenderTexture
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );
    
            // Copy the texture to the render texture
            Graphics.Blit(source, renderTexture);
    
            // Create a new readable texture
            Texture2D readableTexture = new Texture2D(source.width, source.height);
            
            // Remember the active render texture
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            
            // Read pixels from the render texture to the readable texture
            readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readableTexture.Apply();
            
            // Restore the active render texture
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
    
            return readableTexture;
        }
    
        private Texture2D ConvertTextureFormat(Texture2D source, TextureFormat targetFormat)
        {
            if (source.format == targetFormat)
                return source;
    
            // Create a new texture with the target format
            Texture2D convertedTexture = new Texture2D(source.width, source.height, targetFormat, false);
            
            // Copy the pixels
            Color[] pixels = source.GetPixels();
            convertedTexture.SetPixels(pixels);
            convertedTexture.Apply();
    
            return convertedTexture;
        }
    }
}