using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
	public class RadialMeshAnimator : MeshAnimator
	{
		private readonly bool _toCenter;
		private readonly Vector3 _center;
        private readonly bool _unfold;
        private readonly Vector3 _offset;
        private readonly MeshFilter _meshFilter;

        public RadialMeshAnimator(List<SmartTriangle> smartTriangles, bool toCenter, bool unfold, Vector3 offset, MeshFilter meshFilter)
		{
			SmartTriangles = smartTriangles;
			_toCenter = toCenter;
            _center = new Vector3(0, 0, 0);
            _unfold = unfold;
            _offset = offset;
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
