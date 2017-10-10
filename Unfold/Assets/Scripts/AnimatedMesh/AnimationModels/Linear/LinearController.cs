using System.Collections.Generic;
using Unfold;
using UnityEngine;


namespace AnimatedMesh.AnimationModels
{
    public class LinearController : AnimatedControllerBase
    {
        private UnfoldDirection _unfoldDirection;
        private bool _allAreSet = false;
        private float _unfoldSpeed = 5.0f;
        public const float DELTA = 0.01f;
        public const float SPEED = 5.0f;

        private float _unfoldValue;
        protected List<LinearBindModel> _bindModels;
        protected List<LinearBreakModel> _breakModels;
        private const float MIN_AREA = 0.2f;
        public TrianglesPool Pool { get { return _trianglesPool; } }

        private Vector3 _direction;

        public bool IsFolding { get; private set; }

        public LinearController(MeshFilter meshFilter, Vector3 direction, bool isFolding) : base(meshFilter)
        {
            IsFolding = isFolding;
            _direction = direction.normalized;
            FillModels();
            ProcessDirection();
            _unfoldValue =  _unfoldDirection.Min;
        }

        private void FillModels()
        {
            if (IsFolding)
            {
                _bindModels = GenerateBindTriangles();
            }
            else
            {
                _breakModels = GenerateBreakTriangles();
            }
        }

        private void ProcessDirection()
        {
            _unfoldDirection = new UnfoldDirection(_direction);
            if (IsFolding)
            {
                foreach (var triangle in _bindModels)
                {
                    triangle.UpdateUnfoldDirection(_unfoldDirection);
                }
            }
            else
            {
                foreach (var triangle in _breakModels)
                {
                    triangle.UpdateUnfoldDirection(_unfoldDirection);
                }
            }
        }

        protected List<LinearBindModel> GenerateBindTriangles()
        {
            var models = new List<LinearBindModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new LinearBindModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }

        protected List<LinearBreakModel> GenerateBreakTriangles()
        {
            var models = new List<LinearBreakModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new LinearBreakModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }



        public override void UpdateMeshTriangles()
        {
            if (_allAreSet) return;
            _unfoldValue += Time.deltaTime * _unfoldSpeed;
            _allAreSet = UpdateTriangles();
            UpdateMeshWithNewMeshData();
            if (_allAreSet && IsFolding) SetOriginalMeshData();
        }

        public bool IsAbleToHaveChildren(TriangleVertices vertices)
        {
            return vertices.GetArea() > MIN_AREA;
        }

        public bool IsAbleToTransform(float value)
        {
            return value < _unfoldValue;
        }

        private bool UpdateTriangles()
        {
            var allAreSet = true;

            if (IsFolding)
            {
                foreach (var st in _bindModels)
                {
                    st.UpdateModel();
                    if (!st.IsSet) allAreSet = false;
                }
            }
            else
            {
                foreach (var st in _breakModels)
                {
                    st.UpdateModel();
                    if (!st.IsSet) allAreSet = false;
                }
            }

            return allAreSet;
        }
    }
}

