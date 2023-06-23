using UnityEngine;

namespace ProjectWorlds.Geometry._2d
{
    [System.Serializable]
    public struct Bounds2
    {
        [SerializeField]
        private Vector2 center;
        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        [SerializeField]
        private Vector2 size;
        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        [SerializeField]
        private Vector2 halfExtents;
        public Vector2 HalfExtents
        {
            get
            {
                return halfExtents;
            }
        }

        public float Left
        {
            get
            {
                return center.x - halfExtents.x;
            }
        }

        public float Right
        {
            get
            {
                return center.x + halfExtents.x;
            }
        }

        public float Top
        {
            get
            {
                return center.y + halfExtents.y;
            }
        }

        public float Botton
        {
            get
            {
                return center.y - halfExtents.y;
            }
        }

        public Bounds2(Vector2 center, Vector2 size)
        {
            if (size.x < 0)
            {
                size.x = -size.x;
            }
            if (size.y < 0)
            {
                size.y = -size.y;
            }

            this.center = center;
            this.size = size;
            this.halfExtents = size * 0.5f;
        }

        public Bounds2(Vector2[] verts)
        {
            Vector2 max = verts[0];
            Vector2 min = verts[0];

            foreach (Vector2 vert in verts)
            {
                if (vert.x > max.x)
                {
                    max.x = vert.x;
                }
                else if (vert.x < min.x)
                {
                    min.x = vert.x;
                }

                if (vert.y > max.y)
                {
                    max.y = vert.y;
                }
                else if (vert.y < min.y)
                {
                    min.y = vert.y;
                }
            }

            center = (min + max) * 0.5f;
            size = new Vector2(max.x - min.x, max.y - min.y);
            halfExtents = size * 0.5f;
        }

        public bool Intersects(Bounds2 other)
        {
            Vector2 maxDelta = halfExtents + other.halfExtents;
            Vector2 delta = center - other.center;
            return (Mathf.Abs(delta.x) <= Mathf.Abs(maxDelta.x) && Mathf.Abs(delta.y) <= Mathf.Abs(maxDelta.y));
        }

        public bool Contains(Vector2 point)
        {
            point -= center;
            return Mathf.Abs(point.x) <= halfExtents.x && Mathf.Abs(point.y) <= halfExtents.y;
        }

        public override string ToString()
        {
            return "Bounds2D[" + center + ", " + size + "]";
        }

        public override bool Equals(object obj)
        {
            return obj is Bounds2 && this == (Bounds2)obj;
        }

        public static bool operator ==(Bounds2 a, Bounds2 b)
        {
            return (a.Center == b.Center) && (a.halfExtents == b.halfExtents);
        }

        public static bool operator !=(Bounds2 a, Bounds2 b)
        {
            return (a.Center != b.Center) || (a.halfExtents != b.halfExtents);
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ size.GetHashCode();

        }
    }
}
