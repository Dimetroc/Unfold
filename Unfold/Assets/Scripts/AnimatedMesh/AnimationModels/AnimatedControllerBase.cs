using System.Collections.Generic;
using AnimatedMesh.AnimationModels;
using Unfold;
using UnityEngine;


namespace AnimatedMesh
{
    public abstract class AnimatedControllerBase 
    {
        private readonly MeshFilter _meshFilter;
        protected TrianglesPool _trianglesPool;
        protected MeshData _originalMeshData;
        protected MeshData _newMeshData;

        protected AnimatedControllerBase(MeshFilter meshFilter)
        {
            _meshFilter = meshFilter;
            var mesh = _meshFilter.mesh;

            _originalMeshData = new MeshData(mesh);
            _newMeshData = new MeshData();
            _trianglesPool = new TrianglesPool(_newMeshData);

            mesh.Clear();

        }

        public abstract void UpdateMeshTriangles();

        protected void UpdateMeshWithNewMeshData()
        {
            SetMeshData(_newMeshData);
        }

        protected void SetOriginalMeshData()
        {
            _meshFilter.mesh.Clear();
            SetMeshData(_originalMeshData);
        }

        private void SetMeshData(MeshData data)
        {
            _meshFilter.mesh.SetVertices(data.Vertices);
            _meshFilter.mesh.SetNormals(data.Normals);
            _meshFilter.mesh.SetUVs(0, data.Uvs);
            _meshFilter.mesh.SetTriangles(data.Triangles, 0);
        }


    }
}
