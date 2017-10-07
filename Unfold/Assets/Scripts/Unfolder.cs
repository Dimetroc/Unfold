﻿using System.Collections;
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

        private TrianglesPool _trianglesPool;

        private UnfoldDirection _unfoldDirection;

        private List<SmartTriangle> _smartTriangles;

        private bool _allAreSet;
        private float _directionValue;

        #region Settings
        [SerializeField]
        private Direction _direction;
        [SerializeField] 
        private float _minimalArea = 1;
        [SerializeField] 
        private float _unfoldSpeed = 10.0f;
        #endregion

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();
            _trianglesPool = new TrianglesPool(_newMeshData);

            _meshFilter.mesh.Clear();

            GenerateTriangles();
            ProcessDirection();
        }

        private void ProcessDirection()
        {
            _unfoldDirection = new UnfoldDirection(_direction, GetCenter());
            foreach (var triangle in _smartTriangles)
            {
                triangle.UpdateUnfoldDirection(_unfoldDirection);
            }
            _directionValue = _unfoldDirection.Min;
        }

        private Vector3 GetCenter()
        {
            var xSum = 0f;
            var ySum = 0f;
            var zSum = 0f;
            foreach (var triangle in _smartTriangles)
            {
                xSum += triangle.GetCentroid().x;
                ySum += triangle.GetCentroid().y;
                zSum += triangle.GetCentroid().z;
            }
            return new Vector3(xSum / _smartTriangles.Count, ySum / _smartTriangles.Count, zSum / _smartTriangles.Count);
        }

        private void GenerateTriangles()
        {
            _smartTriangles = new List<SmartTriangle>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _trianglesPool, _minimalArea));
            }
        }


        private void Update()
        {
            if(!_allAreSet)
            {
                _directionValue += Time.deltaTime * _unfoldSpeed;
                Profiler.BeginSample("UpdateTriangles");
                _allAreSet = UpdateTriangles(_directionValue);
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
                if (!st.UpdateMeshData(directionValue)) allAreSet = false;
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
