using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class LinearBreakModel : AnimatedModelBase<LinearBreakModel>
    {
        private float _radius = 0.0f;
        private TriangleVertices _currentVertices;
        private readonly Vector3 _centroidVector;
        private readonly LinearController _controller;
        private bool _isBroken = false;

        public LinearBreakModel(TriangleData triangle, LinearController controller, LinearBreakModel parent)
        {
            IsSet = false;
            _triangle = triangle;
            _controller = controller;
            _pool = controller.Pool;
            _parent = parent;

            _currentVertices = new TriangleVertices(_triangle);

            _centroidVector = _currentVertices.GetCentroid();

            _hasChildren = _controller.IsAbleToHaveChildren(_currentVertices);

            if (_hasChildren)
            {
                SetChildren();
            }

            if (_parent == null)
            {
                SetVertices(_currentVertices);
            }
        }

        protected override LinearBreakModel[] GenerateChildren()
        {
            return SubDivider.SubDivideTriangle(_triangle, this, GetChild);
        }

        protected override LinearBreakModel GetChild(TriangleData triangle, LinearBreakModel parent)
        {
            return new LinearBreakModel(triangle, _controller, parent);
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
                _radius = direction.GetCentroidAnimationValue(_currentVertices.GetCentroid());
            }
        }

        public override void UpdateModel()
        {
            if (IsSet) return;

            if (_hasChildren && !_isBroken)
            {
                if (IsReadyToBreak()) Break();
            }

            base.UpdateModel();
        }

        public bool IsReadyToBreak()
        {
            if (!_hasChildren) return _controller.IsAbleToTransform(_radius);

            foreach (var child in _children)
            {
                if (child.IsReadyToBreak()) return true;
            }
            return false;
        }

        private void Break()
        {
            ClearTriangle();
            foreach (var child in _children)
            {
                child.Show();
            }
            _isBroken = true;
        }

        public void Show()
        {
            SetVertices(_currentVertices);
        }

        protected override void UpdateSelf()
        {
            if (!_controller.IsAbleToTransform(_radius)) return;
            _currentVertices.LerpToVector(_centroidVector, Time.deltaTime * RadialController.SPEED);
            IsSet = _currentVertices.AllAreAtTheVector(_centroidVector, RadialController.DELTA);
            SetVertices(_currentVertices);
        }

        protected override void AllChildrenAreSet()
        {
            Clear();
            IsSet = true;
        }
    }
}

