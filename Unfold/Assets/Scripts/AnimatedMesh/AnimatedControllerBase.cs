using System.Collections.Generic;
using AnimatedMesh.AnimationModels;
using Unfold;
using UnityEngine;


namespace AnimatedMesh
{
    public abstract class AnimatedControllerBase<T> where T:AnimatedModelBase<T>
    {
        private readonly MeshFilter _meshFilter;
        protected TrianglesPool _trianglesPool;
        protected List<T> _radialModelBases;
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

        protected void GenerateTriangles()
        {
            _radialModelBases = new List<T>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                _radialModelBases.Add(GetModel(i));
            }
        }

        protected abstract T GetModel(int index);

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
