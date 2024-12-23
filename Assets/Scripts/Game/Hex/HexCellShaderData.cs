using System;
using UnityEngine;

namespace Game.Hex
{
    public class HexCellShaderData: MonoBehaviour, IDisposable
    {
	    private Texture2D _cellTexture;
	    
	    private Color32[] _cellTextureData;

		public void Initialize (int x, int z)
		{
			if (_cellTexture)
			{
				_cellTexture.Reinitialize(x, z);
			}
			else
			{
				_cellTexture = new Texture2D(
					x, z, TextureFormat.RGBA32, false, true
				);
				_cellTexture.filterMode = FilterMode.Point;
				_cellTexture.wrapMode = TextureWrapMode.Clamp;
				Shader.SetGlobalTexture("_HexCellData", _cellTexture);
			}
			Shader.SetGlobalVector(
				"_HexCellData_TexelSize",
				new Vector4(1f / x, 1f / z, x, z)
			);

			if (_cellTextureData == null || _cellTextureData.Length != x * z)
			{
				_cellTextureData = new Color32[x * z];
			}
			
			else 
			{
				for (var i = 0; i < _cellTextureData.Length; i++) {
					_cellTextureData[i] = new Color32(0, 0, 0, 0);
				}
			}

			enabled = true;
		}

		public void LateUpdate ()
		{
			_cellTexture.SetPixels32(_cellTextureData);
			_cellTexture.Apply();

			enabled = false;
		}

		public void Dispose()
		{
			
		}
    }
}