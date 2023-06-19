using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Heaps
{
    public class MinHeap<T> where T : IComparable
    {
        public int Count { get { return count; } }

        public bool IsEmpty { get { return count == 0; } }

        private T[] buffer;
        private int count;

        public MinHeap(int capacity = 4)
        {
            buffer = new T[capacity];
            count = 0;
        }

        public void Clear()
        {
            buffer = new T[buffer.Length];
            count = 0;
        }

        public bool Insert(T value)
        {
            if (count >= buffer.Length)
            {
                int newCapacity;
                if (buffer.Length < 100)
                {
                    newCapacity = buffer.Length * 2;
                }
                else
                {
                    newCapacity = buffer.Length + 100;
                }

                T[] newBuffer = new T[newCapacity];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            // Insert the new item at the end of the buffer
            buffer[count] = value;
            // Get the index of the item and increment count
            int cur = count;

            int parent = ParentIndex(cur);

            // While the new node is larger than the parent, swap the current node with the parent
            while (cur > 0 && buffer[cur].CompareTo(buffer[parent]) < 0)
            {
                // Do the swap
                Swap(parent, cur);
                // Update the current index
                cur = parent;
                parent = ParentIndex(cur);
            }

            count++;
            return true;
        }

        private int ParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private void Swap(int indA, int indB)
        {
            T temp = buffer[indA];
            buffer[indA] = buffer[indB];
            buffer[indB] = temp;
        }

        public T TakeMin()
        {
            if (count == 0)
            {
                return default(T);
            }

            T temp = buffer[0];

            buffer[0] = buffer[count - 1];
            count--;

            FixHeap(0);

            return temp;
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].CompareTo(item) == 0)
                {
                    count--;
                    for (int j = i; j < count; j++)
                    {
                        buffer[j] = buffer[j + 1];
                    }
                    FixHeap(0);

                    return true;
                }
            }
            return false;
        }

        private void FixHeap(int index)
        {
            int left = (index * 2) + 1;
            int right = (index * 2) + 2;

            //if (left < count && (buffer[right] == null || buffer[index].CompareTo(buffer[right]) <= 0))
            if (left < count && (buffer[index].CompareTo(buffer[left]) > 0))
            {
                T tempValue = buffer[left];
                buffer[left] = buffer[index];
                buffer[index] = tempValue;
            }
            //else if (right < count && (buffer[left] == null || buffer[left].CompareTo(buffer[right]) > 0))
            if (right < count && (buffer[index].CompareTo(buffer[right]) > 0))
            {
                T tempValue = buffer[right];
                buffer[right] = buffer[index];
                buffer[index] = tempValue;
            }
            if (left < count)
            {
                FixHeap(left);
            }
            if (right < count)
            {
                FixHeap(right);
            }
        }

        public bool Contains(T item)
        {
            if (count == 0)
            {
                return false;
            }
            else
            {
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(0);

                int cur, comp;
                while (queue.Count > 0)
                {
                    cur = queue.Dequeue();

                    comp = buffer[cur].CompareTo(item);
                    if (comp == 0)
                        return true;
                    else if (comp < 0)
                    {
                        queue.Enqueue((cur * 2) + 1);
                        queue.Enqueue((cur * 2) + 2);
                    }
                }
            }
            return false;
        }
    }
}