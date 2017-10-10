using Unfold;

namespace AnimatedMesh.AnimationModels
{
    public abstract class AnimatedModelBase<T> where T:AnimatedModelBase<T>
    {
        public bool IsSet { get; protected set; }

        protected TrianglesPool _pool;
        protected MeshTriangle _meshTriangle;

        protected TriangleData _triangle;
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

        protected void UpdateMeshData()
        {
            if (IsSet) return;

            if (_hasChildren)
            {
                UpdateChildren();
            }
            else
            { 
                UpdateSelf();
            }
        }

        private void UpdateChildren()
        {
            IsSet = true;

            for (int i = 0; i < _children.Length; i++)
            {
                UpdateChild(_children[i]);
                IsSet = _children[i].IsSet && IsSet;
            }

            if (IsSet)
            {
                Combine();
            }
        }

        protected abstract void UpdateChild(T child);

        protected abstract void UpdateSelf();

        private void Combine()
        {
            ClearChildren();
            SetVertices(_targetVertices);
        }

        protected void SetVertices(TriangleVertices vertices)
        {
            if (_meshTriangle == null)
            {
                _meshTriangle = _pool.GetTriangle();
                _meshTriangle.UseTriangle(_triangle);
            }

            _meshTriangle.UpdateVertices(vertices);
        }
    }
}