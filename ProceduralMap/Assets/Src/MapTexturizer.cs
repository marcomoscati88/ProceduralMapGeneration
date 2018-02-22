using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTexturizer : MonoBehaviour
{

	[SerializeField]
	private Renderer _textureRender;

	public void DrawNoiseMap2D(float[,] _noiseMap)
	{
		int _width = _noiseMap.GetLength(0);
		int _height = _noiseMap.GetLength(1);

		_textureRender.transform.localScale = new Vector3(_width, 1, _height);
		Texture2D _texture = new Texture2D(_width, _height);

		Color[] _colorMap = new Color[_width * _height];
		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				_colorMap[y * _width + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
			}
		}

		_texture.SetPixels(_colorMap);
		_texture.Apply();
		_textureRender.sharedMaterial.mainTexture = _texture;
	}
}
