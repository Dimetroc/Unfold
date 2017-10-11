using System;
using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public class TextureBindModel : AnimatedModelBase<TextureBindModel>
    {
        private float _value = 0.0f;
        private TriangleVertices _currentVertices;
        private TriangleVertices _targetVertices;
        private bool _isFirst = true;

        private readonly TextureController _controller;

        public Vector3 Centroid;

        public TextureBindModel(TriangleData triangle, TextureController controller, TextureBindModel parent)
        {
            IsSet = false;
            _triangle = triangle;
            
            _controller = controller;
            _pool = controller.Pool;
            _parent = parent;

            _currentVertices = _targetVertices = new TriangleVertices(_triangle);
            Centroid = _currentVertices.GetCentroid();

            _hasChildren = _controller.IsAbleToHaveChildren(_currentVertices);
            if (_hasChildren)
            {
                SetChildren();
            }
        }

        protected override TextureBindModel[] GenerateChildren()
        {
            return SubDivider.SubDivideTriangle(_triangle, this, GetChild);
        }

        protected override TextureBindModel GetChild(TriangleData triangle, TextureBindModel parent)
        {
            return new TextureBindModel(triangle, _controller, parent);
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
                _value = calcFunction(_targetVertices.GetCentroid());
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
                _currentVertices.Lerp(_targetVertices, Time.deltaTime * RadialController.SPEED);
                IsSet = _currentVertices.TheSame(_targetVertices, RadialController.DELTA);
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

