using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MapTexturizer))]
public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	private int _mapWidth;
	[SerializeField]
	private int _mapHeight;
	[SerializeField]
	private float _noiseScale;
	[SerializeField]
	private bool _autoUpdate;

	public void GenerateMap()
	{
		float[,] _noiseMap = NoiseGenerator.Instance.GenerateNoiseMap(_mapWidth, _mapHeight, _noiseScale);

		MapTexturizer _mapTexture = this.gameObject.GetComponent<MapTexturizer>();
		_mapTexture.DrawNoiseMap2D(_noiseMap);
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
