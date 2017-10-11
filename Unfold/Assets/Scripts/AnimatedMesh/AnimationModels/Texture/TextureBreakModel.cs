using System;
using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class TextureBreakModel : AnimatedModelBase<TextureBreakModel>
    {
        private float _radius = 0.0f;
        private TriangleVertices _currentVertices;
        public Vector3 Сentroid { get; private set; }
        private readonly TextureController _controller;
        private bool _isBroken = false;

        public TextureBreakModel(TriangleData triangle, TextureController controller, TextureBreakModel parent)
        {
            IsSet = false;
            _triangle = triangle;
            _controller = controller;
            _pool = controller.Pool;
            _parent = parent;

            _currentVertices = new TriangleVertices(_triangle);

            Сentroid = _currentVertices.GetCentroid();


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

        protected override TextureBreakModel[] GenerateChildren()
        {
            return SubDivider.SubDivideTriangle(_triangle, this, GetChild);
        }

        protected override TextureBreakModel GetChild(TriangleData triangle, TextureBreakModel parent)
        {
            return new TextureBreakModel(triangle, _controller, parent);
        }


        //TODO
        public void UpdateKeyValue(Func<Vector3, float> calcFunction)
        {
            if (_hasChildren)
            {
                foreach (var child in _children)
                {
                    child.UpdateKeyValue(calcFunction);
                }
            }
            else
            {
                _radius = calcFunction(Сentroid);
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
            _currentVertices.LerpToVector(Сentroid, Time.deltaTime * RadialController.SPEED);
            IsSet = _currentVertices.AllAreAtTheVector(Сentroid, RadialController.DELTA);
            SetVertices(_currentVertices);
        }

        protected override void AllChildrenAreSet()
        {
            Clear();
            IsSet = true;
        }
    }
}


