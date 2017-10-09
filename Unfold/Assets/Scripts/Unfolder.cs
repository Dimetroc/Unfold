using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unfold
{
	[RequireComponent(typeof(MeshFilter))]
    public class Unfolder:MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private MeshData _originalMeshData;
        private MeshData _newMeshData;

        private TrianglesStorage _trianglesStorage;

        private MeshAnimator _meshAnimator;

        private List<SmartTriangle> _smartTriangles;

        private bool _allAreSet;
        private float _animationValue;

        #region Settings

	    [SerializeField]
		private AnimationType _animationType;
		[SerializeField]
        private Vector3 _direction;
        [SerializeField] 
        private float _minimalArea = 1;
        [SerializeField] 
        private float _unfoldSpeed = 10.0f;
        [SerializeField] 
        private bool _inverse;
        #endregion

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();
            _trianglesStorage = new TrianglesStorage(_newMeshData);
            _meshFilter.mesh.Clear();
            Prepare();
        }

	    private MeshAnimator CreateAnimator()
	    {
			switch (_animationType)
			{
				case AnimationType.Direct:
					return new DirectMeshAnimator(_direction);
				case AnimationType.Radial:
					return new RadialMeshAnimator(_direction);
			}
		    return null;
	    }

	    private void Prepare()
	    {
		    _smartTriangles = new List<SmartTriangle>();

		    for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
		    {
			    _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _trianglesStorage, _minimalArea));
		    }
		    _meshAnimator = CreateAnimator();
			foreach (var triangle in _smartTriangles)
		    {
			    triangle.SetAnimator(_meshAnimator);
		    }
		    _animationValue = _meshAnimator.Min;
		}

        private void Update()
        {
            if(!_allAreSet)
            {
                _animationValue += Time.deltaTime * _unfoldSpeed;
                Profiler.BeginSample("UpdateTriangles");
                _allAreSet = UpdateTriangles(_animationValue);
                Profiler.EndSample();
                Profiler.BeginSample("UpdateMesh");
                UpdateMesh();
                Profiler.EndSample();
            }
        }

        private bool UpdateTriangles(float directionValue)
        {
            var allAreSet = true;
            foreach (var st in _smartTriangles)
            {
                if (!st.UpdateMeshData(directionValue))
                {
                    allAreSet = false;
                }
            }

            return allAreSet;
        }

        private void UpdateMesh()
        {
            _meshFilter.mesh.SetVertices(_newMeshData.Vertices);
            _meshFilter.mesh.SetNormals(_newMeshData.Normals);
            _meshFilter.mesh.SetUVs(0, _newMeshData.Uvs);
            _meshFilter.mesh.SetTriangles(_newMeshData.Triangles, 0);
        }
    }
}
