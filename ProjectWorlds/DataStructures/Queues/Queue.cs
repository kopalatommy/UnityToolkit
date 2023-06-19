namespace ProjectWorlds.DataStructures.Queues
{
    public class Queue<T> : ProjectWorlds.DataStructures.Lists.DoubleLinkedList<T>
    {
        public T Dequeue()
        {
            return TakeFirst();
        }

        public T Peek()
        {
            return Get(0);
        }

        public void Enqueue(T item)
        {
            AppendBack(item);
        }
    }
}