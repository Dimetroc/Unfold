using System.Collections;
using Unfold;
using UnityEngine;

namespace Unfold
{
    public struct TriangleData
    {
        public int T0;
        public int T1;
        public int T2;

        public Vector3 V0;
        public Vector3 V1;
        public Vector3 V2;

        public Vector3 N0;
        public Vector3 N1;
        public Vector3 N2;

        public Vector2 Uv0;
        public Vector2 Uv1;
        public Vector2 Uv2;


        public TriangleData(MeshData data, int startIndex)
        {
            T0 = data.Triangles[startIndex];
            T1 = data.Triangles[startIndex +1];
            T2 = data.Triangles[startIndex + 2];

            V0 = data.Vertices[T0];
            V1 = data.Vertices[T1];
            V2 = data.Vertices[T2];

            N0 = data.Normals[T0];
            N1 = data.Normals[T1];
            N2 = data.Normals[T2];

            Uv0 = data.Uvs[T0];
            Uv1 = data.Uvs[T1];
            Uv2 = data.Uvs[T2];
        }

        public void UpdateTriangleIndex(int startIndex)
        {
            T0 = startIndex;
            T1 = startIndex + 1;
            T2 = startIndex + 2;
        }
    }
}
