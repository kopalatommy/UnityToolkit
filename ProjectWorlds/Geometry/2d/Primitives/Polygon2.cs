using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.Geometry._2d
{
    // Immutable representation of a polygon defined by an arbitrary number of points in 2D space
    [System.Serializable]
    public struct Polygon2
    {
        /// <summary> Verts in local space. </summary>
        public Vector2[] Verts
        {
            get
            {
                return verts;
            }
        }
        [SerializeField] private Vector2[] verts;

        /// <summary> Total polygon area. </summary>
        public float Area
        {
            get
            {
                return area;
            }
        }
        [SerializeField] private float area;

        /// <summary> Triangle indices </summary>
        public int[] TriIndices
        {
            get
            {
                return triIndices;
            }
        }
        [SerializeField] private int[] triIndices;

        /// <summary> Triangle count </summary>
        public int TriCount
        {
            get
            {
                return triCount;
            }
        }
        [SerializeField] private int triCount;

        /// <summary> Area in square units for each triangle </summary>
        public Triangle2[] Tris
        {
            get
            {
                return tris;
            }
        }
        [SerializeField] private Triangle2[] tris;

        /// <summary> Get the bounds of the polygon </summary>
        public Bounds2 Bounds
        {
            get
            {
                return bounds;
            }
        }
        [SerializeField] private Bounds2 bounds;

        /// <summary> Get the bounds of the polygon </summary>
        public Line2[] Edges
        {
            get
            {
                return edges;
            }
        }
        [SerializeField] private Line2[] edges;

        /// <summary> Returns true if polygon verts are in clockwise order </summary>
        public bool IsClockwise
        {
            get
            {
                return isClockwise;
            }
        }
        [SerializeField] private bool isClockwise;

        /// <summary> Returns a centered 2x2 quad </summary>
        public static Polygon2 Quad
        {
            get
            {
                return new Polygon2(new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1));
            }
        }

        /// <summary> Returns a centered quad with set height/width </summary>
        public Polygon2 Rect(float width, float height)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            return new Polygon2(new Vector2(-halfWidth, halfHeight), new Vector2(halfWidth, halfHeight), new Vector2(halfWidth, -halfHeight), new Vector2(-halfWidth, -halfHeight));
        }

        /// <summary> Constructs and returns a centered, circular Polygon2D with a set number of segments. </summary>
        /// <param name="segments">4 or more segments recommended. </param>
        public Polygon2 Circle(int segments)
        {
            Vector2[] verts = new Vector2[segments];
            for (int i = 0; i < segments; i++)
            {
                float rad = ((float)i / segments) * Mathf.PI * 2;
                verts[i] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
            return new Polygon2(verts);
        }

        /// <summary> Constructor </summary>
        public Polygon2(params Vector2[] verts)
        {
            if (verts.Length < 4)
            {
                Debug.LogWarning("You cannot create a polygon with less than 4 verts. Input count: " + verts.Length);
                verts = new Vector2[] { new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
            }
            this.verts = verts;
            triIndices = Triangulate(verts);
            float area = Geometry2.ShoelaceFormula(this.verts);
            this.area = Mathf.Abs(area);
            triCount = triIndices.Length / 3;
            bounds = new Bounds2(verts);
            edges = GetEdges(this.verts);
            tris = GetTriangles(this.verts, triIndices);
            isClockwise = area < 0;
        }

        /// <summary> Convert Polygon2D to a mesh </summary>
        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = Array.ConvertAll(Verts, x => new Vector3(x.x, 0, x.y));
            mesh.triangles = TriIndices;
            Vector2[] uvs = new Vector2[Verts.Length];
            Array.Copy(Verts, uvs, Verts.Length);
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] -= (Bounds.Center + Bounds.HalfExtents);
                uvs[i].x /= Bounds.Size.x;
                uvs[i].y /= Bounds.Size.y;
            }
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary> Returns a random local point inside the Area2D </summary>
        public Vector2 GetRandomPoint()
        {
            int tri = GetWeightedTriIndex();
            return Tris[tri].GetRandomPoint();

        }

        /// <summary> Returns true if local point is inside this poly </summary>
        public bool Contains(Vector2 point)
        {
            for (int i = 0; i < TriCount; i++)
            {
                if (Tris[i].Contains(point)) return true;
            }
            return false;
        }

        /// <summary> Returns true if poly is completely inside this poly </summary>
        public bool Contains(Polygon2 poly)
        {
            //Check if poly contains all lines
            return (Contains(poly.Edges));
        }

        /// <summary> Returns true if poly is completely inside this poly </summary>
        public bool Contains(params Line2[] lines)
        {
            if (lines.Length == 0)
            {
                return true;
            }
            if (!Contains(lines[0].Start))
            {
                return false;
            }
            //Possible optimization: Check for contains a/b instead of check intersect?
            return (!IntersectEdge(lines));
        }

        /// <summary> Returns true if polys intersect eachother</summary>
        public bool Intersect(Polygon2 poly)
        {
            //Possible optimization: bounds check?
            //Start by performing a quick bounds test
            //if (!bounds.Intersects(poly.bounds)) return false;
            //Test against all edges
            if (Contains(poly.Edges[0].Center))
            {
                return true;
            }
            if (poly.Contains(Edges[0].Center))
            {
                return true;
            }
            return (IntersectEdge(poly.Edges));
        }

        /// <summary> Return true if any line intersects the edge of this polygon </summary>
        public bool IntersectEdge(params Line2[] lines)
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                for (int k = 0; k < lines.Length; k++)
                {
                    if (Edges[i].SegmentsIntersect(lines[k])) return true;
                }
            }
            return false;
        }

        public Line2 GetNearestEdge(Vector2 point, out Vector2 intersection)
        {
            Line2 output = new Line2(Vector2.zero, Vector2.zero);
            intersection = Vector3.zero;
            float dist = Mathf.Infinity;
            for (int i = 0; i < Verts.Length; i++)
            {

                Line2 line = new Line2(
                    Verts[i],
                    (i == Verts.Length - 1) ? Verts[0] : Verts[i + 1]);
                Vector2 newIntersection = line.GetClosestPointSegment(point);
                float newDist = Vector2.Distance(point, newIntersection);
                if (newDist < dist)
                {
                    dist = newDist;
                    output = line;
                    intersection = newIntersection;
                }
            }
            return output;
        }

        /// <summary> Returns a Polygon2D with its verts reversed </summary>
        public Polygon2 Flip()
        {
            Vector2[] verts = new Vector2[this.Verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = this.Verts[(verts.Length - 1) - i];
            }
            return new Polygon2(verts);
        }

        public override string ToString()
        {
            return "Polygon2D[" + string.Join(",", Array.ConvertAll(Verts, x => x.ToString())) + "]";
        }

        public override bool Equals(System.Object obj)
        {
            return obj is Polygon2 && this == (Polygon2)obj;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < verts.Length; i++)
            {
                hash ^= verts[i].GetHashCode();
            }
            return hash;
        }

        public static bool operator ==(Polygon2 a, Polygon2 b)
        {
            if (a.verts.Length != b.verts.Length) return false;
            for (int i = 0; i < a.verts.Length; i++)
            {
                if (a.verts[i] != b.verts[i]) return false;
            }
            return true;
        }

        public static bool operator !=(Polygon2 a, Polygon2 b)
        {
            return !(a == b);
        }

        /// <summary> Get a random triangle based on relative size of triangles </summary>
        private int GetWeightedTriIndex()
        {
            float r = UnityEngine.Random.Range(0, this.Area);
            float area = 0;
            for (int i = 0; i < Tris.Length; i++)
            {
                area += Tris[i].area;
                if (r <= area) return i;
            }
            return -1;
        }

        /// <summary> Returns all edges </summary>
        private static Line2[] GetEdges(Vector2[] verts)
        {
            Line2[] edges = new Line2[verts.Length];
            for (int i = 0; i < edges.Length; i++) edges[i] = new Line2(
                verts[i],
                i < verts.Length - 1 ? verts[i + 1] : verts[0]
                );
            return edges;
        }

        private static Triangle2[] GetTriangles(Vector2[] verts, int[] triIndices)
        {
            Triangle2[] tris = new Triangle2[triIndices.Length / 3];
            for (int i = 0; i < tris.Length; i++) tris[i] = new Triangle2(
                verts[triIndices[i * 3]],
                verts[triIndices[i * 3 + 1]],
                verts[triIndices[i * 3 + 2]]
                );
            return tris;
        }

        private static int[] Triangulate(Vector2[] verts)
        {
            List<int> indices = new List<int>();

            int n = verts.Length;
            if (n < 3)
            {
                return indices.ToArray();
            }

            int[] V = new int[n];
            if (GetArea(verts) > 0)
            {
                for (int v = 0; v < n; v++)
                {
                    V[v] = v;
                }
            }
            else
            {
                for (int v = 0; v < n; v++)
                {
                    V[v] = (n - 1) - v;
                }
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2;)
            {
                if ((count--) <= 0)
                {
                    return indices.ToArray();
                }

                int u = v;
                if (nv <= u)
                {
                    u = 0;
                }
                v = u + 1;
                if (nv <= v)
                {
                    v = 0;
                }
                int w = v + 1;
                if (nv <= w)
                {
                    w = 0;
                }

                if (Snip(verts, u, v, w, nv, V))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                    {
                        V[s] = V[t];
                    }
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private static float GetArea(Vector2[] verts)
        {
            int n = verts.Length;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector2 pval = verts[p];
                Vector2 qval = verts[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        private static bool Snip(Vector2[] verts, int u, int v, int w, int n, int[] V)
        {
            int p;
            Triangle2 tri = new Triangle2(verts[V[u]], verts[V[v]], verts[V[w]]);
            if (Mathf.Epsilon > (((tri.b.x - tri.a.x) * (tri.c.y - tri.a.y)) - ((tri.b.y - tri.a.y) * (tri.c.x - tri.a.x))))
            {
                return false;
            }
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                {
                    continue;
                }
                Vector2 P = verts[V[p]];
                if (tri.Contains(P))
                {
                    return false;
                }
            }
            return true;
        }

        public void DebugDraw(Color col)
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                Debug.DrawLine(
                    new Vector3(Edges[i].Start.x, 0, Edges[i].Start.y),
                    new Vector3(Edges[i].End.x, 0, Edges[i].End.y),
                    col);
            }
        }
    }
}
