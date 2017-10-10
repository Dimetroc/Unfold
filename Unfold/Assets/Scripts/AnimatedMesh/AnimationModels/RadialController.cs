using Unfold;
using UnityEngine;


namespace AnimatedMesh.AnimationModels
{
    public class RadialController: AnimatedControllerBase<RadialModel>
    {
        public enum RadialType
        {
            Inward,
            Outward
        }

        private UnfoldDirection _unfoldDirection;
        private bool _allAreSet = false;
        private float _directionValue = 0;
        private float _unfoldSpeed = 5.0f;

        private RadialType _radialType;


        public RadialController(MeshFilter meshFilter, RadialType type) : base(meshFilter)
        {
            _radialType = type;
            GenerateTriangles();
            ProcessDirection();
            _directionValue = _radialType == RadialType.Inward? _unfoldDirection.Min:_unfoldDirection.Max;
        }


        private void ProcessDirection()
        {
            _unfoldDirection = new UnfoldDirection(Vector3.up);
            foreach (var triangle in _radialModelBases)
            {
                triangle.UpdateUnfoldDirection(_unfoldDirection);
            }
        }

        protected override RadialModel GetModel(int index)
        {
            return new RadialModel(new TriangleData(_originalMeshData, index), _trianglesPool, _radialType, 0.5f);
        }


        public override void UpdateMeshTriangles()
        {
            if (_allAreSet) return;
            _directionValue += (_radialType == RadialType.Outward? -1:1) * Time.deltaTime * _unfoldSpeed;
            _allAreSet = UpdateTriangles();
            UpdateMeshWithNewMeshData();
            if(_allAreSet) SetOriginalMeshData();
        }

        private bool UpdateTriangles()
        {
            var allAreSet = true;
            foreach (var st in _radialModelBases)
            {
                st.UpdateModel(_directionValue);
                if(!st.IsSet)allAreSet = false;
            }

            return allAreSet;
        }
    }
}
