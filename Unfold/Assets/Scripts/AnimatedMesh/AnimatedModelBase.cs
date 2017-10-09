using Unfold;
using UnityEngine;

namespace AnimatedMesh.AnimationModels
{
    public abstract class AnimatedModelBase<T> where T:AnimatedModelBase<T>
    {
        public bool IsSet { get; protected set; }

        protected TrianglesPool _pool;
        protected MeshTriangle _meshTriangle;

        protected TriangleData _triangle;
        protected TriangleVertices _currentVertices;
        protected TriangleVertices _targetVertices;
        protected T[] _children;
        protected bool _hasChildren = false;

        protected void SetChildren()
        {
            _children = GenerateChildren();
        }

        protected abstract T[] GenerateChildren();

        protected abstract T GetChild(TriangleData triangle);

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

        protected void ClearChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Clear();
            }
            _children = null;
            _hasChildren = false;
        }

        protected void Combine()
        {
            ClearChildren();

            _meshTriangle = _pool.GetTriangle();
            _meshTriangle.UseTriangle(_triangle);
            _meshTriangle.UpdateVertices(_currentVertices);

            IsSet = true;

        }
    }
}