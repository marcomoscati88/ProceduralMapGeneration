using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTexturizer : MonoBehaviour
{
	private static MapTexturizer _instance;
	private MapTexturizer()
	{

	}

	public static MapTexturizer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MapTexturizer();
			}
			return _instance;
		}
	}

	private Renderer _textureRender;

	public void TextureNoiseMap2D(float[,] _noiseMap, GameObject _obj)
	{
		int _width = _noiseMap.GetLength(0);
		int _height = _noiseMap.GetLength(1);

		Color[] _colorMap = new Color[_width * _height];
		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				_colorMap[y * _width + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
			}
		}

		TextureColor(_colorMap, _width, _height, _obj);
	}

	public Texture2D TextureColor(Color[] _colorMap, int _width, int _height, GameObject _obj)
	{
		Texture2D _texture = new Texture2D(_width, _height);
		_texture.filterMode = FilterMode.Point;
		_texture.wrapMode = TextureWrapMode.Clamp;
		_texture.SetPixels(_colorMap);
		_texture.Apply();
		DrawTexture(_texture, _obj);
		return _texture;
	}

	public void DrawTexture(Texture2D _texture, GameObject _obj)
	{
		_textureRender = _obj.GetComponent<Renderer>();
		_textureRender.sharedMaterial.mainTexture = _texture;
		_textureRender.transform.localScale = new Vector3(_texture.width, 1, _texture.height);
	}

	public void TextureMesh(MeshData _meshData, Texture2D _texture, MeshFilter _meshFilter, MeshRenderer _meshRenderer){
		_meshFilter.sharedMesh = _meshData.CreateMesh();
		_meshRenderer.sharedMaterial.mainTexture = _texture;

	}
}
