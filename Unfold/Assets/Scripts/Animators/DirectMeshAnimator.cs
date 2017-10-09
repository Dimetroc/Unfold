using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class DirectMeshAnimator : MeshAnimator
	{
		private readonly Vector3 _direction;
		private readonly bool _inverse;
		private readonly Vector3 _offset;

		public DirectMeshAnimator(List<SmartTriangle> smartTriangles, Vector3 direction, Vector3 offset, bool inverse)
		{
			SmartTriangles = smartTriangles;
			_direction = direction;
			_offset = offset;
			_inverse = inverse;
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			return GetAnimationValue(centroid, _direction.normalized);
		}

		public override void Start()
		{
			foreach (var st in SmartTriangles)
			{
				st.Setup(this);
				//st.PlaceToStartPosition(_offset);
			}
		}
	}
}
