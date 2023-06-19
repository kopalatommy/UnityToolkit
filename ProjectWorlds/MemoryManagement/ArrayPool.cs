using ProjectWorlds.DataStructures.Stacks;
using System;

namespace ProjectWorlds.MemoryManagement
{
    public class ArrayPool<T>
    {
        private static ArrayPool<T> instance = null;
        private static ArrayPool<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ArrayPool<T>();
                }
                return instance;
            }
        }

        public ArrayPool()
        {
            pools = new Stack<T[]>[16];
            for (int i = 0; i < 16; i++)
            {
                pools[i] = new Stack<T[]>();
            }
        }

        private Stack<T[]>[] pools;

        public static T[] Pop(int capactity)
        {
            return Instance.PopInternal(capactity);
        }

        private T[] PopInternal(int capactity)
        {
            int len = 1;
            while ((1 << len) < capactity)
            {
                len++;
            }
            if (len < pools.Length)
            {
                if (pools[len].Count == 0)
                {
                    return new T[1 << len];
                }
                else
                {
                    return pools[len].Pop();
                }
            }
            // Not tracked by the pool
            else
            {
                return new T[capactity];
            }
        }

        public static void Return(T[] arr)
        {
            Instance.ReturnInternal(arr);
        }

        private void ReturnInternal(T[] arr)
        {
            int len = 1;
            while ((1 << len) < arr.Length)
            {
                len++;
            }
            if ((1 << len) == arr.Length)
            {
                pools[len].Push(arr);
            }
        }
    }
}
