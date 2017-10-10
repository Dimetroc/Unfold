using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class LinearBindModel : AnimatedModelBase<LinearBindModel>
    {
        private float _value = 0.0f;
        private TriangleVertices _currentVertices;
        private TriangleVertices _targetVertices;
        private readonly LinearController _controller;
        private bool _isFirst = true;
        
        public LinearBindModel(TriangleData triangle, LinearController controller, LinearBindModel parent)
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

        protected override LinearBindModel[] GenerateChildren()
        {
            return SubDivider.SubDivideTriangle(_triangle, this, GetChild);
        }

        protected override LinearBindModel GetChild(TriangleData triangle, LinearBindModel parent)
        {
            return new LinearBindModel(triangle, _controller, parent);
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
                _value = direction.GetCentroidAnimationValue(_targetVertices.GetCentroid());
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
                if (!_controller.IsAbleToTransform(_value)) return;
                _currentVertices.Lerp(_targetVertices, Time.deltaTime * LinearController.SPEED);
                IsSet = _currentVertices.TheSame(_targetVertices, LinearController.DELTA);
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

