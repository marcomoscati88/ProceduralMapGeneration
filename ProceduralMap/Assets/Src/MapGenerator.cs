using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
	public DrawMode drawMode;

	public const int MAPCHUNKSIZE = 241;
	[Range(0, 6)]
	public int meshSemplification;
	[Range(1,100)]
	public float noiseZoom;
	[Range(1,5)]
	public int layers;
	[Range(0, 1)]
	public float persistance;
	[Range(1,10)]
	public float lacunarity;

	public int seed;
	public Vector2 offset;
	[Range(1,100)]
	public float meshNoiseMultiplier;
	public AnimationCurve meshNoiseCurve;

	public bool autoUpdate;

	public TerrainType[] regions;

	private Queue<MapThreadInfo<MapData>> _mapThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	private Queue<MapThreadInfo<MeshData>> _meshThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	public void DrawMapInEditor()
	{
		MapData _mapData = GenerateMapData();

		DisplayMap _display = FindObjectOfType<DisplayMap>();
		if (drawMode == DrawMode.NoiseMap)
		{
			_display.DrawTexture(TextureGenerator.Instance.TextureFromHeightMap(_mapData.noiseMap));
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			_display.DrawTexture(TextureGenerator.Instance.TextureFromColourMap(_mapData.colorMap, MAPCHUNKSIZE, MAPCHUNKSIZE));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			_display.DrawMesh(MeshGenerator.Instance.GenerateTerrainMesh(_mapData.noiseMap, meshNoiseMultiplier, meshNoiseCurve, meshSemplification), TextureGenerator.Instance.TextureFromColourMap(_mapData.colorMap, MAPCHUNKSIZE, MAPCHUNKSIZE));
		}
	}

	public void RequestMapData(Action<MapData> _callBack)
	{
		ThreadStart _threadStart = delegate
		{
			MapDataThread(_callBack);
		};

		new Thread(_threadStart).Start();
	}

	private void MapDataThread(Action<MapData> _callBack)
	{
		MapData _mapData = GenerateMapData();
		lock (_mapThreadInfoQueue)
		{
			_mapThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(_callBack, _mapData));
		}
	}

	public void RequestMeshData(MapData _mapData, Action<MeshData> _callBack)
	{
		ThreadStart _threadStart = delegate
		{
			MeshDataThread(_mapData, _callBack);
		};

		new Thread(_threadStart).Start();
	}

	private void MeshDataThread(MapData _mapData, Action<MeshData> _callBack)
	{
		MeshData _meshData = MeshGenerator.Instance.GenerateTerrainMesh(_mapData.noiseMap, meshNoiseMultiplier, meshNoiseCurve, meshSemplification);
		lock (_meshThreadInfoQueue)
		{
			_meshThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(_callBack, _meshData));
		}
	}

	private void Update()
	{
		if (_mapThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < _mapThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MapData> _threadInfo = _mapThreadInfoQueue.Dequeue();
				_threadInfo.callBack(_threadInfo.parameter);
			}
		}

		if (_meshThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < _meshThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MeshData> _threadInfo = _meshThreadInfoQueue.Dequeue();
				_threadInfo.callBack(_threadInfo.parameter);
			}
		}
	}

	private MapData GenerateMapData()
	{
		float[,] _noiseMap = NoiseGenerator.Instance.GenerateNoiseMap(MAPCHUNKSIZE, MAPCHUNKSIZE, seed, noiseZoom, layers, persistance, lacunarity, offset);

		Color[] _colorMap = new Color[MAPCHUNKSIZE * MAPCHUNKSIZE];
		for (int y = 0; y < MAPCHUNKSIZE; y++)
		{
			for (int x = 0; x < MAPCHUNKSIZE; x++)
			{
				float _currentVNoise = _noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
				{
					if (_currentVNoise <= regions[i].height)
					{
						_colorMap[y * MAPCHUNKSIZE + x] = regions[i].colour;
						break;
					}
				}
			}
		}


		return new MapData(_noiseMap, _colorMap);
	}
}

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MapGenerator _generator = (MapGenerator)target;

		if (DrawDefaultInspector())
		{
			if (_generator.autoUpdate)
			{
				_generator.DrawMapInEditor();
			}
		}

		if (GUILayout.Button("Generate"))
		{
			_generator.DrawMapInEditor();
		}
	}
}
