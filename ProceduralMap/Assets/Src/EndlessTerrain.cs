using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

	public const float maxViewDst = 400;
	public Transform viewer;
	public Material meshMaterial;

	public static Vector2 viewerPosition;

	private int _mapChunksInViewDistance;
	private int _mapChunkSize;
	private static MapGenerator _generator;
	private Dictionary<Vector2, MapChunk> _mapChunkDictionary = new Dictionary<Vector2, MapChunk>();
	private List<MapChunk> _lastVisibleChunks = new List<MapChunk>();

	private void Start()
	{
		_generator = FindObjectOfType<MapGenerator>();
		_mapChunkSize = MapGenerator.MAPCHUNKSIZE - 1;
		_mapChunksInViewDistance = Mathf.RoundToInt(maxViewDst / _mapChunkSize);
	}

	private void Update()
	{
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}

	private void UpdateVisibleChunks()
	{

		for (int i = 0; i < _lastVisibleChunks.Count; i++)
		{
			_lastVisibleChunks[i].SetVisible(false);
		}
		_lastVisibleChunks.Clear();

		int _currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / _mapChunkSize);
		int _currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / _mapChunkSize);

		for (int yOffset = -_mapChunksInViewDistance; yOffset <= _mapChunksInViewDistance; yOffset++)
		{
			for (int xOffset = -_mapChunksInViewDistance; xOffset <= _mapChunksInViewDistance; xOffset++)
			{
				Vector2 _viewedChunkCoord = new Vector2(_currentChunkCoordX + xOffset, _currentChunkCoordY + yOffset);

				if (_mapChunkDictionary.ContainsKey(_viewedChunkCoord))
				{
					_mapChunkDictionary[_viewedChunkCoord].UpdateTerrainChunk();
					if (_mapChunkDictionary[_viewedChunkCoord].IsVisible())
					{
						_lastVisibleChunks.Add(_mapChunkDictionary[_viewedChunkCoord]);
					}
				}
				else
				{
					_mapChunkDictionary.Add(_viewedChunkCoord, new MapChunk(_viewedChunkCoord, _mapChunkSize, transform, meshMaterial));
				}

			}
		}
	}

	public class MapChunk
	{

		private GameObject _go;
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private Vector2 _positionV2;
		private Bounds _bounds;


		public MapChunk(Vector2 _coordinates, int _size, Transform _parent, Material _material)
		{
			_positionV2 = _coordinates * _size;
			_bounds = new Bounds(_positionV2, Vector2.one * _size);
			Vector3 _positionV3 = new Vector3(_positionV2.x, 0, _positionV2.y);

			_go = new GameObject("Terrain Chunk");
			_meshRenderer = _go.AddComponent<MeshRenderer>();
			_meshFilter = _go.AddComponent<MeshFilter>();
			_meshRenderer.material = _material;

			_go.transform.position = _positionV3;
			_go.transform.parent = _parent;
			SetVisible(false);

			_generator.RequestMapData(OnMapDataReceived);
		}

		private void OnMapDataReceived(MapData _mapData)
		{
			_generator.RequestMeshData(_mapData, OnMeshDataReceived);
		}

		private void OnMeshDataReceived(MeshData _meshData)
		{
			_meshFilter.mesh = _meshData.CreateMesh();
		}


		public void UpdateTerrainChunk()
		{
			float _distanceFromNearEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
			bool _isVisible = _distanceFromNearEdge <= maxViewDst;
			SetVisible(_isVisible);
		}

		public void SetVisible(bool _isVisible)
		{
			_go.SetActive(_isVisible);
		}

		public bool IsVisible()
		{
			return _go.activeSelf;
		}

	}
}
