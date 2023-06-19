using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.Random
{
    // Designed to work with classes that have the following range before scaling: [0,1]
    public interface IPermutationNoise
    {
        float Scale { get; set; }

        // Get the next item in the permutation
        float Next();
    }
}
