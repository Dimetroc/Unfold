using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class RadialMeshAnimator : MeshAnimator
	{
		private readonly bool _toCenter;
		private readonly Vector3 _center;

		public RadialMeshAnimator(List<SmartTriangle> smartTriangles, bool toCenter, TrianglesStorage pool)
		{
			SmartTriangles = smartTriangles;
			_toCenter = toCenter;
			_center = pool.GetCenter();
		}

		public override void Start()
		{
			foreach (var st in SmartTriangles)
			{
				st.Setup(this);
				//st.PlaceToStartPosition(_offset);
			}
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			var projectionMag = centroid.magnitude;
			MinValue = 0;
			if (projectionMag > MaxValue) MaxValue = projectionMag;

			return projectionMag;
			if (_toCenter)
			{
				return GetAnimationValue(centroid, (_center - centroid).normalized);
			}
			return GetAnimationValue(centroid, (centroid - _center).normalized);
		}
	}
}
