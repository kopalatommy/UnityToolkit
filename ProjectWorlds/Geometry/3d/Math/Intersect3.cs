using ProjectWorlds.Geometry.Intersections;
using UnityEngine;

namespace ProjectWorlds.Geometry._3d
{
    /// <summary>
    /// Functions to test if 
    /// </summary>
    public static class Intersect3
    {
        #region Point-Line

        /// <summary>
        /// Tests if the point lies on the line
        /// </summary>
        public static bool PointLine(Vector3 point, Line3 line)
        {
            return PointLine(point, line.origin, line.direction);
        }

        /// <summary>
        /// Tests if the point lies on the line
        /// </summary>
        public static bool PointLine(Vector3 point, Vector3 lineOrigin, Vector3 lineDirection)
        {
            return Distance3.PointLine(point, lineOrigin, lineDirection) < Geometry3.Epsilon;
        }

        #endregion Point-Line

        #region Point-Ray

        /// <summary>
        /// Tests if the point lies on the ray
        /// </summary>
        public static bool PointRay(Vector3 point, Ray ray)
        {
            return PointRay(point, ray.origin, ray.direction);
        }

        /// <summary>
        /// Tests if the point lies on the ray
        /// </summary>
        public static bool PointRay(Vector3 point, Vector3 rayOrigin, Vector3 rayDirection)
        {
            return Distance3.PointRay(point, rayOrigin, rayDirection) < Geometry3.Epsilon;
        }

        #endregion Point-Ray

        #region Point-Segment

        /// <summary>
        /// Tests if the point lies on the segment
        /// </summary>
        public static bool PointSegment(Vector3 point, Segment3 segment)
        {
            return PointSegment(point, segment.a, segment.b);
        }

        /// <summary>
        /// Tests if the point lies on the segment
        /// </summary>
        public static bool PointSegment(Vector3 point, Vector3 segmentA, Vector3 segmentB)
        {
            return Distance3.PointSegment(point, segmentA, segmentB) < Geometry3.Epsilon;
        }

        #endregion Point-Segment

        #region Point-Sphere

        /// <summary>
        /// Tests if the point is inside the sphere
        /// </summary>
        public static bool PointSphere(Vector3 point, Sphere sphere)
        {
            return PointSphere(point, sphere.Center, sphere.Radius);
        }

        /// <summary>
        /// Tests if the point is inside the sphere
        /// </summary>
        public static bool PointSphere(Vector3 point, Vector3 sphereCenter, float sphereRadius)
        {
            // For points on the sphere's surface magnitude is more stable than sqrMagnitude
            return (point - sphereCenter).magnitude < sphereRadius + Geometry3.Epsilon;
        }

        #endregion Point-Sphere

        #region Line-Line

        /// <summary>
        /// Computes an intersection of the lines
        /// </summary>
        public static bool LineLine(Line3 lineA, Line3 lineB)
        {
            return LineLine(lineA.origin, lineA.direction, lineB.origin, lineB.direction, out Vector3 intersection);
        }

        /// <summary>
        /// Computes an intersection of the lines
        /// </summary>
        public static bool LineLine(Line3 lineA, Line3 lineB, out Vector3 intersection)
        {
            return LineLine(lineA.origin, lineA.direction, lineB.origin, lineB.direction, out intersection);
        }

        /// <summary>
        /// Computes an intersection of the lines
        /// </summary>
        public static bool LineLine(Vector3 originA, Vector3 directionA, Vector3 originB, Vector3 directionB)
        {
            return LineLine(originA, directionA, originB, directionB, out Vector3 intersection);
        }

        /// <summary>
        /// Computes an intersection of the lines
        /// </summary>
        public static bool LineLine(Vector3 originA, Vector3 directionA, Vector3 originB, Vector3 directionB,
            out Vector3 intersection)
        {
            float sqrMagnitudeA = directionA.sqrMagnitude;
            float sqrMagnitudeB = directionB.sqrMagnitude;
            float dotAB = Vector3.Dot(directionA, directionB);

            float denominator = sqrMagnitudeA * sqrMagnitudeB - dotAB * dotAB;
            Vector3 originBToA = originA - originB;
            float a = Vector3.Dot(directionA, originBToA);
            float b = Vector3.Dot(directionB, originBToA);

            Vector3 closestPointA;
            Vector3 closestPointB;
            if (Mathf.Abs(denominator) < Geometry3.Epsilon)
            {
                // Parallel
                float distanceB = dotAB > sqrMagnitudeB ? a / dotAB : b / sqrMagnitudeB;

                closestPointA = originA;
                closestPointB = originB + directionB * distanceB;
            }
            else
            {
                // Not parallel
                float distanceA = (sqrMagnitudeA * b - dotAB * a) / denominator;
                float distanceB = (dotAB * b - sqrMagnitudeB * a) / denominator;

                closestPointA = originA + directionA * distanceA;
                closestPointB = originB + directionB * distanceB;
            }

            if ((closestPointB - closestPointA).sqrMagnitude < Geometry3.Epsilon)
            {
                intersection = closestPointA;
                return true;
            }
            intersection = Vector3.zero;
            return false;
        }

        #endregion Line-Line

        #region Line-Sphere

        /// <summary>
        /// Computes an intersection of the line and the sphere
        /// </summary>
        public static bool LineSphere(Line3 line, Sphere sphere)
        {
            return LineSphere(line.origin, line.direction, sphere.Center, sphere.Radius, out IntersectionLineSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the line and the sphere
        /// </summary>
        public static bool LineSphere(Line3 line, Sphere sphere, out IntersectionLineSphere intersection)
        {
            return LineSphere(line.origin, line.direction, sphere.Center, sphere.Radius, out intersection);
        }

        /// <summary>
        /// Computes an intersection of the line and the sphere
        /// </summary>
        public static bool LineSphere(Vector3 lineOrigin, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius)
        {
            return LineSphere(lineOrigin, lineDirection, sphereCenter, sphereRadius, out IntersectionLineSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the line and the sphere
        /// </summary>
        public static bool LineSphere(Vector3 lineOrigin, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius,
            out IntersectionLineSphere intersection)
        {
            Vector3 originToCenter = sphereCenter - lineOrigin;
            float centerProjection = Vector3.Dot(lineDirection, originToCenter);
            float sqrDistanceToLine = originToCenter.sqrMagnitude - centerProjection * centerProjection;

            float sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
            if (sqrDistanceToIntersection < -Geometry3.Epsilon)
            {
                intersection = IntersectionLineSphere.None();
                return false;
            }
            if (sqrDistanceToIntersection < Geometry3.Epsilon)
            {
                intersection = IntersectionLineSphere.Point(lineOrigin + lineDirection * centerProjection);
                return true;
            }

            float distanceToIntersection = Mathf.Sqrt(sqrDistanceToIntersection);
            float distanceA = centerProjection - distanceToIntersection;
            float distanceB = centerProjection + distanceToIntersection;

            Vector3 pointA = lineOrigin + lineDirection * distanceA;
            Vector3 pointB = lineOrigin + lineDirection * distanceB;
            intersection = IntersectionLineSphere.TwoPoints(pointA, pointB);
            return true;
        }

        #endregion Line-Sphere

        #region Ray-Sphere

        /// <summary>
        /// Computes an intersection of the ray and the sphere
        /// </summary>
        public static bool RaySphere(Ray ray, Sphere sphere)
        {
            return RaySphere(ray.origin, ray.direction, sphere.Center, sphere.Radius, out IntersectionRaySphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the ray and the sphere
        /// </summary>
        public static bool RaySphere(Ray ray, Sphere sphere, out IntersectionRaySphere intersection)
        {
            return RaySphere(ray.origin, ray.direction, sphere.Center, sphere.Radius, out intersection);
        }

        /// <summary>
        /// Computes an intersection of the ray and the sphere
        /// </summary>
        public static bool RaySphere(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius)
        {
            return RaySphere(rayOrigin, rayDirection, sphereCenter, sphereRadius, out IntersectionRaySphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the ray and the sphere
        /// </summary>
        public static bool RaySphere(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius,
            out IntersectionRaySphere intersection)
        {
            Vector3 originToCenter = sphereCenter - rayOrigin;
            float centerProjection = Vector3.Dot(rayDirection, originToCenter);
            if (centerProjection + sphereRadius < -Geometry3.Epsilon)
            {
                intersection = IntersectionRaySphere.None();
                return false;
            }

            float sqrDistanceToLine = originToCenter.sqrMagnitude - centerProjection * centerProjection;
            float sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
            if (sqrDistanceToIntersection < -Geometry3.Epsilon)
            {
                intersection = IntersectionRaySphere.None();
                return false;
            }
            if (sqrDistanceToIntersection < Geometry3.Epsilon)
            {
                if (centerProjection < -Geometry3.Epsilon)
                {
                    intersection = IntersectionRaySphere.None();
                    return false;
                }
                intersection = IntersectionRaySphere.Point(rayOrigin + rayDirection * centerProjection);
                return true;
            }

            // Line intersection
            float distanceToIntersection = Mathf.Sqrt(sqrDistanceToIntersection);
            float distanceA = centerProjection - distanceToIntersection;
            float distanceB = centerProjection + distanceToIntersection;

            if (distanceA < -Geometry3.Epsilon)
            {
                if (distanceB < -Geometry3.Epsilon)
                {
                    intersection = IntersectionRaySphere.None();
                    return false;
                }
                intersection = IntersectionRaySphere.Point(rayOrigin + rayDirection * distanceB);
                return true;
            }

            Vector3 pointA = rayOrigin + rayDirection * distanceA;
            Vector3 pointB = rayOrigin + rayDirection * distanceB;
            intersection = IntersectionRaySphere.TwoPoints(pointA, pointB);
            return true;
        }

        #endregion Ray-Sphere

        #region Segment-Sphere

        /// <summary>
        /// Computes an intersection of the segment and the sphere
        /// </summary>
        public static bool SegmentSphere(Segment3 segment, Sphere sphere)
        {
            return SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out IntersectionSegmentSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the segment and the sphere
        /// </summary>
        public static bool SegmentSphere(Segment3 segment, Sphere sphere, out IntersectionSegmentSphere intersection)
        {
            return SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out intersection);
        }

        /// <summary>
        /// Computes an intersection of the segment and the sphere
        /// </summary>
        public static bool SegmentSphere(Vector3 segmentA, Vector3 segmentB, Vector3 sphereCenter, float sphereRadius)
        {
            return SegmentSphere(segmentA, segmentB, sphereCenter, sphereRadius, out IntersectionSegmentSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the segment and the sphere
        /// </summary>
        public static bool SegmentSphere(Vector3 segmentA, Vector3 segmentB, Vector3 sphereCenter, float sphereRadius,
            out IntersectionSegmentSphere intersection)
        {
            Vector3 segmentAToCenter = sphereCenter - segmentA;
            Vector3 fromAtoB = segmentB - segmentA;
            float segmentLength = fromAtoB.magnitude;
            if (segmentLength < Geometry3.Epsilon)
            {
                float distanceToPoint = segmentAToCenter.magnitude;
                if (distanceToPoint < sphereRadius + Geometry3.Epsilon)
                {
                    if (distanceToPoint > sphereRadius - Geometry3.Epsilon)
                    {
                        intersection = IntersectionSegmentSphere.Point(segmentA);
                        return true;
                    }
                    intersection = IntersectionSegmentSphere.None();
                    return true;
                }
                intersection = IntersectionSegmentSphere.None();
                return false;
            }

            Vector3 segmentDirection = fromAtoB.normalized;
            float centerProjection = Vector3.Dot(segmentDirection, segmentAToCenter);
            if (centerProjection + sphereRadius < -Geometry3.Epsilon ||
                centerProjection - sphereRadius > segmentLength + Geometry3.Epsilon)
            {
                intersection = IntersectionSegmentSphere.None();
                return false;
            }

            float sqrDistanceToLine = segmentAToCenter.sqrMagnitude - centerProjection * centerProjection;
            float sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
            if (sqrDistanceToIntersection < -Geometry3.Epsilon)
            {
                intersection = IntersectionSegmentSphere.None();
                return false;
            }

            if (sqrDistanceToIntersection < Geometry3.Epsilon)
            {
                if (centerProjection < -Geometry3.Epsilon ||
                    centerProjection > segmentLength + Geometry3.Epsilon)
                {
                    intersection = IntersectionSegmentSphere.None();
                    return false;
                }
                intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * centerProjection);
                return true;
            }

            // Line intersection
            float distanceToIntersection = Mathf.Sqrt(sqrDistanceToIntersection);
            float distanceA = centerProjection - distanceToIntersection;
            float distanceB = centerProjection + distanceToIntersection;

            bool pointAIsAfterSegmentA = distanceA > -Geometry3.Epsilon;
            bool pointBIsBeforeSegmentB = distanceB < segmentLength + Geometry3.Epsilon;

            if (pointAIsAfterSegmentA && pointBIsBeforeSegmentB)
            {
                Vector3 pointA = segmentA + segmentDirection * distanceA;
                Vector3 pointB = segmentA + segmentDirection * distanceB;
                intersection = IntersectionSegmentSphere.TwoPoints(pointA, pointB);
                return true;
            }
            if (!pointAIsAfterSegmentA && !pointBIsBeforeSegmentB)
            {
                // The segment is inside, but no intersection
                intersection = IntersectionSegmentSphere.None();
                return true;
            }

            bool pointAIsBeforeSegmentB = distanceA < segmentLength + Geometry3.Epsilon;
            if (pointAIsAfterSegmentA && pointAIsBeforeSegmentB)
            {
                // Point A intersection
                intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * distanceA);
                return true;
            }
            bool pointBIsAfterSegmentA = distanceB > -Geometry3.Epsilon;
            if (pointBIsAfterSegmentA && pointBIsBeforeSegmentB)
            {
                // Point B intersection
                intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * distanceB);
                return true;
            }

            intersection = IntersectionSegmentSphere.None();
            return false;
        }

        #endregion Segment-Sphere

        #region Sphere-Sphere

        /// <summary>
        /// Computes an intersection of the spheres
        /// </summary>
        /// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
        public static bool SphereSphere(Sphere sphereA, Sphere sphereB)
        {
            return SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius, out IntersectionSphereSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the spheres
        /// </summary>
        /// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
        public static bool SphereSphere(Sphere sphereA, Sphere sphereB, out IntersectionSphereSphere intersection)
        {
            return SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius, out intersection);
        }

        /// <summary>
        /// Computes an intersection of the spheres
        /// </summary>
        /// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
        public static bool SphereSphere(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB)
        {
            return SphereSphere(centerA, radiusA, centerB, radiusB, out IntersectionSphereSphere intersection);
        }

        /// <summary>
        /// Computes an intersection of the spheres
        /// </summary>
        /// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
        public static bool SphereSphere(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB,
            out IntersectionSphereSphere intersection)
        {
            Vector3 fromBtoA = centerA - centerB;
            float distanceFromBtoASqr = fromBtoA.sqrMagnitude;
            if (distanceFromBtoASqr < Geometry3.Epsilon)
            {
                if (Mathf.Abs(radiusA - radiusB) < Geometry3.Epsilon)
                {
                    // Spheres are coincident
                    intersection = IntersectionSphereSphere.Sphere(centerA, radiusA);
                    return true;
                }
                // One sphere is inside the other
                intersection = IntersectionSphereSphere.None();
                return true;
            }

            // For intersections on the sphere's edge magnitude is more stable than sqrMagnitude
            float distanceFromBtoA = Mathf.Sqrt(distanceFromBtoASqr);

            float sumOfRadii = radiusA + radiusB;
            if (Mathf.Abs(distanceFromBtoA - sumOfRadii) < Geometry3.Epsilon)
            {
                // One intersection outside
                intersection = IntersectionSphereSphere.Point(centerB + fromBtoA * (radiusB / sumOfRadii));
                return true;
            }
            if (distanceFromBtoA > sumOfRadii)
            {
                // No intersections, spheres are separate
                intersection = IntersectionSphereSphere.None();
                return false;
            }

            float differenceOfRadii = radiusA - radiusB;
            float differenceOfRadiiAbs = Mathf.Abs(differenceOfRadii);
            if (Mathf.Abs(distanceFromBtoA - differenceOfRadiiAbs) < Geometry3.Epsilon)
            {
                // One intersection inside
                intersection = IntersectionSphereSphere.Point(centerB - fromBtoA * (radiusB / differenceOfRadii));
                return true;
            }
            if (distanceFromBtoA < differenceOfRadiiAbs)
            {
                // One sphere is contained within the other
                intersection = IntersectionSphereSphere.None();
                return true;
            }

            // Circle intersection
            float radiusASqr = radiusA * radiusA;
            float distanceToMiddle = 0.5f * (radiusASqr - radiusB * radiusB) / distanceFromBtoASqr + 0.5f;
            Vector3 middle = centerA - fromBtoA * distanceToMiddle;

            float discriminant = radiusASqr / distanceFromBtoASqr - distanceToMiddle * distanceToMiddle;
            float radius = distanceFromBtoA * Mathf.Sqrt(discriminant);

            intersection = IntersectionSphereSphere.Circle(middle, -fromBtoA.normalized, radius);
            return true;
        }

        #endregion Sphere-Sphere
    }
}
