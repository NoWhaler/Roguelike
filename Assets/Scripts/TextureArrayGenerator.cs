using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class TextureArrayGenerator : MonoBehaviour
    {
        [MenuItem("Assets/Create/Texture2DArray")]
        public static void CreateTexture2DArray()
        {
            Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

            if (textures.Length == 0)
            {
                Debug.LogError("No textures selected!");
                return;
            }
            
            int width = textures[0].width;
            int height = textures[0].height;
            TextureFormat format = textures[0].format;

            Texture2DArray textureArray = new Texture2DArray(width, height, textures.Length, format, false);

            for (int i = 0; i < textures.Length; i++)
            {
                for (int mip = 0; mip < textures[i].mipmapCount; mip++)
                {
                    Graphics.CopyTexture(textures[i], 0, mip, textureArray, i, mip);
                }
            }

            textureArray.Apply();

            AssetDatabase.CreateAsset(textureArray, "Assets/NewTexture2DArray.asset");
            AssetDatabase.SaveAssets();

            Debug.Log("Texture2DArray created and saved as 'Assets/NewTexture2DArray.asset'");
        }
    }
}