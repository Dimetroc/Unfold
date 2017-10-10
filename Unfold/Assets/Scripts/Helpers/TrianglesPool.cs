using System.Collections.Generic;

namespace Unfold
{
    public class TrianglesPool
    {
        private readonly MeshData _meshData;
        private const int EXPAND_AMOUNT = 10;
        private readonly Queue<MeshTriangle> _triangles;

        public TrianglesPool(MeshData meshData)
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
	}
}
