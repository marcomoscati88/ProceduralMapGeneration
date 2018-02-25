using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EndlessTerrain))]
public class MapChunk
{

	private Vector2 position;
	private Vector3 _positionV3;
	private Bounds _bounds;
	private GameObject _go;

	public MapChunk(Vector2 _chunkCoordinates, int _chunkSize, Transform _parentTransform)
	{
		position = _chunkCoordinates * _chunkSize;
		_bounds = new Bounds(position, Vector2.one * _chunkSize);
		_positionV3 = new Vector3(position.x, 0, position.y);
		_go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		_go.transform.position = _positionV3;
		_go.transform.localScale = Vector3.one * _chunkSize / 10.0f;
		_go.transform.parent = _parentTransform;
		SetVisible(false);
	}

	public void UpdateVisibleChunk()
	{
		float _viewerDistanceFromEdge = Mathf.Sqrt(_bounds.SqrDistance(EndlessTerrain._viewerPosition));
		bool _isVisible = _viewerDistanceFromEdge <= EndlessTerrain.MAXVIEWDISTANCE;
		SetVisible(_isVisible);
	}

	public void SetVisible(bool _isVisible)
	{
		_go.SetActive(_isVisible);
	}

	public bool IsVisible(){
		return _go.activeSelf;
	}
}
