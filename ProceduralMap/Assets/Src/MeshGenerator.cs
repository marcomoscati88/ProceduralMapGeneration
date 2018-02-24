using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

	private static MeshGenerator _instance;
	private MeshGenerator()
	{

	}

	public static MeshGenerator Instance
	{
		get
		{
			if (_instance == null) 
			{
				_instance = new MeshGenerator();
			}
			return _instance;
		}
	}


	public MeshData GenerateTerrainMap(float[,] _noiseMap, float _heightMultiplier, AnimationCurve _heightCurve, int _levelOfDetail)
	{
		int _width = _noiseMap.GetLength(0);
		int _height = _noiseMap.GetLength(1);
		float _topLeftX = (_width - 1) / -2.0f;
		float _topLeftZ = (_height - 1) / 2.0f;

		int _meshDetailValue = (_levelOfDetail == 0) ? 1 : _levelOfDetail * 2;
		int _verticesInline = (_width - 1) / _meshDetailValue + 1;

		MeshData _meshData = new MeshData(_verticesInline, _verticesInline);
		int _vertexIndex = 0;
		for (int y = 0; y < _height; y+=_meshDetailValue)
		{
			for (int x = 0; x < _width; x+=_meshDetailValue)
			{
				_meshData.vertices[_vertexIndex] = new Vector3(_topLeftX + x, _heightCurve.Evaluate(_noiseMap[x, y]) * _heightMultiplier, _topLeftZ - y);
				_meshData.uvs[_vertexIndex] = new Vector2(x / (float)_width, y / (float)_height);

				if (x < _width - 1 && y < _height - 1)
				{
					_meshData.AddTriange(_vertexIndex, _vertexIndex + _verticesInline + 1, _vertexIndex + _verticesInline);
					_meshData.AddTriange(_vertexIndex + _verticesInline + 1, _vertexIndex, _vertexIndex + 1);

				}

				_vertexIndex++;
			}
		}
		return _meshData;
	}
}
