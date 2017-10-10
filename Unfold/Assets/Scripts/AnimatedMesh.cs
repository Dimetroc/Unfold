using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unfold
{
    [RequireComponent(typeof (MeshFilter))]
    public class AnimatedMesh : MonoBehaviour
    {
	    public event Action AnimationFinished = delegate { };

        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private MeshData _originalMeshData;
        private MeshData _newMeshData;

        private TrianglesPool _trianglesPool;
        private MeshAnimator _meshAnimator;
        private List<SmartTriangle> _smartTriangles;

        #region Settings
        [SerializeField]
        public Vector3 Direction;
        [SerializeField] 
        public Vector3 Offset;
        [SerializeField] 
        private bool ToCenter;
        [SerializeField]
        public float MinimalArea = 1;
        [SerializeField]
        public float UnfoldSpeed = 10.0f;
	    [SerializeField]
		private Texture2D _heightMap;
        #endregion

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
			_mesh = _meshFilter.mesh;
	        _originalMeshData = new MeshData(_mesh);
		}

        public void StartAnimation(AnimationType animationType, bool unfold)
        {
	        _meshFilter.mesh = _mesh;
			_newMeshData = new MeshData();
	        _trianglesPool = new TrianglesPool(_newMeshData);
			_smartTriangles = new List<SmartTriangle>();
            for (var i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _trianglesPool,
                    MinimalArea));
            }
            switch (animationType)
            {
                case AnimationType.Direct:
                    _meshAnimator = new DirectMeshAnimator(_smartTriangles, Direction, Offset, unfold, _meshFilter);
                    break;
                case AnimationType.Radial:
                    _meshAnimator = new RadialMeshAnimator(_smartTriangles, ToCenter, unfold, Offset, _meshFilter);
                    break;
				case AnimationType.Random:
					_meshAnimator = new RandomMeshAnimator(_smartTriangles, Offset, unfold, _meshFilter);
					break;
				case AnimationType.HeightMap:
					_meshAnimator = new HeightMapMeshAnimator(_smartTriangles, _heightMap, unfold, _meshFilter);
					break;
            }
            StartCoroutine(AnimationRoutine());
        }

	    private void Reset()
	    {
		    
	    }

        private IEnumerator AnimationRoutine()
        {
            _meshAnimator.Start();
            yield return new WaitForSeconds(0.3f);
            var animationValue = _meshAnimator.MinValue;
            var allCompleted = false;
            while (!allCompleted)
            {
                animationValue += Time.deltaTime * UnfoldSpeed;
                Profiler.BeginSample("UpdateTriangles");
                allCompleted = UpdateTriangles(animationValue);
                Profiler.EndSample();
                Profiler.BeginSample("UpdateMesh");
                UpdateMesh();
                Profiler.EndSample();
                yield return null;
            }
	        _meshAnimator.End();
	        AnimationFinished();
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
