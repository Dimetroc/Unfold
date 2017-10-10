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
					//st.ChangeTargetPosition(_offset);
				}
				else
				{
					//st.ChangeCurrentPosition(_offset);
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

		public override float GetAnimationValue(Vector3 centroid)
		{
			var magtinude = (centroid - _center).magnitude;
			for (int i = 0; i < _heightMap.width; i++)
			{
				for (int j = 0; j < _heightMap.height; j++)
				{
					if (magtinude > (i + j)/100f)
					{
						return _heightMap.GetPixel(i, j).grayscale;
					}
				}
			}
			return 1;
			//if (_toCenter)
			{
				return GetAnimationValue(centroid, (_center - centroid).normalized);
			}
			return GetAnimationValue(centroid, (centroid - _center).normalized);
		}
	}
}
