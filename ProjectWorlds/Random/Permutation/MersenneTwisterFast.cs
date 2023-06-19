namespace ProjectWorlds.Random
{
    public class MersenneTwisterFast : IPermutationNoise
    {
        public uint Seed
        {
            get { return seed; }
        }

        public float Scale
        {
            get;
            set;
        } = 1.0f;

        private int N = 624;
        private int M = 397;
        private const uint MatrixA = 0x9908b0df;
        private const uint UpperMask = 0x80000000;
        private const uint LowerMask = 0x7fffffff;

        private uint[] _mt;
        private uint _mti;
        private uint index;

        private uint seed;

        public MersenneTwisterFast(uint seed, int N = 624, int M = 397)
        {
            this.seed = seed;
            this.N = N;
            this.M = M;
            this.index = (uint)(N + 1);

            _mti = (uint)(N + 1);

            _mt = new uint[N];
            _mt[0] = seed;
            for (_mti = 1; _mti < N; _mti++)
            {
                ulong v = 1812433253UL * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30));
                _mt[_mti] = (uint)(v + _mti);
            }
        }

        public float Next()
        {
            if (_mti >= N)
            {
                Twist();
            }

            uint y = _mt[_mti++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);

            return y / (1 << 31);
        }

        private void Twist()
        {
            for (int i = 0; i < N; i++)
            {
                uint y = (_mt[i] & UpperMask) | (_mt[(i + 1) % N] & LowerMask);
                _mt[i] = _mt[(i + M) % N] ^ (y >> 1) ^ ((y & 1) == 0 ? 0 : MatrixA);
            }

            _mti = 0;
        }
    }
}