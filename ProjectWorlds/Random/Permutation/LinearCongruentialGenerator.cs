using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.Random
{
    public class LinearCongruentialGenerator : IPermutationNoise
    {
        public float Scale
        {
            get;
            set;
        } = 1.0f;

        public ulong Seed
        {
            get
            {
                return initialSeed;
            }
        }

        public static LinearCongruentialGenerator Default
        {
            get
            {
                return new LinearCongruentialGenerator((ulong)System.DateTime.Now.Ticks, 1103515245, 12345, uint.MaxValue);
            }
        }

        private ulong initialSeed;

        // The seed for the next value
        private ulong seed;

        // Parameters for the algorithm
        private ulong multiplier;
        private ulong increment;
        private ulong max;

        public LinearCongruentialGenerator(ulong initialSeed, ulong multiplier, ulong increment, ulong m)
        {
            this.initialSeed = this.seed = initialSeed;
            this.multiplier = multiplier;
            this.increment = increment;
            this.max = m;
        }

        public float Next()
        {
            seed = (multiplier * seed + increment) % max;
            return seed / (max - 1);
        }
    }
}
