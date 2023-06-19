using System;
using System.Collections.Generic;
using ProjectWorlds.HullDelaunayVeronoi.Primitives;
using ProjectWorlds.HullDelaunayVeronoi.Hull;

namespace ProjectWorlds.HullDelaunayVeronoi.Delaunay
{
    public class DelaunayTriangulation2 : DelaunayTriangulation2<Vertex2>
    {

    }

    public class DelaunayTriangulation2<VERTEX> : DelaunayTriangulation<VERTEX> where VERTEX : class, IVertex, new()
    {
        private float[,] matrixBuffer;

        public DelaunayTriangulation2() : base(2)
        {
            matrixBuffer = new float[3, 3];
        }

        public override void Generate(IList<VERTEX> input, bool assignIds = true, bool checkInput = false)
        {
            Clear();

            if (input.Count <= Dimensions + 1)
            {
                return;
            }

            int count = input.Count;
            for (int i = 0; i < count; i++)
            {
                float lenSq = input[i].SqrMagnitude;

                float[] v = input[i].Position;
                Array.Resize(ref v, Dimensions + 1);
                input[i].Position = v;

                input[i].Position[Dimensions] = (float)lenSq;
            }

            var hull = new ConvexHull<VERTEX>(Dimensions + 1);
            hull.Generate(input, assignIds, checkInput);

            for (int i = 0; i < count; i++)
            {
                float[] v = input[i].Position;
                Array.Resize(ref v, Dimensions);
                input[i].Position = v;
            }

            Vertices = new List<VERTEX>(hull.Vertices);
            Centroid.Position[0] = hull.Centroid[0];
            Centroid.Position[1] = hull.Centroid[1];

            count = hull.Simplexs.Count;

            // ToDo, make foreach loop
            for (int i = 0; i < count; i++)
            {
                Simplex<VERTEX> simplex = hull.Simplexs[i];

                if (simplex.Normal[Dimensions] >= 0.0f)
                {
                    for (int j = 0; j < simplex.Adjacent.Length; j++)
                    {
                        if (simplex.Adjacent[j] != null)
                        {
                            simplex.Adjacent[j].Remove(simplex);
                        }
                    }
                }
                else
                {
                    DelaunayCell<VERTEX> cell = CreateCell(simplex);
                    //cell.CircumCenter.Id = i;
                    Cells.Add(cell);
                }
            }
        }

        private float Determinant()
        {
            float fCofactor00 = matrixBuffer[1, 1] * matrixBuffer[2, 2] - matrixBuffer[1, 2] * matrixBuffer[2, 1];
            float fCofactor10 = matrixBuffer[1, 2] * matrixBuffer[2, 0] - matrixBuffer[1, 0] * matrixBuffer[2, 2];
            float fCofactor20 = matrixBuffer[1, 0] * matrixBuffer[2, 1] - matrixBuffer[1, 1] * matrixBuffer[2, 0];

            float fDet = matrixBuffer[0, 0] * fCofactor00 + matrixBuffer[0, 1] * fCofactor10 + matrixBuffer[0, 2] * fCofactor20;

            return fDet;
        }

        private DelaunayCell<VERTEX> CreateCell(Simplex<VERTEX> simplex)
        {
            // From MathWorld: http://mathworld.wolfram.com/Circumcircle.html

            VERTEX[] verts = simplex.Vertices;

            // x, y, 1
            for (int i = 0; i < 3; i++)
            {
                matrixBuffer[i, 0] = verts[i].Position[0];
                matrixBuffer[i, 1] = verts[i].Position[1];
                matrixBuffer[i, 2] = 1;
            }

            float a = Determinant();

            // size, y, 1
            for (int i = 0; i < 3; i++)
            {
                matrixBuffer[i, 0] = verts[i].SqrMagnitude;
            }

            float dx = -Determinant();

            // size, x, 1
            for (int i = 0; i < 3; i++)
            {
                matrixBuffer[i, 1] = verts[i].Position[0];
            }

            float dy = Determinant();

            // size, x, y
            for (int i = 0; i < 3; i++)
            {
                matrixBuffer[i, 2] = verts[i].Position[1];
            }
            float c = -Determinant();

            float s = -1.0f / (2.0f * a);

            float[] circumCenter = new float[2];
            circumCenter[0] = s * dx;
            circumCenter[1] = s * dy;

            float radius = Math.Abs(s) * (float)Math.Sqrt(dx * dx + dy * dy - 4.0 * a * c);

            return new DelaunayCell<VERTEX>(simplex, circumCenter, radius);
        }
    }
}
