namespace Unfold
{
    public class SmartTriangle
    {
        private readonly MeshData _targetMeshData;
        private TriangleData _triangle;

        private SmartTriangle[] _children;

        private readonly bool _hasChildren = false;

        public SmartTriangle(TriangleData triangle, MeshData targetData, int subdivisionNumber)
        {
            _triangle = triangle;
            _targetMeshData = targetData;
            _hasChildren = subdivisionNumber > 0;
            if (_hasChildren)
            {
                GenerateChildren(subdivisionNumber - 1);
            }
            else
            {
                SetTriangle();
            }
        }

        private void GenerateChildren(int subdivisionNumber)
        {
            _children = SubDivider.SubDivideTriangle(_triangle, _targetMeshData, subdivisionNumber);
        }

        private void SetTriangle()
        {
            _triangle.UpdateTriangleIndex(_targetMeshData.GetStartIndex());
            _targetMeshData.SetTriangle(_triangle);
        }

        public void UpdateMeshData()
        {
            if (_hasChildren)
            {
                UpdateChildren();
            }
            else
            {
                UpdateSelf();
            }
        }

        private void UpdateSelf()
        {
            _targetMeshData.UpdateTriangleVertices(_triangle);
        }

        private void UpdateChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].UpdateMeshData();
            }
        }

    }
}
