using UnityEngine;

namespace PMB
{
    public class PixelMeshBuilder
    {
        private IPixelMeshBuilder _builder = null;

        /// <summary>
        /// Create instance of the builder class
        /// </summary>
        /// <param name="quality">Quality of the final mesh, how optimized it is.</param>
        public PixelMeshBuilder(MeshQuality quality)
        {
            switch (quality)
            {
                case MeshQuality.Low:
                    _builder = new LowPMB();
                    break;
                case MeshQuality.Medium:
                    _builder = new MediumPMB();
                    break;
                case MeshQuality.High:
                    _builder = new HighPMB();
                    break;
                default:
                    Debug.LogError("Unknown quality level!");
                    break;
            }
        }

        /// <summary>
        /// Creates mesh out of sprite.
        /// </summary>
        /// <param name="sprite">Source sprite for the mesh</param>
        /// <param name="scale">Scale - sets size of the final mesh</param>
        /// <param name="optimized">Setting optimized to true will run standart unity mesh optimization before returning the mesh</param>
        /// <param name="recalcNormals">Recalc normals determins if normals will be calculated by standart unity method or set by the script</param>
        /// <returns></returns>
        public Mesh BuildMesh(Sprite sprite, Vector3 scale, bool optimized = true, bool recalcNormals = false)
        {
            return _builder.BuildMesh(sprite, scale, optimized, recalcNormals);
        }
    }
}
