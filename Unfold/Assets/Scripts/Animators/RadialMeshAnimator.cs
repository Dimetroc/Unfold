using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class RadialMeshAnimator : MeshAnimator
	{
		private readonly bool _toCenter;
		private readonly Vector3 _center;
        private readonly bool _inverse;
        private readonly Vector3 _offset;
        private readonly MeshFilter _meshFilter;

        public RadialMeshAnimator(List<SmartTriangle> smartTriangles, bool toCenter, Vector3 center, bool inverse, Vector3 offset, MeshFilter meshFilter)
		{
			SmartTriangles = smartTriangles;
			_toCenter = toCenter;
            _center = center;
            _inverse = inverse;
            _offset = offset;
            _meshFilter = meshFilter;
		}

		public override void Start()
		{
            if (!_inverse)
            {
                _meshFilter.mesh.Clear();
            }
            foreach (var st in SmartTriangles)
            {
                st.Setup(this);
                if (_inverse)
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

		public override float GetAnimationValue(Vector3 centroid)
		{
			if (_toCenter)
			{
				return GetAnimationValue(centroid, (_center - centroid).normalized);
			}
			return GetAnimationValue(centroid, (centroid - _center).normalized);
		}
	}
}
