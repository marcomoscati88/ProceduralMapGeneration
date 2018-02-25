using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

	public const float MAXVIEWDISTANCE = 400;
	[SerializeField]
	private Transform _viewer;

	public static Vector2 _viewerPosition;

	private int _chunkSize;
	private int _chunkVisible;
	private Dictionary<Vector2, MapChunk> _terrainChunkDictionary = new Dictionary<Vector2, MapChunk>();
	private List<MapChunk> _chunkVisibleLastUpdate = new List<MapChunk>();

	private void Start()
	{
		_chunkSize = MapGenerator.MAPCHUNKSIZE - 1;
		_chunkVisible = Mathf.RoundToInt(MAXVIEWDISTANCE / _chunkSize);
	}

	private void Update()
	{
		_viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);
		_CheckVisibleChunks();
	}

	private void _CheckVisibleChunks()
	{
		for (int i = 0; i < _chunkVisibleLastUpdate.Count; i++)
		{
			_chunkVisibleLastUpdate[i].SetVisible(false);
		}
		_chunkVisibleLastUpdate.Clear();

		int _currentXCoordChunk = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
		int _currentYCoordChunk = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

		for (int yOffset = -_chunkVisible; yOffset <= _chunkVisible; yOffset++)
		{
			for (int xOffset = -_chunkVisible; xOffset < _chunkVisible; xOffset++)
			{
				Vector2 _viewChunkCoord = new Vector2(_currentXCoordChunk + xOffset, _currentYCoordChunk + yOffset);
				if (_terrainChunkDictionary.ContainsKey(_viewChunkCoord)) {
					_terrainChunkDictionary[_viewChunkCoord].UpdateVisibleChunk();
					if (_terrainChunkDictionary[_viewChunkCoord].IsVisible())
					{
						_chunkVisibleLastUpdate.Add(_terrainChunkDictionary[_viewChunkCoord]);
					}
				} else {
					_terrainChunkDictionary.Add(_viewChunkCoord, new MapChunk(_viewChunkCoord, _chunkSize, this.transform));
				}
			}
		}
	}
}
