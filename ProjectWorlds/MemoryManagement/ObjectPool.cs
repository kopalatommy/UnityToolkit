using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.MemoryManagement
{
    public class ObjectPool<T> : Stack<T>, IObjectPool<T>
    {
        public ObjectPool()
        {

        }

        public ObjectPool(int initialCount)
        {
            Populate(initialCount);
        }

        public void Populate(int count)
        {
            for (; count > 0; count--)
            {
                Push(default(T));
            }
        }

        int IObjectPool<T>.Count()
        {
            return Count;
        }
    }
}
