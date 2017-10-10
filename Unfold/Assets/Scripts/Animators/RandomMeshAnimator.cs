using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unfold
{
	public class RandomMeshAnimator : MeshAnimator
	{
		private readonly bool _unfold;
		private readonly Vector3 _offset;
		private readonly MeshFilter _meshFilter;

		public RandomMeshAnimator(List<SmartTriangle> smartTriangles, Vector3 offset, bool unfold, MeshFilter meshFilter)
		{
			SmartTriangles = smartTriangles;
			_offset = offset;
			_unfold = unfold;
			_meshFilter = meshFilter;
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			var center = new Vector3(0,0,0);
			
			GetAnimationValue(centroid, (centroid - center).normalized);
			return Random.Range(MinValue, MaxValue);
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
					st.ChangeTargetPosition(_offset);
				}
				else
				{
					st.ChangeCurrentPosition(_offset);
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
	}
}
