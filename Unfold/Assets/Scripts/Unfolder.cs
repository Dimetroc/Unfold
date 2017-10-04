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

        private List<SmartTriangle> _smartTriangles;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _originalMeshData = new MeshData(_mesh);
            _newMeshData = new MeshData();

            GenerateTriangles();

            SetNewDataToMesh();
        }

        private void GenerateTriangles()
        {
            _smartTriangles = new List<SmartTriangle>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i+= 3)
            {
                _smartTriangles.Add(new SmartTriangle(new TriangleData(_originalMeshData, i), _newMeshData, 2));
            }
        }

        private void SetNewDataToMesh()
        {
            
            _meshFilter.mesh.SetVertices(_newMeshData.Vertices);
            _meshFilter.mesh.SetNormals(_newMeshData.Normals);
            _meshFilter.mesh.SetUVs(0,_newMeshData.Uvs);
            _meshFilter.mesh.SetTriangles(_newMeshData.Triangles, 0);
        }

        private void Update()
        {
            UpdateTriangles();
            UpdateMesh();
        }


        private void UpdateTriangles()
        {
            foreach (var st in _smartTriangles)
            {
                if(!st.UpdateMeshData()) break;
            }
        }

        private void UpdateMesh()
        {
            _meshFilter.mesh.SetVertices(_newMeshData.Vertices);
        }
    }
}
