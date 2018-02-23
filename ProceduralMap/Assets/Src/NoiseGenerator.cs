using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{

	private static NoiseGenerator _instance = null;

	private NoiseGenerator()
	{

	}

	public static NoiseGenerator Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NoiseGenerator();
			}
			return _instance;
		}
	}

	public float[,] GenerateNoiseMap(int _mapWidth, int _mapHeight, int _seed, float _noiseMagnitude, int _layerNumber, float _detailLevel, float _lacunarity, Vector2 _offset)
	{
		float[,] _noiseMap = new float[_mapWidth, _mapHeight];

		System.Random _prng = new System.Random(_seed);
		Vector2[] _layerOffsets = new Vector2[_layerNumber];
		for (int i = 0; i < _layerNumber; i++)
		{
			float _offsetX = _prng.Next(-100000, 100000) + _offset.x;
			float _offsetY = _prng.Next(-100000, 100000) + _offset.y;
			_layerOffsets[i] = new Vector2(_offsetX, _offsetY);
		}

		if (_noiseMagnitude <= 0)
		{
			_noiseMagnitude = 0.0001f;
		}

		float _minNoiseHeight = float.MaxValue;
		float _maxNoiseHeight = float.MinValue;

		float _halfWidth = _mapWidth / 2.0f;
		float _halfHeight = _mapHeight / 2.0f;

		for (int y = 0; y < _mapHeight; y++)
		{
			for (int x = 0; x < _mapHeight; x++)
			{
				float _amplitude = 1.0f;
				float _frequency = 1.0f;
				float _noiseHeight = 0.0f;

				for (int i = 0; i < _layerNumber; i++)
				{
					float _xCoordinate = (x - _halfWidth) / _noiseMagnitude * _frequency + _layerOffsets[i].x;
					float _yCoordinate = (y - _halfHeight) / _noiseMagnitude * _frequency + _layerOffsets[i].y;
					float _perlinValue = Mathf.PerlinNoise(_xCoordinate, _yCoordinate) * 2 - 1;

					_noiseHeight += _perlinValue * _amplitude;
					_amplitude *= _detailLevel;
					_frequency *= _lacunarity;
				}

				if (_noiseHeight > _maxNoiseHeight)
				{
					_maxNoiseHeight = _noiseHeight;
				}
				else if (_noiseHeight < _minNoiseHeight)
				{
					_minNoiseHeight = _noiseHeight;
				}


				_noiseMap[x, y] = _noiseHeight;
			}
		}

		//Cicle again to normalize the Noisemap
		for (int y = 0; y < _mapHeight; y++)
		{
			for (int x = 0; x < _mapWidth; x++)
			{
				_noiseMap[x, y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _noiseMap[x, y]);
			}
		}

		return _noiseMap;
	}
}
