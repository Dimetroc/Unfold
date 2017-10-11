using System.Collections.Generic;
using Unfold;
using UnityEngine;


namespace AnimatedMesh.AnimationModels
{
    public class TextureController : AnimatedControllerBase
    {
        private bool _allAreSet = false;
        private float _unfoldSpeed = 0.1f;
        public const float DELTA = 0.1f;
        public const float SPEED = 10.0f;

        private Texture2D _texture;
        private float _currentValue;
        protected List<TextureBindModel> _bindModels;
        protected List<TextureBreakModel> _breakModels;
        private const float MIN_AREA = 0.2f;
        public TrianglesPool Pool { get { return _trianglesPool; } }

        public bool IsFolding { get; private set; }

        private Vector4 _bounds;

        public TextureController(MeshFilter meshFilter, Texture2D texture, bool isFolding) : base(meshFilter)
        {
            IsFolding = isFolding;
            _texture = texture;
            FillModels();
            GetBounds();
            ProcessBounds();
            ProcessDirection();
            _currentValue = 0;
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

        private void GetBounds()
        {
            _bounds = Vector4.zero;
            if (IsFolding)
            {
                foreach (var triangle in _bindModels)
                {
                    ProcessCentroids(triangle.Centroid);
                }
            }
            else
            {
                foreach (var triangle in _breakModels)
                {
                    ProcessCentroids(triangle.Сentroid);
                }
            }
        }

        private void ProcessBounds()
        {
            _bounds[0] *= -1;
            _bounds[1] += _bounds[0];
            _bounds[2] *= -1;
            _bounds[3] += _bounds[2];
        }

        private void ProcessCentroids(Vector3 centroid)
        {
            if (centroid[0] < _bounds[0]) _bounds[0] = centroid[0];
            if (centroid[0] > _bounds[1]) _bounds[1] = centroid[0];
            if (centroid[1] < _bounds[2]) _bounds[2] = centroid[1];
            if (centroid[1] > _bounds[3]) _bounds[3] = centroid[1];

        }

        private void ProcessDirection()
        {

            if (IsFolding)
            {
                foreach (var triangle in _bindModels)
                {
                    triangle.UpdateKeyValue(GetValue);
                }
            }
            else
            {
                foreach (var triangle in _breakModels)
                {
                    triangle.UpdateKeyValue(GetValue);
                }
            }
        }

        private float GetValue(Vector3 centroid)
        {
            var u = (centroid.x + _bounds[0]) / _bounds[1];
            var v = (centroid.y + _bounds[2]) / _bounds[3];
            return 1.0f - _texture.GetPixelBilinear(u,v).grayscale;
        }

        protected List<TextureBindModel> GenerateBindTriangles()
        {
            var models = new List<TextureBindModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new TextureBindModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }

        protected List<TextureBreakModel> GenerateBreakTriangles()
        {
            var models = new List<TextureBreakModel>();

            for (int i = 0; i < _originalMeshData.Triangles.Count; i += 3)
            {
                models.Add(new TextureBreakModel(new TriangleData(_originalMeshData, i), this, null));
            }

            return models;
        }



        public override void UpdateMeshTriangles()
        {
            if (_allAreSet) return;
            _currentValue += Time.deltaTime * _unfoldSpeed;
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
            return _currentValue > value;
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

