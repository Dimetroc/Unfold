using UnityEngine;

namespace Unfold
{
    public class DividedTriangle
    {
        private Vector3 _a0;
        private Vector3 _b0;
        private Vector3 _c0;

        public Vector3 Centroid
        {
            get
            {
                return new Vector3((_a0.x + _b0.x + _c0.x)/3.0f, (_a0.y + _b0.y + _c0.y) / 3.0f, (_a0.z + _b0.z + _c0.z) / 3.0f);
            }
        }


    }
}
