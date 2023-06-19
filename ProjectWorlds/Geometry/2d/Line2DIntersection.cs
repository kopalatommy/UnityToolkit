using UnityEngine;

namespace ProjectWorlds.Geometry._2d
{
    [System.Serializable]
    public class Line2DIntersection
    {
        /// <summary> True if the lines intersect between a and b </summary>
        public bool SegmentsIntersect { get { return segmentsIntersect; } }
        [SerializeField]
        private bool segmentsIntersect;

        /// <summary> The point where the lines intersect </summary>
        public Vector2 Point { get { return point; } }
        [SerializeField]
        private Vector2 point;

        /// <summary> Return the point on the first segment that is closest to the point of intersection </summary>
        public Vector2 ClosestSeg1 { get { return closestSeg1; } }
        [SerializeField]
        private Vector2 closestSeg1;

        /// <summary> Return the point on the second segment that is closest to the point of intersection </summary>
        public Vector2 ClosestSeg2 { get { return closestSeg2; } }
        [SerializeField]
        private Vector2 closestSeg2;

        /// <summary> T value at the intersection on line A </summary>
        public float T1 { get { return t1; } }
        [SerializeField]
        private float t1;

        /// <summary> T value at the intersection on line B </summary>
        public float T2 { get { return t2; } }
        [SerializeField]
        private float t2;

        public Line2DIntersection(bool segments_intersect, Vector2 point, Vector2 closestSeg1, Vector2 closestSeg2, float t1, float t2)
        {
            segmentsIntersect = segments_intersect;
            this.point = point;
            this.closestSeg1 = closestSeg1;
            this.closestSeg2 = closestSeg2;
            this.t1 = t1;
            this.t2 = t2;
        }
    }
}
