using System.Collections.Generic;
using UnityEngine;

namespace PMB
{
    sealed class MediumPMB: BasePMB, IPixelMeshBuilder
    {
        private List<MediumMeshStripe> _stripes;

        MeshQuality IPixelMeshBuilder.Quality { get { return MeshQuality.Medium; } }

        Mesh IPixelMeshBuilder.BuildMesh(Sprite sprite, Vector3 scale, bool optimized, bool recalcNormals)
        {
            if (sprite == null)
            {
                Debug.LogError("Pixel Mesh Builder. Sprite is Null, mesh will not be build!");
                return null;
            }

            _scale = scale;
            _recalcNormals = recalcNormals;

            PrepareVariables(sprite);

            PrepareData(sprite);

            BuildData();

            var mesh = new Mesh
            {
                vertices = _verts.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };

            if (_recalcNormals)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.normals = _normals.ToArray();
            }
            if (optimized) ;
            ClearMemory();
            return mesh;
        }

        protected override void ClearMemory()
        {
            base.ClearMemory();
            _stripes.Clear();
        }

        protected override void PrepareVariables(Sprite sprite)
        {
            base.PrepareVariables(sprite);
            _stripes = new List<MediumMeshStripe>();
        }

        private void PrepareData(Sprite sprite)
        {

            for (int y = _yMax - 1; y >= _yMin; y--)
            {
                for (int x = _xMin; x < _xMax; x++)
                {
                    if (sprite.texture.GetPixel(x, y).a > 0.0f)
                    {
                        var stripe = new MediumMeshStripe(_size, new Int2(x, y));

                        if (!stripe.NeedToCoverTop && (y + 1 >= _yMax || sprite.texture.GetPixel(x, y + 1).a <= 0.0f)) stripe.NeedToCoverTop = true;
                        if (!stripe.NeedToCoverBottom && (y - 1 <= _yMin || sprite.texture.GetPixel(x, y - 1).a <= 0.0f)) stripe.NeedToCoverBottom = true;

                        x++;
                        while (sprite.texture.GetPixel(x, y).a > 0.0f && x < _xMax)
                        {
                            stripe.SetEnd(new Int2(x, y));
                            if (!stripe.NeedToCoverTop && sprite.texture.GetPixel(x, y + 1).a <= 0.0f) stripe.NeedToCoverTop = true;
                            if (!stripe.NeedToCoverBottom && sprite.texture.GetPixel(x, y - 1).a <= 0.0f) stripe.NeedToCoverBottom = true;
                            x++;
                        }

                        var merged = false;

                        for (int i = 0; i < _stripes.Count; i++)
                        {
                            if (_stripes[i].MergeableToTheBottom(stripe))
                            {
                                _stripes[i] = _stripes[i].Merge(stripe);
                                merged = true;
                                break;
                            }
                        }
                        if (!merged) _stripes.Add(stripe);
                    }
                }
            }
        }

        private void BuildData()
        {
            foreach (var stripe in _stripes)
            {
                var startIndex = _verts.Count;

                _verts.Add(stripe.GetVertCoordinates(Corner.TopLeft, Side.Front, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.TopLeft, Side.Back, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.TopRight, Side.Front, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.TopRight, Side.Back, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.BottomLeft, Side.Front, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.BottomLeft, Side.Back, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.BottomRight, Side.Front, _scale) - _offset);
                _verts.Add(stripe.GetVertCoordinates(Corner.BottomRight, Side.Back, _scale) - _offset);

                if (!_recalcNormals)
                {
                    _normals.Add(GetNormal(Corner.TopLeft, Side.Front));
                    _normals.Add(GetNormal(Corner.TopLeft, Side.Back));
                    _normals.Add(GetNormal(Corner.TopRight, Side.Front));
                    _normals.Add(GetNormal(Corner.TopRight, Side.Back));
                    _normals.Add(GetNormal(Corner.BottomLeft, Side.Front));
                    _normals.Add(GetNormal(Corner.BottomLeft, Side.Back));
                    _normals.Add(GetNormal(Corner.BottomRight, Side.Front));
                    _normals.Add(GetNormal(Corner.BottomRight, Side.Back));
                }

                _uvs.Add(stripe.GetUvAtCorner(Corner.TopLeft));
                _uvs.Add(stripe.GetUvAtCorner(Corner.TopLeft));
                _uvs.Add(stripe.GetUvAtCorner(Corner.TopRight));
                _uvs.Add(stripe.GetUvAtCorner(Corner.TopRight));
                _uvs.Add(stripe.GetUvAtCorner(Corner.BottomLeft));
                _uvs.Add(stripe.GetUvAtCorner(Corner.BottomLeft));
                _uvs.Add(stripe.GetUvAtCorner(Corner.BottomRight));
                _uvs.Add(stripe.GetUvAtCorner(Corner.BottomRight));

                _triangles.Add(startIndex);
                _triangles.Add(startIndex + 2);
                _triangles.Add(startIndex + 4);
                _triangles.Add(startIndex + 2);
                _triangles.Add(startIndex + 6);
                _triangles.Add(startIndex + 4);
                _triangles.Add(startIndex + 3);
                _triangles.Add(startIndex + 1);
                _triangles.Add(startIndex + 5);
                _triangles.Add(startIndex + 3);
                _triangles.Add(startIndex + 5);
                _triangles.Add(startIndex + 7);
                _triangles.Add(startIndex + 1);
                _triangles.Add(startIndex);
                _triangles.Add(startIndex + 5);
                _triangles.Add(startIndex);
                _triangles.Add(startIndex + 4);
                _triangles.Add(startIndex + 5);
                _triangles.Add(startIndex + 7);
                _triangles.Add(startIndex + 6);
                _triangles.Add(startIndex + 2);
                _triangles.Add(startIndex + 2);
                _triangles.Add(startIndex + 3);
                _triangles.Add(startIndex + 7);

                if (stripe.NeedToCoverTop)
                {
                    _triangles.Add(startIndex + 3);
                    _triangles.Add(startIndex + 2);
                    _triangles.Add(startIndex + 1);
                    _triangles.Add(startIndex + 1);
                    _triangles.Add(startIndex + 2);
                    _triangles.Add(startIndex + 0);
                }

                if (stripe.NeedToCoverBottom)
                {
                    _triangles.Add(startIndex + 5);
                    _triangles.Add(startIndex + 4);
                    _triangles.Add(startIndex + 7);
                    _triangles.Add(startIndex + 6);
                    _triangles.Add(startIndex + 7);
                    _triangles.Add(startIndex + 4);
                }

            }
        }

        private struct MediumMeshStripe
        {
            private Int2 _topLeft;
            private Int2 _bottomLeft;
            private Int2 _topRight;
            private Int2 _bottomRight;

            private Int2 _size;

            public bool NeedToCoverTop;
            public bool NeedToCoverBottom;

            public MediumMeshStripe(Int2 size, Int2 start)
            {
                _size = size;
                _topRight = _bottomRight = _topLeft = _bottomLeft = start;
                NeedToCoverBottom = NeedToCoverTop = false;
            }

            public void SetEnd(Int2 end)
            {
                _topRight = _bottomRight = end;
            }

            public Vector3 GetVertCoordinates(Corner corner, Side side, Vector3 scale)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return new Vector3((_topLeft.X - 0.5f) * scale.x, (_topLeft.Y + 0.5f) * scale.y, (side == Side.Front ? -1 : 1) * 0.5f * scale.z);
                    case Corner.TopRight:
                        return new Vector3((_topRight.X + 0.5f) * scale.x, (_topRight.Y + 0.5f) * scale.y, (side == Side.Front ? -1 : 1) * 0.5f * scale.z);
                    case Corner.BottomLeft:
                        return new Vector3((_bottomLeft.X - 0.5f) * scale.x, (_bottomLeft.Y - 0.5f) * scale.y, (side == Side.Front ? -1 : 1) * 0.5f * scale.z);
                    case Corner.BottomRight:
                        return new Vector3((_bottomRight.X + 0.5f) * scale.x, (_bottomRight.Y - 0.5f) * scale.y, (side == Side.Front ? -1 : 1) * 0.5f * scale.z);
                    default:
                        return Vector3.zero;
                }
            }

            public Vector2 GetUvAtCorner(Corner corner)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return new Vector2((float)_topLeft.X / _size.X, (float)(_topLeft.Y + 1) / _size.Y);
                    case Corner.TopRight:
                        return new Vector2((float)(_topRight.X + 1) / _size.X, (float)(_topRight.Y + 1) / _size.Y);
                    case Corner.BottomLeft:
                        return new Vector2((float)_bottomLeft.X / _size.X, (float)_bottomLeft.Y / _size.Y);
                    case Corner.BottomRight:
                        return new Vector2((float)(_bottomRight.X + 1) / _size.X, (float)_bottomRight.Y / _size.Y);
                    default:
                        return Vector2.zero;
                }
            }

            public bool MergeableToTheBottom(MediumMeshStripe stripe)
            {
                return _bottomLeft.X == stripe._bottomLeft.X && _bottomRight.X == stripe._bottomRight.X && _bottomLeft.Y - stripe._topLeft.Y == 1;
            }

            public MediumMeshStripe Merge(MediumMeshStripe stripe)
            {
                _bottomLeft = new Int2(_bottomLeft.X, stripe._bottomLeft.Y);
                _bottomRight = new Int2(_bottomRight.X, stripe._bottomRight.Y);
                NeedToCoverBottom = stripe.NeedToCoverBottom;
                return this;
            }
        }
    }
}
