using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public enum AnimationType
	{
		Direct,
		Radial,
		Random
	}

	public abstract class MeshAnimator
	{
		protected List<SmartTriangle> SmartTriangles;

		public float MaxValue { get; protected set; }
		public float MinValue { get; protected set; }

		public abstract void Start();

		public abstract void End();

		public abstract float GetAnimationValue(Vector3 centroid);

		protected float GetAnimationValue(Vector3 centroid, Vector3 direction)
		{
			var projection = Vector3.Project(centroid, direction);
			var projectionMag = projection.magnitude;
			if (Vector3.Dot(projection, direction) < 0)
			{
				projectionMag *= -1;
				if (projectionMag < MinValue)
				{
					MinValue = projectionMag;
				}
			}
			else
			{
				if (projectionMag > MaxValue)
				{
					MaxValue = projectionMag;
				}
			}
			return projectionMag;
		}
	}
}
