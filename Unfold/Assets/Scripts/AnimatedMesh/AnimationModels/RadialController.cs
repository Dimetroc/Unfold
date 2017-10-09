using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unfold;
using UnityEngine;
using UnityEngine.Profiling;

namespace AnimatedMesh.AnimationModels
{
    public class RadialController
    {
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private TrianglesPool _trianglesPool;
        private UnfoldDirection _unfoldDirection;
        private List<RadialModel> _radialModelBases;
        private MeshData _originalMeshData;
        private MeshData _newMeshData;

        private bool _allAreSet = false;
        private float _directionValue = 0;
        private float _unfoldSpeed = 5.0f;

        public RadialController(MeshFilter meshFilter)
        {
            _meshFilter = meshFilter;
            _mesh = _meshFilter.mesh;

            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();
            _trianglesPool = new TrianglesPool(_newMeshData);

            _mesh.Clear();

            GenerateTriangles();

            ProcessDirection();

            _directionValue = _unfoldDirection.Min;
        }


        private void ProcessDirection()
        {
            _unfoldDirection = new UnfoldDirection(Vector3.up);
            foreach (var triangle in _radialModelBases)
            {
                triangle.UpdateUnfoldDirection(_unfoldDirection);
            }
        }

        private void GenerateTriangles()
        {
            _radialModelBases = new List<RadialModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                _radialModelBases.Add(new RadialModel(new TriangleData(_originalMeshData, i), _trianglesPool, 0.2f));
            }
        }


        public void UpdateMeshTriangles()
        {
            if (!_allAreSet)
            {
                _directionValue += Time.deltaTime * _unfoldSpeed;
                _allAreSet = UpdateTriangles(_directionValue);
                UpdateMesh();

            }
        }

        private bool UpdateTriangles(float directionValue)
        {
            var allAreSet = true;
            foreach (var st in _radialModelBases)
            {
                st.UpdateMeshData(directionValue);
                if(!st.IsSet)allAreSet = false;
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
