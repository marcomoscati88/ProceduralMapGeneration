using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	private int _triangleIndex;

	public MeshData(int _meshWidth, int _meshHeight)
	{
		vertices = new Vector3[_meshWidth * _meshHeight];
		uvs = new Vector2[_meshWidth * _meshHeight];
		triangles = new int[(_meshWidth - 1) * (_meshHeight - 1) * 6];
	}

	public void AddTriangle(int _a, int _b, int _c)
	{
		triangles[_triangleIndex] = _a;
		triangles[_triangleIndex + 1] = _b;
		triangles[_triangleIndex + 2] = _c;
		_triangleIndex += 3;
	}

	public Mesh CreateMesh()
	{
		Mesh _mesh = new Mesh();
		_mesh.vertices = vertices;
		_mesh.triangles = triangles;
		_mesh.uv = uvs;
		_mesh.RecalculateNormals();
		return _mesh;
	}

}
