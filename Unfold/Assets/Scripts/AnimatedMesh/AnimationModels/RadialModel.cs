using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class RadialModel:AnimatedModelBase<RadialModel>
    {

        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;
        private float _rad = 0.0f;
        private float _unfoldRadius = 0.0f;


        private bool _isFirst = true;

        private float _minArea;

        public RadialModel(TriangleData triangle, TrianglesPool pool, float minimumArea)
        {
            IsSet = false;
            _triangle = triangle;
            _minArea = minimumArea;
            _currentVertices = _targetVertices = new TriangleVertices(_triangle);
            _pool = pool;

            _hasChildren = _targetVertices.GetArea() > minimumArea;
            if (_hasChildren)
            {
                SetChildren();
            }
        }

        protected override RadialModel[] GenerateChildren()
        {
            return   SubDivider.SubDivideTriangle(_triangle, GetChild); 
        }

        protected override RadialModel GetChild(TriangleData triangle)
        {
            return new RadialModel(triangle, _pool, _minArea);
        }

       

        private float _directionValue = 0.0f;






        public bool UpdateMeshData(float unfoldValue)
        {
            if (IsSet) return true;



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
                    if (unfoldValue < _directionValue) return false;
                    UpdateSelf();
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
                //_directionValue = direction.GetCentroidAnimationValue(_targetVertices.GetCentroid());
                _directionValue = direction.GetCentroidRadiusAnimationValue(_targetVertices.GetCentroid());
            }
        }

        public void Clear()
        {
            if (_children != null && _children.Length != 0)
            {
                ClearChildren();
            }

            _meshTriangle.ClearTriangle();
            _pool.ReturnTriangle(_meshTriangle);
            _pool = null;
            _meshTriangle = null;
        }

        private void SetFirst()
        {
            //_currentVertices.SetToVector((_targetVertices + new Vector3(0, 10, 0)).GetCentroid());
            _currentVertices.SetToVector((_targetVertices).GetCentroid());
        }

        private void UpdateSelf()
        {

            if (_meshTriangle == null)
            {
                _meshTriangle = _pool.GetTriangle();
                _meshTriangle.UseTriangle(_triangle);
            }

            _currentVertices.Lerp(_targetVertices, Time.deltaTime * SPEED);
            IsSet = _currentVertices.TheSame(_targetVertices, DELTA);
            if (IsSet) _currentVertices = _targetVertices;

            _meshTriangle.UpdateVertices(_currentVertices);
        }

        private void UpdateChildren(float unfoldValue)
        {
            IsSet = true;

            for (int i = 0; i < _children.Length; i++)
            {
                IsSet = _children[i].UpdateMeshData(unfoldValue) && IsSet;
            }

            if (IsSet)
            {
                Combine();
            }
        }

        private void ClearChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Clear();
            }
            _children = null;
            _hasChildren = false;
        }

        private void Combine()
        {
            ClearChildren();

            _meshTriangle = _pool.GetTriangle();
            _meshTriangle.UseTriangle(_triangle);
            _meshTriangle.UpdateVertices(_currentVertices);

            IsSet = true;

        }
    }
}
