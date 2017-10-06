using UnityEngine;

namespace Unfold
{
    public class UnfoldDirection
    {
        private readonly Vector3 _direction;

        public float Max { get; private set; }
        public float Min { get; private set; }

        public UnfoldDirection(Vector3 direction)
        {
            _direction = direction.normalized;
            Max = Min = 0;
        }

        public float GetCentroidAnimationValue(Vector3 centroid)
        {
            
            var projection = Vector3.Project(centroid, _direction);
            //Debug.Log(centroid + "|" + projection + "|" + Vector3.Dot(projection, _direction) + "|" + projection.magnitude);
            var projectionMag = projection.magnitude;
            if (Vector3.Dot(projection, _direction) < 0)
            {
                projectionMag *= -1;
                if (projectionMag < Min) Min = projectionMag;
            }
            else
            {
                if (projectionMag > Max) Max = projectionMag;
            }

            return projectionMag;
        }

        public float GetCentroidRadiusAnimationValue(Vector3 centroid)
        {

            //Debug.Log(centroid + "|" + projection + "|" + Vector3.Dot(projection, _direction) + "|" + projection.magnitude);
            var projectionMag = centroid.magnitude;
            Min = 0;
            if (projectionMag > Max) Max = projectionMag;

            return projectionMag;
        }
    }
}
