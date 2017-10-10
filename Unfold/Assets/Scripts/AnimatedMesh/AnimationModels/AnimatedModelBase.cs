using Unfold;

namespace AnimatedMesh.AnimationModels
{
    public abstract class AnimatedModelBase<T>:IAnimatedModel where T:AnimatedModelBase<T>
    {
        public bool IsSet { get; protected set; }

        protected TrianglesPool _pool;
        protected MeshTriangle _meshTriangle;

        protected TriangleData _triangle;
        protected T[] _children;
        protected T _parent;
        protected bool _hasChildren = false;

        protected void SetChildren()
        {
            _children = GenerateChildren();
        }

        protected abstract T[] GenerateChildren();

        protected abstract T GetChild(TriangleData triangle, T parent);

        public void Clear()
        {
            if (_children != null && _children.Length != 0)
            {
                ClearChildren();
            }

            ClearTriangle();
            _pool = null;

        }

        protected void ClearTriangle()
        {
            if (_meshTriangle != null)
            {
                _meshTriangle.ClearTriangle();
                _pool.ReturnTriangle(_meshTriangle);
                _meshTriangle = null;
            }
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

        public virtual void UpdateModel()
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
            var allChildrenSet = true;

            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].UpdateModel();
                allChildrenSet = _children[i].IsSet && allChildrenSet;
            }

            if (allChildrenSet)
            {
                AllChildrenAreSet();
            }
        }

        protected abstract void UpdateSelf();

        protected abstract void AllChildrenAreSet();

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