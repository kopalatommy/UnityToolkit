namespace ProjectWorlds.DataStructures.Trees
{
    public interface ITree<T>
    {
        public void Insert(T item);

        public bool Remove(T item);

        public bool Contains(T item);

        public T Min();

        public T Max();

        public void Clear();

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection);

        // Left height - right height
        //public int Skew();
    }
}
