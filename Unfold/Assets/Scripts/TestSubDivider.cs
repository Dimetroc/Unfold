using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TestSubDivider : MonoBehaviour
{
    private Mesh _startMesh;

    private int _trianglesCount = 0;
    private int _verticesCount = 0;

    private Vector3[] _newVertices;
    private Vector3[] _newNormals;
    private Vector2[] _newUvs;
    private int[] _newTriangles;

    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _startMesh = _meshFilter.mesh;

        //PrintMesh(_startMesh);

        GenerateDataHolders();

        SubDivide();
    }

    private void GenerateDataHolders()
    {
        _trianglesCount = _startMesh.triangles.Length / 3;
        _verticesCount = _trianglesCount * 12 ;

        _newVertices = new Vector3[_verticesCount];
        _newTriangles = new int[_verticesCount];
        _newNormals = new Vector3[_verticesCount];
        _newUvs = new Vector2[_verticesCount];
    }

    private void PrintMesh(Mesh mesh)
    {
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var uv = mesh.uv;
        var normals = mesh.normals;

        foreach (var vertex in vertices)
        {
            Debug.Log(vertex);
        }

        foreach (var triangle in triangles)
        {
            Debug.Log(triangle);
        }
    }

    private void SubDivide()
    {
        var vertices = _startMesh.vertices;
        var triangles = _startMesh.triangles;
        var uv = _startMesh.uv;
        var normals = _startMesh.normals;

        var trianglesCount = 0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            SubDivideTriangles(trianglesCount);
            SubDivideArray(trianglesCount, triangles, vertices, ref _newVertices);
            SubDivideArray(trianglesCount, triangles, normals, ref _newNormals);
            SubDivideArray(trianglesCount, triangles, uv, ref _newUvs);
            trianglesCount++;
        }

        var mesh = new Mesh
        {
            vertices = _newVertices,
            normals = _newNormals,
            uv = _newUvs,
            triangles = _newTriangles
        };

        _meshFilter.mesh = mesh;
    }

    private void SubDivideTriangles(int index)
    {
        for (int i = 0; i < 12; i++)
        {
            _newTriangles[index * 12 + i] = index * 12 + i;
        }
    }


    private void SubDivideArray(int index, int[] triangles, Vector3[] source, ref Vector3[] target)
    {
        var v0 = source[triangles[index * 3 + 0]];
        var v1 = source[triangles[index * 3 + 1]];
        var v2 = source[triangles[index * 3 + 2]];
        var va = (v0 + v1) / 2.0f;
        var vb = (v1 + v2) / 2.0f;
        var vc = (v2 + v0) / 2.0f;



        target[index * 12 + 0] = v0;
        target[index * 12 + 1] = va;
        target[index * 12 + 2] = vc;

        target[index * 12 + 3] = vc;
        target[index * 12 + 4] = vb;
        target[index * 12 + 5] = v2;

        target[index * 12 + 6] = va;
        target[index * 12 + 7] = vb;
        target[index * 12 + 8] = vc;

        target[index * 12 + 9] = va;
        target[index * 12 + 10] = v1;
        target[index * 12 + 11] = vb;

    }

    private void SubDivideArray(int index, int[] triangles, Vector2[] source, ref Vector2[] target)
    {
        var v0 = source[triangles[index * 3 + 0]];
        var v1 = source[triangles[index * 3 + 1]];
        var v2 = source[triangles[index * 3 + 2]];
        var va = (v0 + v1) / 2.0f;
        var vb = (v1 + v2) / 2.0f;
        var vc = (v2 + v0) / 2.0f;

        target[index * 12 + 0] = v0;
        target[index * 12 + 1] = va;
        target[index * 12 + 2] = vc;

        target[index * 12 + 3] = vc;
        target[index * 12 + 4] = vb;
        target[index * 12 + 5] = v2;

        target[index * 12 + 6] = va;
        target[index * 12 + 7] = vb;
        target[index * 12 + 8] = vc;

        target[index * 12 + 9] = va;
        target[index * 12 + 10] = v1;
        target[index * 12 + 11] = vb;

    }
}
