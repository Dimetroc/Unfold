﻿using System.Collections;
using System.Collections.Generic;
using AnimatedMesh.AnimationModels;
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

        private bool _allAreSet = false;
        private float _directionValue = 0;

        private TextureController _radialController;

        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _minimalArea = 1;
        [SerializeField] private float _unfoldSpeed = 10.0f;
        [SerializeField] private Texture2D _texture;
         
        private void Awake()
        {


            _meshFilter = GetComponent<MeshFilter>();

            _radialController = new TextureController(_meshFilter, _texture, false);
            return;
            _mesh = _meshFilter.mesh;
            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();
            _trianglesPool = new TrianglesPool(_newMeshData);

            _meshFilter.mesh.Clear();

            GenerateTriangles();

            ProcessDirection();

            _directionValue = _unfoldDirection.Min;
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
                _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _trianglesPool, _minimalArea));
            }
        }


        private void Update()
        {
            _radialController.UpdateMeshTriangles();
            return;
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
