using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData
{
	public readonly float[,] noiseMap;
	public readonly Color[] colorMap;

	public MapData(float[,] _noiseMap, Color[] _colorMap)
	{
		this.noiseMap = _noiseMap;
		this.colorMap = _colorMap;
	}
}
