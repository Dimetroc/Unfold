using UnityEngine;

namespace Unfold
{
	public class RadialMeshAnimator : MeshAnimator
	{
		public Vector3 _direction;

		public RadialMeshAnimator(Vector3 direction)
		{
			_direction = direction;
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			var projectionMag = centroid.magnitude;
			Min = 0;
			if (projectionMag > Max) Max = projectionMag;

			return projectionMag;
		}
	}
}
