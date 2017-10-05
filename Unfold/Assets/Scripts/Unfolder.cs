using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
    [RequireComponent(typeof(MeshFilter))]
    public class Unfolder:MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private MeshData _originalMeshData;
        private MeshData _newMeshData;

        private UnfoldDirection _unfoldDirection;

        private List<SmartTriangle> _smartTriangles;

        [SerializeField] private Vector3 _direction;
        [SerializeField] private int _subDivisionNumber = 1;
        [SerializeField] private float _unfoldSpeed = 10.0f;
         
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();

            GenerateTriangles();

            SetNewDataToMesh();

            ProcessDirection();

            StartCoroutine(UnfoldRoutine());
        }

        private void ProcessDirection()
        {
            _unfoldDirection = new UnfoldDirection(_direction);
            foreach (var triangle in _smartTriangles)
            {
                triangle.UpdateUnfoldDirection(_unfoldDirection);
            }
        }

        private void GenerateTriangles()
        {
            _smartTriangles = new List<SmartTriangle>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i+= 3)
            {
                _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _newMeshData, _subDivisionNumber));
            }
        }

        private void SetNewDataToMesh()
        {
            
            _meshFilter.mesh.SetVertices(_newMeshData.Vertices);
            _meshFilter.mesh.SetNormals(_newMeshData.Normals);
            _meshFilter.mesh.SetUVs(0,_newMeshData.Uvs);
            _meshFilter.mesh.SetTriangles(_newMeshData.Triangles, 0);
        }


        private IEnumerator UnfoldRoutine()
        {
            Debug.Log(_unfoldDirection.Min);
            Debug.Log(_unfoldDirection.Max);

            var allAreSet = false;
            var directionValue = _unfoldDirection.Min;
            while (!allAreSet)
            {
                directionValue += Time.deltaTime * _unfoldSpeed;
                allAreSet = UpdateTriangles(directionValue);
                UpdateMesh();
                yield return null;
            }
        }

        private bool UpdateTriangles(float directionValue)
        {
            var allAreSet = true;
            foreach (var st in _smartTriangles)
            {
                if (!st.UpdateMeshData(directionValue)) allAreSet = false;
            }

            return allAreSet;
        }

        private void UpdateMesh()
        {
            _meshFilter.mesh.SetVertices(_newMeshData.Vertices);
        }
    }
}
