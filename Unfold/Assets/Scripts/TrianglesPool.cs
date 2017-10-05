using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unfold
{
    public class TrianglesPool
    {
        private readonly MeshData _meshData;
        private const int EXPAND_AMOUNT = 10;
        private readonly Queue<MeshTriangle> _triangles;

        private int TrianglesCount = 0;

        public TrianglesPool(MeshData meshData)
        {
            _triangles = new Queue<MeshTriangle>();
            _meshData = meshData;
        }

        public MeshTriangle GetTriangle()
        {
            var tr = GetFreeTriangle();
            Profiler.BeginSample("Expand pool");
            if (tr == null) ExpandPool();
            Profiler.EndSample();
            return GetFreeTriangle();
        }

        public void ReturnTriangle(MeshTriangle triangle)
        {
            Debug.Log("enqueue");
            _triangles.Enqueue(triangle);
            Debug.Log(_triangles.Count);
        }

        private MeshTriangle GetFreeTriangle()
        {
            if (_triangles.Count == 0) ExpandPool();
            return _triangles.Dequeue();
        }

        private void ExpandPool()
        {
            TrianglesCount += EXPAND_AMOUNT;
            for (int i = 0; i < EXPAND_AMOUNT; i++)
            {
                _triangles.Enqueue(new MeshTriangle(_meshData));
            }

            Debug.Log(TrianglesCount);
        }
    }
}
