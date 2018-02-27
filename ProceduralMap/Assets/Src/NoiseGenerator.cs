using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
	private static readonly NoiseGenerator _instance = new NoiseGenerator();

	static NoiseGenerator()
	{
	
	}

	private NoiseGenerator()
	{

	}

	public static NoiseGenerator Instance{
		get
		{
			return _instance;
		}
	}

	public float[,] GenerateNoiseMap(int _mapWidth, int _mapHeight, int _seed, float _scale, int _layers, float _persistance, float _lacunarity, Vector2 _offset)
	{
		float[,] _noiseMap = new float[_mapWidth, _mapHeight];

		System.Random _random = new System.Random(_seed);
		Vector2[] _layerOffset = new Vector2[_layers];
		for (int i = 0; i < _layers; i++)
		{
			float _offsetX = _random.Next(-100000, 100000) + _offset.x;
			float _offsetY = _random.Next(-100000, 100000) + _offset.y;
			_layerOffset[i] = new Vector2(_offsetX, _offsetY);
		}

		if (_scale <= 0)
		{
			_scale = 0.0001f;
		}

		float _maxNoiseHeight = float.MinValue;
		float _minNoiseHeight = float.MaxValue;

		float _hWidth = _mapWidth / 2f;
		float _hHeight = _mapHeight / 2f;


		for (int y = 0; y < _mapHeight; y++)
		{
			for (int x = 0; x < _mapWidth; x++)
			{

				float _amplitude = 1;
				float _frequency = 1;
				float _noiseHeight = 0;

				for (int i = 0; i < _layers; i++)
				{
					float _varX = (x - _hWidth) / _scale * _frequency + _layerOffset[i].x;
					float _varY = (y - _hHeight) / _scale * _frequency + _layerOffset[i].y;

					float _perlinValue = Mathf.PerlinNoise(_varX, _varY) * 2 - 1;
					_noiseHeight += _perlinValue * _amplitude;

					_amplitude *= _persistance;
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