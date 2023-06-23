namespace ProjectWorlds.DataStructures.Lists
{
    public interface IListExtended<T> : System.Collections.Generic.IList<T>
    {
        public void Set(int index, T value);

        public T Get(int index);

        public void Add(System.Collections.Generic.IEnumerable<T> other);

        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other);

        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other, int length);

        public T TakeFirst();

        public T TakeAt(int index);

        public T TakeLast();

        public void RemoveFirst();

        public void RemoveLast();

        public void RemoveRange(int start, int length);

        public void RemoveAll(T item);

        public int FirstIndexOf(T item);

        public int LastIndexOf(T item);

        public T[] ToArray();

        public T Front();

        public T Back();
    }
}
