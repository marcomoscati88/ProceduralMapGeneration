using UnityEngine;
using System.Collections;

public class MeshGenerator
{
	private static readonly MeshGenerator _instance = new MeshGenerator();

	static MeshGenerator()
	{

	}

	private MeshGenerator()
	{

	}

	public static MeshGenerator Instance
	{
		get
		{
			return _instance;
		}
	}

	public MeshData GenerateTerrainMesh(float[,] _noiseMap, float _noiseMultiplier, AnimationCurve _noiseCurve, int _meshSemplification)
	{
		AnimationCurve heightCurve = new AnimationCurve(_noiseCurve.keys);

		int _width = _noiseMap.GetLength(0);
		int _height = _noiseMap.GetLength(1);
		float _topLeftX = (_width - 1) / -2f;
		float _topLeftZ = (_height - 1) / 2f;

		int _meshSemplificationIncrement = (_meshSemplification == 0) ? 1 : _meshSemplification * 2;
		int _verticesInLine = (_width - 1) / _meshSemplificationIncrement + 1;

		MeshData _meshData = new MeshData(_verticesInLine, _verticesInLine);
		int _vertexIndex = 0;

		for (int y = 0; y < _height; y += _meshSemplificationIncrement)
		{
			for (int x = 0; x < _width; x += _meshSemplificationIncrement)
			{
				_meshData.vertices[_vertexIndex] = new Vector3(_topLeftX + x, heightCurve.Evaluate(_noiseMap[x, y]) * _noiseMultiplier, _topLeftZ - y);
				_meshData.uvs[_vertexIndex] = new Vector2(x / (float)_width, y / (float)_height);

				if (x < _width - 1 && y < _height - 1)
				{
					_meshData.AddTriangle(_vertexIndex, _vertexIndex + _verticesInLine + 1, _vertexIndex + _verticesInLine);
					_meshData.AddTriangle(_vertexIndex + _verticesInLine + 1, _vertexIndex, _vertexIndex + 1);
				}

				_vertexIndex++;
			}
		}

		return _meshData;
	}
}