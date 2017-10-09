using UnityEngine;

namespace Unfold
{
	public class DirectMeshAnimator : MeshAnimator
	{
		public Vector3 _direction;
		private TrianglesStorage _storage;

		public DirectMeshAnimator(Vector3 direction)
		{
			_direction = direction;
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			var dir = _direction.normalized;
			var projection = Vector3.Project(centroid, dir);
			var projectionMag = projection.magnitude;
			if (Vector3.Dot(projection, dir) < 0)
			{
				projectionMag *= -1;
				if (projectionMag < Min)
				{
					Min = projectionMag;
				}
			}
			else
			{
				if (projectionMag > Max)
				{
					Max = projectionMag;
				}
			}
			return projectionMag;
		}
	}
}
