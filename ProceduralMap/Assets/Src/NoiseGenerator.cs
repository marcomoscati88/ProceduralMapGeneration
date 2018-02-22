using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {

	public static float[,] GenerateNoiseMap(int _mapWidth, int _mapHeight, float _noiseMagnitude) {
		float[,] _noiseMap = new float[_mapWidth, _mapHeight];

		if (_noiseMagnitude <= 0)
		{
			_noiseMagnitude = 0.0001f;
		}

		for (int y = 0; y < _mapHeight; y++)
		{
			for (int x = 0; x < _mapHeight; x++)
			{
				float _xCoordinate = x / _noiseMagnitude;
				float _yCoordinate = y / _noiseMagnitude;

				float _perlinValue = Mathf.PerlinNoise(_xCoordinate, _yCoordinate);
				_noiseMap[x, y] = _perlinValue;
			}
		}

		return _noiseMap;
	}
}
