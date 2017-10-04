using UnityEngine;

namespace Unfold
{
    public class SmartTriangle
    {
        private readonly MeshData _targetMeshData;
        private TriangleData _triangle;
        private TriangleVertices _currentVertices;
        private TriangleVertices _targetVertices;
        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;

        private SmartTriangle[] _children;

        private readonly bool _hasChildren = false;
        private bool _isFirst = true;
        public bool IsSet { get; private set; }

        public SmartTriangle(TriangleData triangle, MeshData targetData, int subdivisionNumber)
        {
            IsSet = false;
            _triangle = triangle;
            _currentVertices = _targetVertices = new TriangleVertices(_triangle);
            _targetMeshData = targetData;
            _hasChildren = subdivisionNumber > 0;
            if (_hasChildren)
            {
                GenerateChildren(subdivisionNumber - 1);
            }
            else
            {
                SetTriangle();
                _targetMeshData.UpdateZeroedTriangleVertices(_triangle);
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

        public bool UpdateMeshData()
        {
            if(IsSet)return IsSet;

            

            if (_hasChildren)
            {
                UpdateChildren();
            }
            else
            {
                if (_isFirst)
                {
                    SetFirst();
                    _isFirst = false;
                }
                else
                {
                    UpdateSelf();
                }

                
            }

            return false;
        }

        private void SetFirst()
        {
            _currentVertices.SetToVector((_targetVertices + new Vector3(0, 10, 0)).GetCentroid());
        }

        private void UpdateSelf()
        {
            _currentVertices.Lerp(_targetVertices, Time.deltaTime * SPEED);
            IsSet = _currentVertices.TheSame(_targetVertices, DELTA);
            if (IsSet) _currentVertices = _targetVertices;
            _targetMeshData.UpdateTriangleVertices(_triangle, _currentVertices);
        }

        private void UpdateChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                if(! _children[i].UpdateMeshData())return;
            }

            IsSet = true;
        }

    }
}
