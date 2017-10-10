using UnityEngine;

namespace Unfold
{
    public class MeshTriangle
    {
        private readonly MeshData _targetMeshData;
        private TriangleData _triangleData;

        public MeshTriangle(MeshData targetData)
        {
            _targetMeshData = targetData;
            _triangleData = new TriangleData(_targetMeshData.GetStartIndex());
            _targetMeshData.AddTriangle(_triangleData);
        }

        public void UseTriangle(TriangleData triangleData)
        {
            _triangleData.UpdateData(triangleData);
            _targetMeshData.UpdateTriangleData(_triangleData);
        }

        public void UpdateTriangle(TriangleData vertices)
        {
            _triangleData.UpdateVertices(vertices);
            _targetMeshData.UpdateTriangleVertices(_triangleData);
        }

        public void ClearTriangle()
        {
			_triangleData.ClearData();
            _targetMeshData.UpdateTriangleData(_triangleData);
		}

	    public Vector3 GetCentroid()
	    {
		    return _triangleData.GetCentroid();
	    }
    }
}
