﻿using System;
using System.Collections.Generic;
using ProjectWorlds.HullDelaunayVeronoi.Primitives;

namespace ProjectWorlds.HullDelaunayVeronoi.Delaunay
{
    public interface IDelaunayTriangulation<VERTEX> where VERTEX : class, IVertex, new()
    {
        int Dimensions { get; }

        IList<VERTEX> Vertices { get; }

        IList<DelaunayCell<VERTEX>> Cells { get; }

        VERTEX Centroid { get; }

        void Clear();

        void Generate(IList<VERTEX> input, bool assignIds = true, bool checkInput = false);
    }
}
