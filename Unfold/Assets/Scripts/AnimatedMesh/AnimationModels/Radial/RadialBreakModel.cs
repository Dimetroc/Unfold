using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class RadialBreakModel : AnimatedModelBase<RadialBreakModel>
    {

        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;
        private float _radius = 0.0f;
        private TriangleVertices _currentVertices;
        private Vector3 _centroidVector;
        private readonly RadialController _controller;
        private bool _isBroken = false;

        public RadialBreakModel(TriangleData triangle, RadialController controller, RadialBreakModel parent)
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

        protected override RadialBreakModel[] GenerateChildren()
        {
            return SubDivider.SubDivideTriangle(_triangle, this, GetChild);
        }

        protected override RadialBreakModel GetChild(TriangleData triangle, RadialBreakModel parent)
        {
            return new RadialBreakModel(triangle, _controller, parent);
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
                _radius = direction.GetCentroidRadiusAnimationValue(_centroidVector);
            }
        }

        public override void UpdateModel()
        {
            if(IsSet)return;

            if (_hasChildren && !_isBroken)
            {
                if(IsReadyToBreak()) Break();
            }

            base.UpdateModel();
        }

        public bool IsReadyToBreak()
        {
            if (!_hasChildren) return _controller.IsAbleToBreak(_radius);

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
            if (!_controller.IsAbleToBreak(_radius)) return;
            _currentVertices.LerpToVector(_centroidVector, Time.deltaTime * SPEED);
            IsSet = _currentVertices.AllAreAtTheVector(_centroidVector, DELTA);
            SetVertices(_currentVertices);
        }

        protected override void AllChildrenAreSet()
        {
            Clear();
            IsSet = true;
        }
    }
}

