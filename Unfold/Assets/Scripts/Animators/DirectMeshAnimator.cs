using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class DirectMeshAnimator : MeshAnimator
	{
		private readonly Vector3 _direction;
		private readonly bool _unfold;
		private readonly Vector3 _offset;
	    private readonly MeshFilter _meshFilter;

		public DirectMeshAnimator(List<SmartTriangle> smartTriangles, Vector3 direction, Vector3 offset, bool unfold, MeshFilter meshFilter)
		{
			SmartTriangles = smartTriangles;
			_direction = direction;
			_offset = offset;
			_unfold = unfold;
		    _meshFilter = meshFilter;
		}

		public override float GetAnimationValue(Vector3 centroid)
		{
			return GetAnimationValue(centroid, _direction.normalized);
		}

		public override void Start()
		{
		    if (!_unfold)
		    {
		        _meshFilter.mesh.Clear();
		    }
			foreach (var st in SmartTriangles)
			{
                st.Setup(this);
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
	}
}
