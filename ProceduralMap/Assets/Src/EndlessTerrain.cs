using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MapGenerator))]
public class EndlessTerrain : MonoBehaviour
{

	public static float maxViewDistance = 400;
	public static Vector2 viewerPosition;

	public Transform viewer;
	public Material meshMaterial;
	public LODInfo[] detailLevels;

	private static MapGenerator _generator;

	private const float _MOVETHREASHOLDFORUPDATE = 25.0f;
	private float _sqrMoveThreashold = _MOVETHREASHOLDFORUPDATE * _MOVETHREASHOLDFORUPDATE;
	private int _mapChunksInViewDistance;
	private int _mapChunkSize;
	private Vector2 _oldViewerPosition;
	private Dictionary<Vector2, MapChunk> _mapChunkDictionary = new Dictionary<Vector2, MapChunk>();
	private List<MapChunk> _lastVisibleChunks = new List<MapChunk>();

	private void Start()
	{
		_generator = FindObjectOfType<MapGenerator>();

		maxViewDistance = detailLevels[detailLevels.Length - 1].distanceThreashold;
		_mapChunkSize = MapGenerator.MAPCHUNKSIZE - 1;
		_mapChunksInViewDistance = Mathf.RoundToInt(maxViewDistance / _mapChunkSize);
		UpdateVisibleChunks();
	}

	private void Update()
	{
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		if ((_oldViewerPosition - viewerPosition).sqrMagnitude > _sqrMoveThreashold)
		{
			_oldViewerPosition = viewerPosition;
			UpdateVisibleChunks();
		}
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
					_mapChunkDictionary.Add(_viewedChunkCoord, new MapChunk(_viewedChunkCoord, _mapChunkSize, detailLevels, transform, meshMaterial));
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
		private LODInfo[] _detailLevels;
		private LODetail[] _meshLod;
		private MapData _mapData;
		private int _previousLodIndex = -1;
		private bool _hasReceivedMapData;

		public MapChunk(Vector2 _coordinates, int _size, LODInfo[] _lodLevels, Transform _parent, Material _material)
		{
			_positionV2 = _coordinates * _size;
			_bounds = new Bounds(_positionV2, Vector2.one * _size);
			Vector3 _positionV3 = new Vector3(_positionV2.x, 0, _positionV2.y);
			_detailLevels = _lodLevels;
			_go = new GameObject("Terrain Chunk");
			_meshRenderer = _go.AddComponent<MeshRenderer>();
			_meshFilter = _go.AddComponent<MeshFilter>();
			_meshRenderer.material = _material;

			_go.transform.position = _positionV3;
			_go.transform.parent = _parent;
			SetVisible(false);
			_meshLod = new LODetail[_detailLevels.Length];
			for (int i = 0; i < _detailLevels.Length; i++)
			{
				_meshLod[i] = new LODetail(_detailLevels[i].lod, UpdateTerrainChunk);
			}
			_generator.RequestMapData(_positionV2, OnMapDataReceived);
		}

		private void OnMapDataReceived(MapData _mapData)
		{
			this._mapData = _mapData;
			_hasReceivedMapData = true;
			Texture2D _texture = TextureGenerator.Instance.TextureFromColourMap(_mapData.colorMap, MapGenerator.MAPCHUNKSIZE, MapGenerator.MAPCHUNKSIZE);
			_meshRenderer.material.mainTexture = _texture;
			UpdateTerrainChunk();
		}

		public void UpdateTerrainChunk()
		{
			if (_hasReceivedMapData)
			{
				float _distanceFromNearEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
				bool _isVisible = _distanceFromNearEdge <= maxViewDistance;

				if (_isVisible)
				{
					int _lodIndex = 0;
					for (int i = 0; i < _detailLevels.Length - 1; i++)
					{
						if (_distanceFromNearEdge > _detailLevels[i].distanceThreashold)
						{
							_lodIndex = i + 1;
						}
						else
						{
							break;
						}
					}

					if (_lodIndex != _previousLodIndex)
					{
						LODetail _lodMesh = _meshLod[_lodIndex];
						if (_lodMesh.hasMesh)
						{
							_meshFilter.mesh = _lodMesh.mesh;
							_previousLodIndex = _lodIndex;
						}
						else if (!_lodMesh.hasRequestedMesh)
						{
							_lodMesh.RequestMesh(_mapData);
						}
					}
				}

				SetVisible(_isVisible);
			}
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

	public class LODetail
	{
		public bool hasMesh;
		public bool hasRequestedMesh;
		public Mesh mesh;
		System.Action _updateCallBack;

		private int _levelOfDetailValue;

		public LODetail(int _lod, System.Action _callBack)
		{
			_levelOfDetailValue = _lod;
			_updateCallBack = _callBack;
		}

		public void RequestMesh(MapData _mapData)
		{
			hasRequestedMesh = true;
			_generator.RequestMeshData(_mapData, _levelOfDetailValue, OnMeshDataReceived);
		}

		private void OnMeshDataReceived(MeshData _meshData)
		{
			mesh = _meshData.CreateMesh();
			hasMesh = true;

			_updateCallBack();
		}
	}
}
