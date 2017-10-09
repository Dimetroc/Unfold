using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
    public class TrianglesStorage
    {
        private readonly MeshData _meshData;
        private const int EXPAND_AMOUNT = 10;
        private readonly Queue<MeshTriangle> _triangles;

        public TrianglesStorage(MeshData meshData)
        {
            _triangles = new Queue<MeshTriangle>();
            _meshData = meshData;
        }

        public MeshTriangle GetTriangle()
        {
	        if (_triangles.Count == 0)
	        {
		        ExpandPool();
	        }
            return _triangles.Dequeue();
        }

        public void ReturnTriangle(MeshTriangle triangle)
        {
            _triangles.Enqueue(triangle);
        }

        private void ExpandPool()
        {
            for (int i = 0; i < EXPAND_AMOUNT; i++)
            {
                _triangles.Enqueue(new MeshTriangle(_meshData));
            }
        }

	    public Vector3 GetCenter()
	    {
		    var xSum = 0f;
		    var ySum = 0f;
		    var zSum = 0f;
		    foreach (var triangle in _triangles)
		    {
			    xSum += triangle.GetCentroid().x;
			    ySum += triangle.GetCentroid().y;
			    zSum += triangle.GetCentroid().z;
		    }
		    return new Vector3(xSum / _triangles.Count, ySum / _triangles.Count, zSum / _triangles.Count);
	    }
	}
}
