using System;
using UnityEngine;

namespace PMB
{
    sealed class LowPMB:BasePMB,IPixelMeshBuilder
    {
        MeshQuality IPixelMeshBuilder.Quality { get {return MeshQuality.Low;} }

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

        private void ReadData(Sprite sprite)
        {

            for (int y = _yMax - 1; y >= _yMin; y--)
            {
                for (int x = _xMin; x < _xMax; x++)
                {
                    if (sprite.texture.GetPixel(x, y).a > 0.0f)
                    {
                        var start = new Int2(x, y);
                        var end = new Int2(x, y);

                        x++;
                        while (sprite.texture.GetPixel(x, y).a > 0.0f && x < _xMax)
                        {
                            end = new Int2(x, y);
                            x++;
                        }

                        BuildData(start, end);
                    }
                }
            }
        }

        private void BuildData(Int2 start, Int2 end)
        {

            var startIndex = _verts.Count;

            _verts.Add(GetVertCoordinates(start,Corner.TopLeft, Side.Front, _scale));
            _verts.Add(GetVertCoordinates(start, Corner.TopLeft, Side.Back, _scale));
            _verts.Add(GetVertCoordinates(end, Corner.TopRight, Side.Front, _scale));
            _verts.Add(GetVertCoordinates(end, Corner.TopRight, Side.Back, _scale));
            _verts.Add(GetVertCoordinates(start, Corner.BottomLeft, Side.Front, _scale));
            _verts.Add(GetVertCoordinates(start, Corner.BottomLeft, Side.Back, _scale));
            _verts.Add(GetVertCoordinates(end, Corner.BottomRight, Side.Front, _scale));
            _verts.Add(GetVertCoordinates(end, Corner.BottomRight, Side.Back, _scale));

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

            _uvs.Add(GetUvAtCorner(start, Corner.TopLeft));
            _uvs.Add(GetUvAtCorner(start, Corner.TopLeft));
            _uvs.Add(GetUvAtCorner(end, Corner.TopRight));
            _uvs.Add(GetUvAtCorner(end, Corner.TopRight));
            _uvs.Add(GetUvAtCorner(start, Corner.BottomLeft));
            _uvs.Add(GetUvAtCorner(start, Corner.BottomLeft));
            _uvs.Add(GetUvAtCorner(end, Corner.BottomRight));
            _uvs.Add(GetUvAtCorner(end, Corner.BottomRight));

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
            _triangles.Add(startIndex + 3);
            _triangles.Add(startIndex + 2);
            _triangles.Add(startIndex + 1);
            _triangles.Add(startIndex + 1);
            _triangles.Add(startIndex + 2);
            _triangles.Add(startIndex + 0);
            _triangles.Add(startIndex + 5);
            _triangles.Add(startIndex + 4);
            _triangles.Add(startIndex + 7);
            _triangles.Add(startIndex + 6);
            _triangles.Add(startIndex + 7);
            _triangles.Add(startIndex + 4);

        }

        private Vector3 GetVertCoordinates(Int2 position, Corner corner, Side side, Vector3 scale)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    return new Vector3((position.X - 0.5f)*scale.x, (position.Y + 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z) - _offset;
                case Corner.TopRight:
                    return new Vector3((position.X + 0.5f)*scale.x, (position.Y + 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z) - _offset;
                case Corner.BottomLeft:
                    return new Vector3((position.X - 0.5f)*scale.x, (position.Y - 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z) - _offset;
                case Corner.BottomRight:
                    return new Vector3((position.X + 0.5f)*scale.x, (position.Y - 0.5f)*scale.y, (side == Side.Front ? -1 : 1)*0.5f*scale.z) - _offset;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector2 GetUvAtCorner(Int2 position, Corner corner)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    return new Vector2((float) position.X/_size.X, (float) (position.Y + 1)/_size.Y);
                case Corner.TopRight:
                    return new Vector2((float) (position.X + 1)/(_size.X), (float) (position.Y + 1)/_size.Y);
                case Corner.BottomLeft:
                    return new Vector2((float) position.X/_size.X, (float) position.Y/_size.Y);
                case Corner.BottomRight:
                    return new Vector2((float) (position.X + 1)/(_size.X), (float) position.Y/_size.Y);
                default:
                    return Vector2.zero;
            }
        }
    }
}
