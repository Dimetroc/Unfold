using UnityEngine;

namespace Unfold
{
	public enum AnimationType
	{
		Direct,
		Radial
	}

	public abstract class MeshAnimator
	{
		public float Max { get; protected set; }
		public float Min { get; protected set; }

		public abstract float GetAnimationValue(Vector3 centroid);
	}
}
