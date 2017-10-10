using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class RadialBindModel:AnimatedModelBase<RadialBindModel>
    {

        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;
        private float _radius = 0.0f;
        private TriangleVertices _currentVertices;
        private TriangleVertices _targetVertices;
        private bool _isFirst = true;

        private readonly RadialController _controller;

        public RadialBindModel(TriangleData triangle, RadialController controller, RadialBindModel parent)
        {
            IsSet = false;
            _triangle = triangle;
            _controller = controller;
            _pool = controller.Pool;
            _parent = parent;

            _currentVertices = _targetVertices = new TriangleVertices(_triangle);

            _hasChildren = _controller.IsAbleToHaveChildren(_currentVertices);
            if (_hasChildren)
            {
                SetChildren();
            }
        }

        protected override RadialBindModel[] GenerateChildren()
        {
            return   SubDivider.SubDivideTriangle(_triangle, this, GetChild); 
        }

        protected override RadialBindModel GetChild(TriangleData triangle, RadialBindModel parent)
        {
            return new RadialBindModel(triangle, _controller, parent);
        }


        //TODO
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

        protected override void UpdateSelf()
        {
            if (_isFirst)
            {
                _currentVertices.SetToVector((_targetVertices).GetCentroid());
                _isFirst = false;
            }
            else
            {
                if(!_controller.IsAbleToBreak(_radius)) return;
                _currentVertices.Lerp(_targetVertices, Time.deltaTime * SPEED);
                IsSet = _currentVertices.TheSame(_targetVertices, DELTA);
                if (IsSet) _currentVertices = _targetVertices;
            }
            SetVertices(_currentVertices);
        }

        protected override void AllChildrenAreSet()
        {
            ClearChildren();
            SetVertices(_targetVertices);
            IsSet = true;
        }
    }
}
