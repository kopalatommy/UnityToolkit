using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    public class CircularList<T> : ICollection, IEnumerable, IListExtended<T>
    {
        internal class Enumerator : IEnumerator, IEnumerator<T>
        {
            private CircularList<T> list = null;
            private int index = -1;
            private bool isDisposed = false;

            public Enumerator(CircularList<T> list)
            {
                this.list = list;
            }

            public bool MoveNext()
            {
                if (index + 1 < list.Count)
                {
                    index++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool isDisposing)
            {
                if (!isDisposed)
                {
                    if (isDisposing)
                    {
                        // Clear all property values that maybe have been set
                        // when the class was instantiated
                        list = null;
                        isDisposed = true;
                    }

                    // Indicate that the instance has been disposed.
                    isDisposed = true;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return list[index];
                }
            }

            public T Current
            {
                get
                {
                    try
                    {
                        return list[index];
                    }
                    catch
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }
                }
            }
        }


        public int Capacity
        {
            get
            {
                return buffer.Length;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsFull
        {
            get
            {
                return count == Capacity;
            }
        }

        public T this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        private T[] buffer = null;
        private int start = 0;
        private int end = 0;
        private int count = 0;

        public CircularList()
        {
            buffer = new T[0];
        }

        public CircularList(int capacity)
        {
            buffer = new T[capacity];
        }

        public CircularList(int capacity, T[] items)
        {
            buffer = new T[capacity];
            Array.Copy(items, buffer, items.Length);
            count = items.Length;
            end = items.Length == capacity ? 0 : count;
        }

        public T Front()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[start];
        }

        public T Back()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[(end != 0 ? end : Capacity) - 1];
        }

        public void Add(T item)
        {
            buffer[end] = item;
            end = (end + 1) % Capacity;
            if (IsFull)
            {
                start = end;
            }
            else
            {
                count++;
            }
        }

        public void Add(System.Collections.Generic.IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Add(item);
            }
        }

        public void AddFront(T item)
        {
            if (start == 0)
            {
                start = Capacity;
            }
            start--;
            buffer[start] = item;

            if (IsFull)
            {
                end = start;
            }
            else
            {
                count++;
            }
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException();
            }

            if (start < end)
            {
                for (int i = end; i > start + index; i--)
                {
                    buffer[i] = buffer[i - 1];
                }
                end = (end + 1) % Capacity;
                if (IsFull)
                {
                    start = end;
                }
                buffer[start + index] = item;
            }
            else
            {
                // Get the true index of the item
                int itemIndex = start + index % Capacity;
                if (itemIndex < end)
                {
                    for (int i = itemIndex; i < end; i++)
                    {
                        buffer[i] = buffer[i - 1];
                    }
                    buffer[itemIndex] = item;
                    end = (end + 1) % Capacity;
                    if (IsFull)
                    {
                        start = end;
                    }
                }
                else
                {
                    for (int i = end; i > 0; i--)
                    {
                        buffer[i] = buffer[i - 1];
                    }
                    buffer[0] = buffer[Capacity - 1];
                    for (int i = Capacity - 1; i > Capacity; i--)
                    {
                        buffer[i] = buffer[i - 1];
                    }
                    end = (end + 1) % Capacity;
                    if (IsFull)
                    {
                        start = end;
                    }
                }
                buffer[itemIndex] = item;
            }

            if (!IsFull)
            {
                count++;
            }
        }

        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Insert(index++, item);
            }
        }

        // ToDo, could be more efficient
        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other, int length)
        {
            foreach (T item in other)
            {
                Insert(index++, item);

                length--;
                if (length == 0)
                {
                    return;
                }
            }
        }

        public T TakeFirst()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            T temp = buffer[start];
            buffer[start] = default(T);
            start = start + 1 % Capacity;
            count--;

            return temp;
        }

        public T TakeAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            int itemIndex = start + index % Capacity;
            T temp = buffer[itemIndex];
            if (itemIndex < end)
            {
                for (int i = itemIndex; i < end; i++)
                {
                    buffer[i] = buffer[i + 1];
                }
                end--;
                buffer[end] = default(T);
            }
            else
            {
                for (int i = itemIndex; i > start; i--)
                {
                    buffer[i] = buffer[i - 1];
                }
                buffer[start] = default(T);
                start = (start + 1) % Capacity;
            }
            count--;
            return temp;
        }

        public T TakeLast()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (end == 0)
            {
                end = Capacity;
            }
            end--;

            T temp = buffer[end];
            buffer[end] = default(T);
            count--;

            return temp;
        }

        public void RemoveFirst()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            buffer[start] = default(T);
            start = start + 1 % Capacity;
            count--;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            int itemIndex = start + index % Capacity;
            if (itemIndex < end)
            {
                for (int i = itemIndex; i < end; i++)
                {
                    buffer[i] = buffer[i + 1];
                }
                end--;
                buffer[end] = default(T);
            }
            else
            {
                for (int i = itemIndex; i > start; i--)
                {
                    buffer[i] = buffer[i - 1];
                }
                buffer[start] = default(T);
                start = (start + 1) % Capacity;
            }
            count--;
        }

        public void RemoveLast()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (end == 0)
            {
                end = Capacity;
            }
            end--;

            buffer[end] = default(T);
            count--;
        }

        public void RemoveRange(int start, int length)
        {
            if (length < 0 || length > count)
            {
                throw new IndexOutOfRangeException();
            }

            int startIndex = this.start + start;
            int endIndex = startIndex + length;

            for (int i = 0; i < length; i++)
            {
                buffer[startIndex + i % Capacity] = buffer[endIndex + i % Capacity];
            }

            count -= length;

            end -= length;
            if (end < 0)
            {
                end = Math.Abs(end) - 1;
            }
        }

        public void Clear()
        {
            start = end = count = 0;
            Array.Clear(buffer, 0, buffer.Length);
        }

        public T[] ToArray()
        {
            T[] temp = new T[count];
            
            if (start < end)
            {
                for (int i = start; i < end; i++)
                {
                    temp[i - start] = buffer[i];
                }
            }
            else
            {
                for (int i = start; i < Capacity; i++)
                {
                    temp[i - start] = buffer[i];
                }
                for (int i = 0; i < end; i++)
                {
                    temp[(Capacity - start) + i] = buffer[i];
                }
            }

            return temp;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[start + index % Capacity];
        }

        public void Set(int index, T item)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == count)
            {
                Add(item);
            }
            else
            {
                buffer[start + index % Capacity] = item;
            }
        }

        public void Resize(int newCapacity)
        {
            T[] newBuffer = new T[newCapacity];

            if (start < end)
            {
                for (int i = start; i < end && newCapacity > 0; i++, newCapacity--)
                {
                    newBuffer[i - start] = buffer[i];
                }
            }
            else
            {
                for (int i = start; i < Capacity && newCapacity > 0; i++, newCapacity--)
                {
                    newBuffer[i - start] = buffer[i];
                }
                for (int i = 0; i < end && newCapacity > 0; i++, newCapacity--)
                {
                    newBuffer[(Capacity - start) + i] = buffer[i];
                }
                start = 0;
                end = Count;
            }
        }

        public bool Remove(T item)
        {
            if (start < end)
            {
                for (int i = start; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j > start; j--)
                        {
                            buffer[j] = buffer[j - 1];
                        }
                        buffer[start] = default(T);
                        start++;

                        count--;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = start; i < Capacity; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j > start; j--)
                        {
                            buffer[j] = buffer[j - 1];
                        }
                        buffer[start] = default(T);
                        start = start + 1 % Capacity;

                        count--;
                        return true;
                    }
                }
                for (int i = 0; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = end - 1; j > i; j--)
                        {
                            buffer[j - 1] = buffer[j];
                        }
                        buffer[end] = default(T);
                        end--;
                        if (end < 0)
                        {
                            end = Capacity - 1;
                        }

                        count--;
                        return true;
                    }
                }
            }

            return false;
        }

        public void RemoveAll(T item)
        {
            if (start < end)
            {
                for (int i = start; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j > start; j--)
                        {
                            buffer[i] = buffer[i - 1];
                        }
                        buffer[start] = default(T);
                        start++;

                        count--;
                    }
                }
            }
            else
            {
                for (int i = start; i < Capacity; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j > start; j--)
                        {
                            buffer[i] = buffer[i - 1];
                        }
                        buffer[start] = default(T);
                        start = start + 1 % Capacity;

                        count--;
                    }
                }
                for (int i = 0; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = end - 1; j > i; j--)
                        {
                            buffer[i - 1] = buffer[i];
                        }
                        buffer[end] = default(T);
                        end--;
                        if (end < 0)
                        {
                            end = Capacity - 1;
                        }

                        count--;
                    }
                }
            }
        }




























        public bool IsSynchronized { get { return true; } }

        public object SyncRoot { get { return this; } }

        public bool IsReadOnly => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            else if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            else if (array.Length < count)
                throw new ArgumentException("length");

            foreach (T item in this)
                array.SetValue(item, index++);
        }

        public bool Contains(T item)
        {
            if (start < end)
            {
                for (int i = start; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = start; i < Capacity; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < end; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int IndexOf(T item)
        {
            int ind = 0;
            foreach (T it in this)
            {
                if (it.Equals(item))
                    return ind;
                else
                    ind++;
            }
            return -1;
        }

        public override string ToString()
        {
            string ret = "{ ";
            foreach (T it in this)
            {
                ret += it.ToString() + ", ";
            }
            ret = ret.Remove(ret.Length - 2, 2);
            return ret + " } Count: " + count;
        }

        public int FirstIndexOf(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public int LastIndexOf(T item)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (buffer[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
