using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
    public class MeshData
    {
        public List<int> Triangles { get; private set; }
        public List<Vector3> Vertices { get; private set; }
        public List<Vector3> Normals { get; private set; }
        public List<Vector2> Uvs { get; private set; }

        private int _index = -1;
        
        public MeshData()
        {
            Triangles = new List<int>();
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Uvs = new List<Vector2>();
        }

        public MeshData(Mesh mesh)
        {
            Triangles = new List<int>(mesh.triangles);
            Vertices = new List<Vector3>(mesh.vertices);
            Normals = new List<Vector3>(mesh.normals);
            Uvs = new List<Vector2>(mesh.uv);
        }

        public int GetStartIndex()
        {
            return Triangles.Count;
        }

        public void AddTriangle(TriangleData triangle)
        {
            Triangles.Add(triangle.T0);
            Triangles.Add(triangle.T1);
            Triangles.Add(triangle.T2);

            Vertices.Add(triangle.V0);
            Vertices.Add(triangle.V1);
            Vertices.Add(triangle.V2);

            Normals.Add(triangle.N0);
            Normals.Add(triangle.N1);
            Normals.Add(triangle.N2);

            Uvs.Add(triangle.Uv0);
            Uvs.Add(triangle.Uv1);
            Uvs.Add(triangle.Uv2);
		}

        public void UpdateTriangleData(TriangleData triangle)
        {
            Vertices[triangle.T0] = triangle.V0;
            Vertices[triangle.T1] = triangle.V1;
            Vertices[triangle.T2] = triangle.V2;

            Normals[triangle.T0] = triangle.N0;
            Normals[triangle.T1] = triangle.N1;
            Normals[triangle.T2] = triangle.N2;

            Uvs[triangle.T0] = triangle.Uv0;
            Uvs[triangle.T1] = triangle.Uv1;
            Uvs[triangle.T2] = triangle.Uv2;
        }

        public void UpdateTriangleVertices(TriangleData triangle)
        {
            Vertices[triangle.T0] = triangle.V0;
            Vertices[triangle.T1] = triangle.V1;
            Vertices[triangle.T2] = triangle.V2;
        }

        public void UpdateZeroedTriangleVertices(TriangleData triangle)
        {
            Vertices[triangle.T0] = Vector3.zero;
            Vertices[triangle.T1] = Vector3.zero;
            Vertices[triangle.T2] = Vector3.zero;
        }
    }
}
