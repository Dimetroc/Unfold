using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class RadialModel:AnimatedModelBase<RadialModel>
    {

        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;
        private float _radius = 0.0f;
        private float _unfoldRadius = 0.0f;
        private TriangleVertices _currentVertices;
        private bool _isFirst = true;

        private readonly float _minArea;

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

        public void UpdateModel(float radius)
        {
            _unfoldRadius = radius;
            UpdateMeshData();
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
                _radius = direction.GetCentroidRadiusAnimationValue(_targetVertices.GetCentroid());
            }
        }

        protected override void UpdateChild(RadialModel child)
        {
            child.UpdateModel(_unfoldRadius);
        }

        protected override void UpdateSelf()
        {
            if (_isFirst)
            {
                _currentVertices.SetToVector((_targetVertices).GetCentroid());
                _isFirst = false;
            }
            else
            {

                if (_unfoldRadius < _radius) return;

                _currentVertices.Lerp(_targetVertices, Time.deltaTime * SPEED);
                IsSet = _currentVertices.TheSame(_targetVertices, DELTA);
                if (IsSet) _currentVertices = _targetVertices;
            }
            SetVertices(_currentVertices);
        }

    }
}
