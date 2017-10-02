using System.Collections.Generic;
using UnityEngine;

namespace PMB
{
    sealed class HighPMB: BasePMB, IPixelMeshBuilder
    {
        private List<MeshStripeHigh> _stripes;
        private PixelDataHigh[,] _pixels;

        MeshQuality IPixelMeshBuilder.Quality { get { return MeshQuality.High; } }

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
            ReadData(sprite);
            BuildFrontAndBack();
            BuildSides();

            var mesh = new Mesh
            {
                vertices = _verts.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray(),
            };

            if (_recalcNormals)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.normals = _normals.ToArray();
            }
            if(optimized) ;
            ClearMemory();
            return mesh;
        }

        protected override void PrepareVariables(Sprite sprite)
        {
            base.PrepareVariables(sprite);
            _stripes = new List<MeshStripeHigh>();
            _pixels = new PixelDataHigh[_size.X, _size.Y];
        }

        private void ReadData(Sprite sprite)
        {
            for (int y = _yMax - 1; y >= _yMin; y--)
            {
                for (int x = _xMin; x < _xMax; x++)
                {
                    if (sprite.texture.GetPixel(x, y).a > 0.0f)
                    {
                        _pixels[x, y] = new PixelDataHigh(x, y, _size);

                        var stripe = new MeshStripeHigh(_pixels[x, y]);

                        x++;
                        while (sprite.texture.GetPixel(x, y).a > 0.0f && x < _xMax)
                        {
                            _pixels[x, y] = new PixelDataHigh(x, y, _size);
                            stripe.AddPixelToTheStripe(_pixels[x, y]);
                            x++;
                        }

                        bool any = false;
                        foreach (var s in _stripes)
                        {
                            if (s.MergeableToTheBottom(stripe))
                            {
                                any = true;
                                break;
                            }
                        }
                        if (!any)
                        {
                            _stripes.Add(stripe);
                        }
                    }
                }
            }

            for (int y = _yMax - 1; y >= _yMin; y--)
            {
                for (int x = _xMin; x < _xMax; x++)
                {
                    if (_pixels[x, y] == null) continue;
                    _pixels[x, y].TopCovered = y != _yMax - 1 && _pixels[x, y + 1] != null;
                    _pixels[x, y].RightCovered = x != _xMax - 1 && _pixels[x + 1, y] != null;
                    _pixels[x, y].LeftCovered = x != _xMin && _pixels[x - 1, y] != null;
                    _pixels[x, y].BottomCovered = y != _yMin && _pixels[x, y - 1] != null;
                }
            }
        }

        private void BuildFrontAndBack()
        {
            foreach (var stripe in _stripes)
            {
                var tr = new int[12];

                var verts = SetVertices(stripe, Corner.TopLeft);
                tr[0] = verts.X;
                tr[7] = verts.Y;
                verts = SetVertices(stripe, Corner.TopRight);
                tr[1] = tr[3] = verts.X;
                tr[6] = tr[9] = verts.Y;
                verts = SetVertices(stripe, Corner.BottomLeft);
                tr[2] = tr[5] = verts.X;
                tr[8] = tr[10] = verts.Y;
                verts = SetVertices(stripe, Corner.BottomRight);
                tr[4] = verts.X;
                tr[11] = verts.Y;
                _triangles.AddRange(tr);
            }
        }

        private Int2 SetVertices(MeshStripeHigh stripe, Corner corner)
        {
            var coord = stripe.GetPixelAtCorner(corner).VertPosition(corner);
            Int2 result;
            if (_vertMatrix[coord.X, coord.Y].X < 0)
            {
                result = new Int2(_verts.Count, _verts.Count + 1);
                _vertMatrix[coord.X, coord.Y] = new Int2(_verts.Count, _verts.Count + 1);
                _verts.Add(stripe.GetPixelAtCorner(corner).GetVertCoordinates(corner, Side.Front, _scale) - _offset);
                _verts.Add(stripe.GetPixelAtCorner(corner).GetVertCoordinates(corner, Side.Back, _scale) - _offset);
                _uvs.Add(stripe.GetPixelAtCorner(corner).GetUvAtCorner(corner));
                _uvs.Add(stripe.GetPixelAtCorner(corner).GetUvAtCorner(corner));
                if (!_recalcNormals)
                {
                    _normals.Add(GetNormal(corner, Side.Front));
                    _normals.Add(GetNormal(corner, Side.Back));
                }
            }
            else
            {
                result = _vertMatrix[coord.X, coord.Y];
            }

            return result;
        }

        private void BuildSides()
        {
            bool isAllCovered = false;

            while (!isAllCovered)
            {
                isAllCovered = true;
                for (int y = _yMax - 1; y >= _yMin; y--)
                {
                    for (int x = _xMin; x < _xMax; x++)
                    {
                        if (_pixels[x, y] != null && !_pixels[x, y].IsCovered())
                        {
                            isAllCovered = false;
                            CoverPixelSides(_pixels[x, y]);
                        }
                    }
                }
            }
        }

        private void CoverPixelSides(PixelDataHigh p)
        {
            var pixel = p;
            var tr = new int[6];

            if (!pixel.TopCovered)
            {
                int x = pixel.Position.X;
                while (x >= _xMin && _pixels[x, pixel.Position.Y] != null && !_pixels[x, pixel.Position.Y].TopCovered)
                {
                    pixel = _pixels[x, pixel.Position.Y];
                    x--;
                }

                var coord = pixel.VertPosition(Corner.TopLeft);
                tr[5] = _vertMatrix[coord.X, coord.Y].X;
                tr[2] = tr[3] = _vertMatrix[coord.X, coord.Y].Y;  
                var endPixel = pixel;
                x = pixel.Position.X;
                while (x < _xMax && _pixels[x, pixel.Position.Y] != null && !_pixels[x, pixel.Position.Y].TopCovered)
                {
                    endPixel = _pixels[x, pixel.Position.Y];
                    endPixel.TopCovered = true;
                    x++;
                }

                coord = endPixel.VertPosition(Corner.TopRight);
                tr[1] = tr[4] = _vertMatrix[coord.X, coord.Y].X;
                tr[0] = _vertMatrix[coord.X, coord.Y].Y;  

            }
            else if (!pixel.RightCovered)
            {
                int y = pixel.Position.Y;
                while (y < _yMax && _pixels[pixel.Position.X, y] != null && !_pixels[pixel.Position.X, y].RightCovered)
                {
                    pixel = _pixels[pixel.Position.X, y];
                    y++;
                }

                var coord = pixel.VertPosition(Corner.TopRight);
                tr[5] = _vertMatrix[coord.X, coord.Y].X;
                tr[2] = tr[3] = _vertMatrix[coord.X, coord.Y].Y; 
                var endPixel = pixel;
                y = pixel.Position.Y;
                while (y >= _yMin && _pixels[pixel.Position.X, y] != null && !_pixels[pixel.Position.X, y].RightCovered)
                {
                    endPixel = _pixels[pixel.Position.X, y];
                    endPixel.RightCovered = true;
                    y--;
                }

                coord = endPixel.VertPosition(Corner.BottomRight);
                tr[1] = tr[4] = _vertMatrix[coord.X, coord.Y].X;
                tr[0] = _vertMatrix[coord.X, coord.Y].Y;  
            }
            else if (!pixel.BottomCovered)
            {
                int x = pixel.Position.X;
                while (x >= _xMin && _pixels[x, pixel.Position.Y] != null && !_pixels[x, pixel.Position.Y].BottomCovered)
                {
                    pixel = _pixels[x, pixel.Position.Y];
                    x--;
                }

                var coord = pixel.VertPosition(Corner.BottomLeft);
                tr[0] = _vertMatrix[coord.X, coord.Y].X;
                tr[2] = tr[3] = _vertMatrix[coord.X, coord.Y].Y;  
                var endPixel = pixel;
                x = pixel.Position.X;
                while (x < _xMax && _pixels[x, pixel.Position.Y] != null && !_pixels[x, pixel.Position.Y].BottomCovered)
                {
                    endPixel = _pixels[x, pixel.Position.Y];
                    endPixel.BottomCovered = true;
                    x++;
                }

                coord = endPixel.VertPosition(Corner.BottomRight);
                tr[1] = tr[4] = _vertMatrix[coord.X, coord.Y].X;
                tr[5] = _vertMatrix[coord.X, coord.Y].Y;  
            }
            else if (!pixel.LeftCovered)
            {
                int y = pixel.Position.Y;
                while (y < _yMax && _pixels[pixel.Position.X, y] != null && !_pixels[pixel.Position.X, y].LeftCovered)
                {
                    pixel = _pixels[pixel.Position.X, y];
                    y++;
                }

                var coord = pixel.VertPosition(Corner.TopLeft);
                tr[0] = _vertMatrix[coord.X, coord.Y].X;
                tr[2] = tr[3] = _vertMatrix[coord.X, coord.Y].Y;  
                var endPixel = pixel;
                y = pixel.Position.Y;
                while (y >= _yMin && _pixels[pixel.Position.X, y] != null && !_pixels[pixel.Position.X, y].LeftCovered)
                {
                    endPixel = _pixels[pixel.Position.X, y];
                    endPixel.LeftCovered = true;
                    y--;
                }
                coord = endPixel.VertPosition(Corner.BottomLeft);
                tr[1] = tr[4] = _vertMatrix[coord.X, coord.Y].X;
                tr[5] = _vertMatrix[coord.X, coord.Y].Y; 
            }
            _triangles.AddRange(tr);
        }

        protected override void ClearMemory()
        {
            base.ClearMemory();
            _stripes.Clear();
            _pixels = new PixelDataHigh[0,0];
        }

        private class MeshStripeHigh
        {
            private PixelDataHigh _topLeftPixel;
            private PixelDataHigh _bottomLeftPixel;
            private PixelDataHigh _topRightPixel;
            private PixelDataHigh _bottomRightPixel;

            public MeshStripeHigh(PixelDataHigh startPixel)
            {
                _topLeftPixel = _bottomLeftPixel = _topRightPixel = _bottomRightPixel = startPixel;
            }

            public PixelDataHigh GetPixelAtCorner(Corner corner)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return _topLeftPixel;
                    case Corner.TopRight:
                        return _topRightPixel;
                    case Corner.BottomLeft:
                        return _bottomLeftPixel;
                    case Corner.BottomRight:
                        return _bottomRightPixel;
                    default:
                        return _topLeftPixel;
                }
            }


            public void AddPixelToTheStripe(PixelDataHigh pixel)
            {
                pixel.LeftCovered = true;
                _topRightPixel = _bottomRightPixel = pixel;
            }

            public bool MergeableToTheBottom(MeshStripeHigh stripeHigh)
            {
                if (_bottomLeftPixel.Position.X == stripeHigh._bottomLeftPixel.Position.X && _bottomRightPixel.Position.X == stripeHigh._bottomRightPixel.Position.X && _bottomLeftPixel.Position.Y - stripeHigh._topLeftPixel.Position.Y == 1)
                {
                    MergeStripeTotheBottom(stripeHigh);
                    return true;
                }

                return false;
            }

            private void MergeStripeTotheBottom(MeshStripeHigh stripeHigh)
            {
                _bottomLeftPixel = stripeHigh._bottomLeftPixel;
                _bottomRightPixel = stripeHigh._bottomRightPixel;
            }
        }

        private class PixelDataHigh
        {
            public Int2 Position;

            public bool LeftCovered;
            public bool TopCovered;
            public bool RightCovered;
            public bool BottomCovered;
            private Int2 _size;

            public PixelDataHigh(int x, int y, Int2 size)
            {
                _size = size;
                Position = new Int2(x, y);
            }

            public bool IsCovered()
            {
                return TopCovered && RightCovered && BottomCovered && LeftCovered;
            }

            public Vector3 GetVertCoordinates(Corner corner, Side side, Vector3 scale)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return new Vector3((Position.X - 0.5f)*scale.x, (Position.Y + 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z);
                    case Corner.TopRight:
                        return new Vector3((Position.X + 0.5f)*scale.x, (Position.Y + 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z);
                    case Corner.BottomLeft:
                        return new Vector3((Position.X - 0.5f)*scale.x, (Position.Y - 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z);
                    case Corner.BottomRight:
                        return new Vector3((Position.X + 0.5f)*scale.x, (Position.Y - 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z);
                    default:
                        return Vector3.zero;
                }
            }

            public Int2 VertPosition(Corner corner)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return new Int2(Position.X, Position.Y + 1);
                    case Corner.TopRight:
                        return new Int2(Position.X + 1, Position.Y + 1);
                    case Corner.BottomLeft:
                        return new Int2(Position.X, Position.Y);
                    case Corner.BottomRight:
                        return new Int2(Position.X + 1, Position.Y);
                    default:
                        return new Int2(0,0);
                }
            }

            public Vector2 GetUvAtCorner(Corner corner)
            {
                switch (corner)
                {
                    case Corner.TopLeft:
                        return new Vector2((float)Position.X / _size.X, (float)(Position.Y + 1) / _size.Y);
                    case Corner.TopRight:
                        return new Vector2((float)(Position.X + 1) / (_size.X), (float)(Position.Y + 1) / _size.Y);
                    case Corner.BottomLeft:
                        return new Vector2((float)Position.X / _size.X, (float)Position.Y / _size.Y);
                    case Corner.BottomRight:
                        return new Vector2((float)(Position.X + 1) / (_size.X), (float)Position.Y / _size.Y);
                    default:
                        return Vector2.zero;
                }
            }
        }
    }
}
