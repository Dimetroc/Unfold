using System.Collections.Generic;
using UnityEngine;

namespace Unfold
{
    public class DividedMesh
    {
        public List<Vector3> Vertices { get; private set; }
        public List<Vector3> Normals { get; private set; }
        public List<Vector2> Uvs { get; private set; }
        public List<int> Triangles { get; private set; }

        public Mesh Mesh { get; private set; }

        public DividedMesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Uvs = new List<Vector2>();
            Triangles = new List<int>();

            Mesh = new Mesh();
        }




    }
}
