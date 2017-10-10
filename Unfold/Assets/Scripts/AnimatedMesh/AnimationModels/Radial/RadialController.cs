using System.Collections.Generic;
using Unfold;
using UnityEngine;


namespace AnimatedMesh.AnimationModels
{
    public class RadialController: AnimatedControllerBase
    {
        public enum RadialType
        {
            Inward,
            Outward
        }

        private UnfoldDirection _unfoldDirection;
        private bool _allAreSet = false;
        private float _unfoldSpeed = 5.0f;
        private float _unfoldRadius;
        private readonly RadialType _type;
        protected List<RadialBindModel> _bindModels;
        protected List<RadialBreakModel> _breakModels;
        private const float MIN_AREA = 0.2f;
        public TrianglesPool Pool { get { return _trianglesPool; } }
        
        public bool IsFolding { get; private set; }

        public RadialController(MeshFilter meshFilter, RadialType type, bool isFolding) : base(meshFilter)
        {
            IsFolding = isFolding;
            _type = type;
            
            FillModels();
            ProcessDirection();
            _unfoldRadius = _type == RadialType.Inward? _unfoldDirection.Min:_unfoldDirection.Max;
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
            _unfoldDirection = new UnfoldDirection(Vector3.up);
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

        protected List<RadialBindModel> GenerateBindTriangles()
        {
            var models = new List<RadialBindModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new RadialBindModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }

        protected List<RadialBreakModel> GenerateBreakTriangles()
        {
            var models = new List<RadialBreakModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new RadialBreakModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }



        public override void UpdateMeshTriangles()
        {
            if (_allAreSet) return;
            _unfoldRadius += (_type == RadialType.Outward? -1:1) * Time.deltaTime * _unfoldSpeed;
            _allAreSet = UpdateTriangles();
            UpdateMeshWithNewMeshData();
            if(_allAreSet && IsFolding) SetOriginalMeshData();
        }

        public bool IsAbleToHaveChildren(TriangleVertices vertices)
        {
            return vertices.GetArea() > MIN_AREA;
        }

        public bool IsAbleToBind(float radius)
        {
            if (_type == RadialController.RadialType.Inward)
            {
                if (_unfoldRadius < radius) return false;
            }
            else
            {
                if (_unfoldRadius > radius) return false;
            }

            return true;
        }

        public bool IsAbleToBreak(float radius)
        {
            if (_type == RadialController.RadialType.Inward)
            {
                if (_unfoldRadius < radius) return false;
            }
            else
            {
                if (_unfoldRadius > radius) return false;
            }

            return true;
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
