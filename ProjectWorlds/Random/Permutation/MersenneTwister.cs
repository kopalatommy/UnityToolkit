using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.Random
{
    public class MersenneTwister : IPermutationNoise
    {
        public uint Seed
        {
            get
            {
                return seed;
            }
        }

        public float Scale
        {
            get;
            set;
        } = 1.0f;

        private int N;
        private int M;
        private uint MatrixA = 0x9908b0df;
        private const uint UMASK = 0x80000000U;
        private const uint LMASK = 0x7fffffffU;

        private uint[] mt;
        private int left = 1;
        private int initf = 0;
        private uint next;

        private uint seed;

        public MersenneTwister(uint seed, int N = 624, int M = 397)
        {
            this.seed = seed;
            this.N = N;
            this.M = M;

            mt = new uint[N];
            Init(seed);
        }

        private void Init(uint seed)
        {
            mt[0] ^= seed;
            for (int i = 1; i < N; i++)
            {
                mt[i] = ((uint)(1812433253U * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i));
            }
            left = 1;
        }

        public float Next()
        {
            if (--left == 0)
            {
                NextState();
            }

            uint y = mt[next++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            // divided by 2^32 - 1
            return y * (1.0f / 4294967295.0f);
        }

        private void NextState()
        {
            left = N;
            next = 0;

            uint p = 0;
            for (int i = N - M + 1; --i > 0; p++)
            {
                mt[p] = mt[p + M] ^ Twist(mt[p], mt[p + 1]);
            }

            for (int i = M; --i > 0; p++)
            {
                mt[p] = mt[p + (M - N)] ^ Twist(mt[p], mt[p + 1]);
            }
            mt[p] = mt[p + (M - N)] ^ Twist(mt[p], mt[0]);
        }

        private readonly uint[] mag01 = new[] { 0x0U, 0x9908b0dfU };
        private uint Twist(uint u, uint v)
        {
            return ((MixBits(u, v) >> 1) ^ mag01[v & 1U]);
        }

        private uint MixBits(uint u, uint v)
        {
            return (((u) & UMASK) | ((v) & LMASK));
        }
    }
}
