using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{

	public Vector3[] vertices;
	public Vector2[] uvs;
	public int[] triangles;

	private int _trianglesNumber;

	public MeshData(int _meshWidth, int _meshHeight)
	{
		vertices = new Vector3[_meshWidth * _meshHeight];
		uvs = new Vector2[_meshWidth * _meshHeight];
		triangles = new int[(_meshWidth - 1) * (_meshHeight - 1) * 6];
	}

	public void AddTriange(int _a, int _b, int _c)
	{
		triangles[_trianglesNumber] = _a;
		triangles[_trianglesNumber + 1] = _b;
		triangles[_trianglesNumber + 2] = _c;
		_trianglesNumber += 3;
	}

	public Mesh CreateMesh(){
		Mesh _mesh = new Mesh();
		_mesh.vertices = vertices;
		_mesh.triangles = triangles;
		_mesh.uv = uvs;
		_mesh.RecalculateNormals();
		return _mesh;
	}
}
