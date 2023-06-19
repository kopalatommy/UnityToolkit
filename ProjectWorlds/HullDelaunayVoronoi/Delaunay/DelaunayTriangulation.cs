using System;
using System.Collections.Generic;
using ProjectWorlds.HullDelaunayVeronoi.Primitives;

namespace ProjectWorlds.HullDelaunayVeronoi.Delaunay
{
    public abstract class DelaunayTriangulation<VERTEX> : IDelaunayTriangulation<VERTEX> where VERTEX : class, IVertex, new()
    {
		public int Dimensions { get; private set; }

		public IList<VERTEX> Vertices { get; protected set; }

		public IList<DelaunayCell<VERTEX>> Cells { get; protected set; }

		public VERTEX Centroid { get; private set; }

		public DelaunayTriangulation(int dimensions)
		{
			Dimensions = dimensions;

			Vertices = new List<VERTEX>();
			Cells = new List<DelaunayCell<VERTEX>>();
			Centroid = new VERTEX();
		}

		public virtual void Clear()
		{
			Cells.Clear();
			Vertices.Clear();
			Centroid = new VERTEX();
		}

		public abstract void Generate(IList<VERTEX> input, bool assignIds = true, bool checkInput = false);
	}
}
