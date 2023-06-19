using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.HullDelaunayVeronoi.Primitives
{
    public interface IVertex
    {
        int Dimensions { get; }
        int Id { get; set; }
        int Tag { get; set; }
        float[] Position { get; set; }
        float Magnitude { get; }
        float SqrMagnitude { get; }
        float Distance(IVertex v);
        float SqrDistance(IVertex v);
    }
}
