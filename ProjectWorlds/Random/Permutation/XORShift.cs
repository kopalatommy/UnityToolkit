namespace ProjectWorlds.Random
{
    public class XORShift : IPermutationNoise
    {
        public float Scale
        {
            get;
            set;
        } = 1.0f;

        public uint Seed
        {
            get { return seed; }
        }

        private int x;
        private int y;
        private int z;
        private int w;

        private uint seed;

        public XORShift(uint seed)
        {
            x = (int)seed;
            y = 362436069;
            z = 521288629;
            w = 88675123;
        }

        public float Next()
        {
            uint t = (uint)(x ^ (x << 11));
            x = y;
            y = z;
            z = w;
            w = (int)(w ^ (w >> 19) ^ (t ^ (t >> 8)));
            return (float)(w / uint.MaxValue + 1.0);
        }
    }
}