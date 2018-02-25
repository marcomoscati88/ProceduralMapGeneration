using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MapTexturizer))]
public class MapGenerator : MonoBehaviour
{
	//If you change this value be aware that the number you use MINUS 1 MUST be a factor of (2, 4 or 8). In this case, our mesh, will be 240x240 because we use a chunksize of 241.
	public const int MAPCHUNKSIZE = 241;

	[SerializeField]
	private GameObject _objToMap = null;

	[SerializeField]
	private MeshFilter _meshFilter;

	[SerializeField]
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private DrawMode _drawMode = DrawMode.NoiseMap;

	[SerializeField]
	[Range(0, 6)]
	private int _levelOfDetail;

	[SerializeField]
	[Range(1, 241)]
	private int _mapSquareSize = 1;

	[SerializeField]
	private float _heightMultiplier = 1.0f;

	[SerializeField]
	private AnimationCurve _heightCurve;

	[SerializeField]
	private float _noiseMagnitude = 2.0f;

	[SerializeField]
	[Range(0, 100)]
	private int _layerNumber = 2;

	[SerializeField]
	[Range(0, 100)]
	private float _detailLevel = 0.5f;

	[SerializeField]
	[Range(1, 100)]
	private float _lacunarity = 1.0f;

	[SerializeField]
	private bool _autoUpdate = false;

	[SerializeField]
	private int _seed;

	[SerializeField]
	private Vector2 _offset;

	[SerializeField]
	private TerrainType[] _regions;

	public void GenerateMap()
	{
		float[,] _noiseMap = NoiseGenerator.Instance.GenerateNoiseMap(_mapSquareSize, _mapSquareSize, _seed, _noiseMagnitude, _layerNumber, _detailLevel, _lacunarity, _offset);

		Color[] _colorMap = new Color[_mapSquareSize * _mapSquareSize];

		for (int y = 0; y < _mapSquareSize; y++)
		{
			for (int x = 0; x < _mapSquareSize; x++)
			{
				float _currentHeight = _noiseMap[x, y];
				for (int i = 0; i < _regions.Length; i++)
				{
					if (_currentHeight <= _regions[i].height)
					{
						_colorMap[y * _mapSquareSize + x] = _regions[i].color;
						break;
					}
				}
			}
		}

		if (_drawMode == DrawMode.NoiseMap)
		{
			MapTexturizer.Instance.TextureNoiseMap2D(_noiseMap, _objToMap);
		}
		else if (_drawMode == DrawMode.ColorMap)
		{
			MapTexturizer.Instance.TextureColor(_colorMap, _mapSquareSize, _mapSquareSize, _objToMap);
		}
		else if (_drawMode == DrawMode.Mesh)
		{
			MapTexturizer.Instance.TextureMesh(MeshGenerator.Instance.GenerateTerrainMap(_noiseMap, _heightMultiplier, _heightCurve, _levelOfDetail), MapTexturizer.Instance.TextureColor(_colorMap, _mapSquareSize, _mapSquareSize, _objToMap), _meshFilter, _meshRenderer);
		}

	}

	[CustomEditor(typeof(MapGenerator))]
	public class MapGeneratorEditor : Editor
	{
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
