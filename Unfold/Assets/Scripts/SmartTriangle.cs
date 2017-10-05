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
        private float _directionValue = 0.0f;
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

        public bool UpdateMeshData(float unfoldValue)
        {
            if(IsSet)return true;

            

            if (_hasChildren)
            {
                UpdateChildren(unfoldValue);
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
                    UpdateSelf(unfoldValue);
                }
            }

            return IsSet;
        }

        public void UpdateUnfoldDirection(UnfoldDirection direction)
        {
            if (_hasChildren)
            {
                foreach (var child in _children)
                {
                    child.UpdateUnfoldDirection(direction);
                }
            }
            else
            {
                _directionValue = direction.GetCentroidAnimationValue(_targetVertices.GetCentroid());
            }
        }

        private void SetFirst()
        {
            _currentVertices.SetToVector((_targetVertices + new Vector3(0, 10, 0)).GetCentroid());
        }

        private void UpdateSelf(float unfoldValue)
        {
            if (unfoldValue < _directionValue)return;
            
            _currentVertices.Lerp(_targetVertices, Time.deltaTime * SPEED);
            IsSet = _currentVertices.TheSame(_targetVertices, DELTA);
            if (IsSet) _currentVertices = _targetVertices;

            _targetMeshData.UpdateTriangleVertices(_triangle, _currentVertices);
        }

        private void UpdateChildren(float unfoldValue)
        {
            IsSet = true;

            for (int i = 0; i < _children.Length; i++)
            {
                IsSet = _children[i].UpdateMeshData(unfoldValue) && IsSet;
            }

            
        }

    }
}
