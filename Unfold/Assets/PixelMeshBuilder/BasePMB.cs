using System.Collections.Generic;
using UnityEngine;

namespace PMB
{
    public interface IPixelMeshBuilder
    {
        MeshQuality Quality { get; }
        Mesh BuildMesh(Sprite sprite, Vector3 scale, bool optimized = false, bool recalcNormals = false);
    }

    public abstract class BasePMB
    {
        protected List<Vector3> _verts;
        protected List<Vector3> _normals;
        protected List<int> _triangles;
        protected List<Vector2> _uvs;
        protected Int2[,] _vertMatrix;
        protected Int2 _size;
        protected Vector3 _scale;
        protected Vector3 _offset;
        protected int _xMin;
        protected int _yMin;
        protected int _xMax;
        protected int _yMax;
        protected bool _recalcNormals;

        protected Vector3 GetNormal(Corner corner, Side side)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    return side == Side.Front ? new Vector3(-0.6f, 0.6f, -0.6f) : new Vector3(-0.6f, 0.6f, 0.6f);
                case Corner.TopRight:
                    return side == Side.Front ? new Vector3(0.6f, 0.6f, -0.6f) : new Vector3(0.6f, 0.6f, 0.6f);
                case Corner.BottomLeft:
                    return side == Side.Front ? new Vector3(-0.6f, -0.6f, -0.6f) : new Vector3(-0.6f, -0.6f, 0.6f);
                case Corner.BottomRight:
                    return side == Side.Front ? new Vector3(0.6f, -0.6f, -0.6f) : new Vector3(0.6f, -0.6f, 0.6f);
                default:
                    return Vector3.zero;
            }
        }

        protected virtual void PrepareVariables(Sprite sprite)
        {
            _xMin = (int)sprite.rect.x;
            _yMin = (int)sprite.rect.y;
            _xMax = (int)sprite.rect.x + (int)sprite.rect.width;
            _yMax = (int)sprite.rect.y + (int)sprite.rect.height;
            _offset = new Vector3(((sprite.rect.width/2.0f) + _xMin - 1)*_scale.x, ((sprite.rect.height/2.0f) + _yMin - 1)*_scale.y, 0.0f);
            _size = new Int2(sprite.texture.width, sprite.texture.height);
            _verts = new List<Vector3>();
            _normals = new List<Vector3>();
            _triangles = new List<int>();
            _uvs = new List<Vector2>();
            _vertMatrix = new Int2[_size.X + 1, _size.Y + 1];
            for (int y = 0; y < _vertMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < _vertMatrix.GetLength(0); x++)
                {
                    _vertMatrix[x, y] = new Int2(-1, -1);
                }
            }
        }

        protected virtual void ClearMemory()
        {
            _verts.Clear();
            _normals.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _vertMatrix = new Int2[0,0];
        }

        protected struct Int2
        {
            public int X;
            public int Y;

            public override string ToString()
            {
                return "(" + X + "," + Y + ")";
            }

            public Int2(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        protected enum Side
        {
            Front = 0,
            Back = 1
        }

        protected enum Corner
        {
            TopLeft = 0,
            TopRight = 2,
            BottomLeft = 4,
            BottomRight = 6,
        }
    }

    public enum MeshQuality
    {
        Low,
        Medium,
        High
    }
}
