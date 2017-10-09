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
			T1 = data.Triangles[startIndex + 1];
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

		public TriangleData(int startIndex)
		{
			T0 = startIndex;
			T1 = startIndex + 1;
			T2 = startIndex + 2;

			V0 = V1 = V2 = Vector3.zero;

			N0 = N1 = N2 = Vector3.zero;

			Uv0 = Uv1 = Uv2 = Vector2.zero;
		}

		public void UpdateData(TriangleData data)
		{
			V0 = data.V0;
			V1 = data.V1;
			V2 = data.V2;

			N0 = data.N0;
			N1 = data.N1;
			N2 = data.N2;

			Uv0 = data.Uv0;
			Uv1 = data.Uv1;
			Uv2 = data.Uv2;
		}

		public void UpdateVertices(TriangleData triangleData)
		{
			V0 = triangleData.V0;
			V1 = triangleData.V1;
			V2 = triangleData.V2;
		}

		public void ClearData()
		{
			V0 = V1 = V2 = Vector3.zero;

			N0 = N1 = N2 = Vector3.zero;

			Uv0 = Uv1 = Uv2 = Vector2.zero;
		}

		public void UpdateTriangleIndex(int startIndex)
		{
			T0 = startIndex;
			T1 = startIndex + 1;
			T2 = startIndex + 2;
		}

		public TriangleData AddVector3(Vector3 vector)
		{
			V0 += vector;
			V1 += vector;
			V2 += vector;
			return this;
		}

		public void Lerp(TriangleData to, float fraction)
		{
			V0 = Vector3.Lerp(V0, to.V0, fraction);
			V1 = Vector3.Lerp(V1, to.V1, fraction);
			V2 = Vector3.Lerp(V2, to.V2, fraction);
		}

		public Vector3 GetCentroid()
		{
			return new Vector3((V0.x + V1.x + V2.x) / 3.0f, (V0.y + V1.y + V2.y) / 3.0f, (V0.z + V1.z + V2.z) / 3.0f);
		}

		public float GetArea()
		{
			var a = (V0 - V1).magnitude;
			var b = (V1 - V2).magnitude;
			var c = (V2 - V0).magnitude;
			var s = (a + b + c) / 2.0f;
			return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
		}

		public override string ToString()
		{
			return "V0: " + V0 + " V1: " + V1 + " V2: " + V2;
		}

		public void SetVector(Vector3 vector)
		{
			V0 = V1 = V2 = vector;
		}

		public static TriangleData operator +(TriangleData left, Vector3 right)
		{
			return left.AddVector3(right);
		}
	}
}
