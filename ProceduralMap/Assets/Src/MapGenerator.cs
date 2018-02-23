using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MapTexturizer))]
public class MapGenerator : MonoBehaviour
{

	[SerializeField]
	private GameObject _objToMap = null;

	[SerializeField]
	private DrawMode _drawMode = DrawMode.NoiseMap;

	[SerializeField]
	[Range(1, 100000)]
	private int _mapWidth = 100;

	[SerializeField]
	[Range(1, 100000)]
	private int _mapHeight = 100;

	[SerializeField]
	private float _noiseMagnitude = 2.0f;

	[SerializeField]
	[Range(0, 100)]
	private int _layerNumber = 2;

	[SerializeField]
	[Range(0, 100)]
	private float _detailLevel = 0.5f;

	[SerializeField]
	[Range(1,100)]
	private float _lacunarity = 1.0f;

	[SerializeField]
	private bool _autoUpdate = false;

	[SerializeField]
	private int _seed;

	[SerializeField]
	private Vector2 _offset;

	[SerializeField]
	private TerrainType[] _regions ;

	public void GenerateMap()
	{
		float[,] _noiseMap = NoiseGenerator.Instance.GenerateNoiseMap(_mapWidth, _mapHeight, _seed, _noiseMagnitude, _layerNumber, _detailLevel, _lacunarity, _offset);

		Color[] _colorMap = new Color[_mapWidth * _mapHeight];

		for (int y = 0; y < _mapHeight; y++)
		{
			for (int x = 0; x < _mapWidth; x++)
			{
				float _currentHeight = _noiseMap[x, y];
				for (int i = 0; i < _regions.Length; i++)
				{
					if (_currentHeight <= _regions[i]._height)
					{
						_colorMap[y * _mapWidth + x] = _regions[i]._color;
						break;
					}
				}
			}
		}

		if (_drawMode == DrawMode.NoiseMap)
		{
			MapTexturizer.Instance.TextureNoiseMap2D(_noiseMap, _objToMap);
		}else if (_drawMode == DrawMode.ColorMap) {
			MapTexturizer.Instance.TextureColor(_colorMap, _mapWidth, _mapHeight, _objToMap);
		}

	}

	[CustomEditor(typeof(MapGenerator))]
	public class MapGeneratorEditor : Editor {
		public override void OnInspectorGUI()
		{
			MapGenerator _generator = (MapGenerator)target;
			if (DrawDefaultInspector() && _generator._autoUpdate)
			{
				_generator.GenerateMap();
			}
			
			if (GUILayout.Button("Generate"))
			{
				_generator.GenerateMap();
			}
		}
	}
}
