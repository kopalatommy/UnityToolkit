using System;
using UnityEngine;

namespace ProjectWorlds.Geometry._2d
{
    [System.Serializable]
    public class Line2d
    {
        [SerializeField]
        private Vector2 start;
        public Vector2 Start
        {
            get
            {
                return start;
            }
        }

        [SerializeField]
        private Vector2 end;
        public Vector2 End
        {
            get
            {
                return End;
            }
        }

        [SerializeField]
        private Vector2 difference;
        public Vector2 Difference
        {
            get
            {
                return difference;
            }
        }

        [SerializeField]
        private Vector2 direction;
        public Vector2 Direction
        {
            get
            {
                return direction;
            }
        }

        [SerializeField]
        private float length;
        public float Length
        {
            get
            {
                return length;
            }
        }

        [SerializeField]
        private Vector2 Normal
        {
            get
            {
                return difference.Rotate90().normalized;
            }
        }

        [SerializeField]
        private Bounds2d bounds;
        public Bounds2d Bounds
        {
            get
            {
                return bounds;
            }
        }

        public Vector2 Center
        {
            get
            {
                return (start + end) * 0.5f;
            }
        }

        public Line2d(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
            difference = end - start;
            length = difference.magnitude;
            this.bounds = new Bounds2d((start + end) * 0.5f, difference);
        }

        /// <summary>
        /// Rotate the line around the world centers
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public Line2d Rotate(float rad)
        {
            return new Line2d(start.Rotate(rad), end.Rotate(rad));
        }

        /// <summary>
        /// Rotate the line around the specified pivot
        /// </summary>
        /// <param name="rad"></param>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public Line2d Rotate(float rad, Vector2 pivot)
        {
            return new Line2d((start - pivot).Rotate90() + pivot, (end - pivot).Rotate90() + pivot);
        }

        public Line2d Rotate90()
        {
            return new Line2d(start.Rotate90(), end.Rotate90());
        }

        public Line2d Rotate90(Vector2 pivot)
        {
            return new Line2d((start - pivot).Rotate90() + pivot, (end - pivot).Rotate90() + pivot);
        }

        public Line2d RotateNeg90()
        {
            return new Line2d(start.RotateNeg90(), end.RotateNeg90());
        }

        public Line2d RotateNeg90(Vector2 pivot)
        {
            return new Line2d((start - pivot).RotateNeg90() + pivot, (end - pivot).RotateNeg90() + pivot);
        }

        public Line2d Flip()
        {
            return new Line2d(-start, -end);
        }

        public Line2d Flip(Vector2 pivot)
        {
            return new Line2d((-(start - pivot) + pivot), (-(end - pivot) + pivot));
        }

        public Line2d Translate(Vector2 offset)
        {
            return new Line2d(start + offset, end + offset);
        }

        public Line2d Crop(float startDist, float endDist)
        {
            return new Line2d(LerpDistance(startDist), LerpDistance(length - endDist));
        }

        public Vector2 Lerp(float t)
        {
            return start + (difference * t);
        }

        public Vector2 LerpClamped(float t)
        {
            return Lerp(Mathf.Clamp01(t));
        }

        public Vector2 LerpDistance(float d)
        {
            return start + (difference.normalized * d);
        }

        public Vector2 LerpDistanceClamped(float d)
        {
            return start + (difference.normalized * Mathf.Clamp01(d));
        }

        public Vector2 Project(Vector2 p)
        {
            Vector2 a = difference.normalized;
            p.Normalize();
            return Vector2.Dot(a, p) * p;
        }

        public float PositionAlongLine(Vector2 p)
        {
            Vector2 otherAB = difference.Rotate90();
            float denominator = (difference.y * otherAB.x - difference.x * otherAB.y);
            float t1 =
                ((start.x - p.x) * otherAB.y + (p.y - start.y) * otherAB.x)
                    / denominator;
            return t1;
        }

        public Vector2 GetClosestPoint(Vector2 p)
        {
            return start + (difference * PositionAlongLine(p));
        }

        public Vector2 GetClosestPointSegment(Vector2 p)
        {
            return start + (difference * Mathf.Clamp01(PositionAlongLine(p)));
        }

        public float DistanceFromPointSegment(Vector2 p)
        {
            return Vector2.Distance(GetClosestPointSegment(p), p);
        }

        public bool Intersect(Line2d other, out Line2DIntersection intersection)
        {
            float denominator = (difference.y * other.difference.x - difference.x * other.difference.y);

            float t1 =
                ((start.x - other.start.x) * other.difference.y + (other.start.y - start.y) * other.difference.x)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                intersection = new Line2DIntersection(false, Vector2.zero, Vector2.zero, Vector2.zero, 0, 0);
                return false;
            }

            float t2 =
                ((other.start.x - start.x) * difference.y + (start.y - other.start.y) * difference.x)
                    / -denominator;

            // Find the point of intersection.
            Vector2 i_point = new Vector2(start.x + difference.x * t1, start.y + difference.y * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            bool segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            float t1cached = t1;
            float t2cached = t2;

            // Find the closest points on the segments.
            if (t1 < 0) t1 = 0;
            else if (t1 > 1) t1 = 1;
            if (t2 < 0) t2 = 0;
            else if (t2 > 1) t2 = 1;

            Vector2 close_p1 = start + (difference * t1);
            Vector2 close_p2 = other.start + (other.difference * t2);
            intersection = new Line2DIntersection(segments_intersect, i_point, close_p1, close_p2, t1cached, t2cached);
            return true;
        }

        public bool Intersect(Vector2 point, out Line2DIntersection intersection)
        {
            Line2d other = new Line2d(point, point + Normal);
            return Intersect(other, out intersection);
        }

        public bool SegmentsIntersect(Line2d other)
        {
            Line2DIntersection intersection;
            if (Intersect(other, out intersection))
            {
                if (intersection.SegmentsIntersect) return true;
            }
            return false;
        }

        public void DebugDraw(Color color)
        {
#if UNITY_EDITOR
            Debug.DrawLine(new Vector3(start.x, 0, start.y), new Vector3(end.x, 0, end.y), color);
            Debug.Log(new Vector3(start.x, 0, start.y) + " " + new Vector3(end.x, 0, end.y));
#endif
        }

        public override string ToString()
        {
            return "Line2D[" + start + "," + end + "]";
        }

        public override bool Equals(System.Object obj)
        {
            return obj is Line2d && this == (Line2d)obj;
        }
        public override int GetHashCode()
        {
            return start.GetHashCode() ^ end.GetHashCode();
        }

        public static bool operator ==(Line2d a, Line2d b)
        {
            return (a.start == b.start) && (a.end == b.end);
        }

        public static bool operator !=(Line2d a, Line2d b)
        {
            return !(a == b);
        }
    }
}
