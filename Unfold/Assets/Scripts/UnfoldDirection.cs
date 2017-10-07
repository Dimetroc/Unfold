using UnityEngine;

namespace Unfold
{
    public enum UnfoldDirections
    {
        FromCenter,
        ToCenter,
        FromTop,
        FromBottom,
        FromLeft,
        FromRight
    }


    public class UnfoldDirection
    {
        public float Max { get; private set; }
        public float Min { get; private set; }

        private readonly UnfoldDirections _direction;
        private readonly Vector3 _center;

        public UnfoldDirection(UnfoldDirections dir, Vector3 center)
        {
            _direction = dir;
            _center = center;
        }

        public float GetCentroidAnimationValue(Vector3 centroid)
        {
            var dir = GetDirection(centroid).normalized;
            var projection = Vector3.Project(centroid, dir);
            var projectionMag = projection.magnitude;
            if (Vector3.Dot(projection, dir) < 0)
            {
                projectionMag *= -1;
                if (projectionMag < Min)
                {
                    Min = projectionMag;
                }
            }
            else
            {
                if (projectionMag > Max)
                {
                    Max = projectionMag;
                }
            }

            return projectionMag;
        }

        private Vector3 GetDirection(Vector3 centroid)
        {
            switch (_direction)
            {
                case UnfoldDirections.FromCenter:
                    return centroid - _center;
                case UnfoldDirections.ToCenter:
                    return _center - centroid;
                case UnfoldDirections.FromTop:
                    return new Vector3(0, -1, 0);
                case UnfoldDirections.FromLeft:
                    return new Vector3(1, 0, 0);
                case UnfoldDirections.FromBottom:
                    return new Vector3(0, 1, 0);
                case UnfoldDirections.FromRight:
                    return new Vector3(-1, 0, 0);
            }
            return Vector3.zero;
        }
    }
}
