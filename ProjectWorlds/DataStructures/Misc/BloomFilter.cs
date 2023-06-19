using System;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Misc
{
    public class BloomFilter<T>
    {
        private IList<Func<T, int>> hashFuncts = null;
        private int bits = 0;
        ulong bitField = 0;

        public IList<Func<T, int>> HashFuncts
        {
            get { return hashFuncts; }
        }
        public int Size
        {
            get { return bits; }
        }
        public ulong BitField
        {
            get { return bitField; }
        }

        public BloomFilter(int bits, IList<Func<T, int>> hashFuncts)
        {
            if (hashFuncts.Count == 0)
                throw new ArgumentException("hashFuncts.count must be greater than 0");

            this.bits = bits;
            this.hashFuncts = hashFuncts;
        }

        public void Add(T item)
        {
            foreach (Func<T, int> func in hashFuncts)
            {
                int ind = func(item) % bits;
                bitField |= (1UL << ind);
            }
        }

        public bool Contains(T item)
        {
            foreach (Func<T, int> func in hashFuncts)
            {
                int ind = func(item) % bits;
                if ((bitField & (1UL << ind)) == 0)
                    return false;
            }
            return true;
        }
    }
}
