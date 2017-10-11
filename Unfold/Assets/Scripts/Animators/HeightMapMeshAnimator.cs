using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class HeightMapMeshAnimator : MeshAnimator
	{
		private readonly Vector3 _center;
		private readonly bool _unfold;
		private readonly MeshFilter _meshFilter;
		private readonly Texture2D _heightMap;

		public HeightMapMeshAnimator(List<SmartTriangle> smartTriangles, Texture2D heightMap, bool unfold, MeshFilter meshFilter)
		{
			SmartTriangles = smartTriangles;
			_center = new Vector3(0, 0, 0);
			_unfold = unfold;
			_heightMap = heightMap;
			_meshFilter = meshFilter;
		}

		public override void Start()
		{
			if (!_unfold)
			{
				_meshFilter.mesh.Clear();
			}
			foreach (var st in SmartTriangles)
			{
				st.Setup(this, !_unfold);
				if (_unfold)
				{
					st.Show();
					st.ChangeTargetPosition(new Vector3(0, 1, 0));
				}
				else
				{
					st.ChangeCurrentPosition(new Vector3(0, 1, 0));
				}
			}
		}

		public override void End()
		{
			if (_unfold)
			{
				_meshFilter.mesh.Clear();
			}
		}

		private float Linear(float x, float x0, float x1, float y0, float y1)
		{
			if ((x1 - x0) == 0)
			{
				return (y0 + y1) / 2;
			}
			return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			var valueRight = GetAnimationValue(centroid, new Vector3(1, 0, 0).normalized);
			var tWidth = Linear(valueRight, MinValue, MaxValue, 0, _heightMap.width);
			var valueTop = GetAnimationValue(centroid, new Vector3(0, 1, 0).normalized);
			var tHeight = Linear(valueTop, MinValue, MaxValue, 0, _heightMap.height);
			var grayscale = _heightMap.GetPixel((int)tWidth, (int)tHeight).grayscale;
			return MaxValue + grayscale * (MinValue - MaxValue);
		}
	}
}
