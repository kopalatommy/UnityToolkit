using ProjectWorlds.Geometry._2d;
using System;
using UnityEngine;

namespace ProjectWorlds.Geometry._3d
{
    [System.Serializable]
    public class Sphere : IEquatable<Sphere>, IFormattable
    {
        // The center of the sphere
        public Vector3 Center
        {
            get;
            set;
        }

        // The radius of the sphere
        public float Radius
        {
            get;
            set;
        }

        // Get the area of the sphere
        public float Area
        {
            get
            {
                return 4 * Mathf.PI * Radius * Radius;
            }
        }

        // Get the volume of the sphere
        public float Volume
        {
            get
            {
                return 4.0f / 3.0f * Mathf.PI * Radius * Radius * Radius;
            }
        }

        // Unit sphere has a radius of one and is located at 0,0,0
        public Sphere Unit
        {
            get
            {
                return new Sphere(Vector3.zero, 1);
            }
        }

        public Sphere(float radius)
        {
            Center = Vector3.zero;
            Radius = radius;
        }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Returns a point on the sphere at the given coordinates
        /// </summary>
        /// <param name="horizontalAngle"></param>
        /// <param name="verticalAngle"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float horizontalAngle, float verticalAngle)
        {
            return Center + Geometry3.PointOnSphere(Radius, horizontalAngle, verticalAngle);
        }

        /// <summary>
        /// Returns true if the point interects the sphere
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(Vector3 point)
        {
            return Intersect3.PointSphere(point, Center, Radius);
        }

        /// <summary>
        /// Linearly interpolates between two spheres
        /// </summary>
        public static Sphere Lerp(Sphere a, Sphere b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Sphere(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
        }

        /// <summary>
        /// Linearly interpolates between two spheres without clamping the interpolant
        /// </summary>
        public static Sphere LerpUnclamped(Sphere a, Sphere b, float t)
        {
            return new Sphere(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
        }

        public static explicit operator Circle2(Sphere sphere)
        {
            return new Circle2((Vector2)sphere.Center, sphere.Radius);
        }

        public static Sphere operator +(Sphere sphere, Vector3 vector)
        {
            return new Sphere(sphere.Center + vector, sphere.Radius);
        }

        public static Sphere operator -(Sphere sphere, Vector3 vector)
        {
            return new Sphere(sphere.Center - vector, sphere.Radius);
        }

        public static bool operator ==(Sphere a, Sphere b)
        {
            return a.Center == b.Center && a.Radius == b.Radius;
        }

        public static bool operator !=(Sphere a, Sphere b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ (Radius.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            return other is Sphere && Equals((Sphere)other);
        }

        public bool Equals(Sphere other)
        {
            return Center.Equals(other.Center) && Radius.Equals(other.Radius);
        }

        public override string ToString()
        {
            return string.Format("Sphere(center: {0}, radius: {1})", Center, Radius);
        }

        public string ToString(string format)
        {
            return string.Format("Sphere(center: {0}, radius: {1})", Center.ToString(format), Radius.ToString(format));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Sphere(center: {0}, radius: {1})", Center.ToString(format, formatProvider),
                Radius.ToString(format, formatProvider));
        }
    }
}
