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
                    return false;
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


        private T[] buffer = null;
        private int head = 0;
        private int tail = 0;
        private int count = 0;

        public int Count
        {
            get { return count; }
        }

        public T this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public int Head { get { return head; } }
        public int Tail { get { return tail; } }

        public bool IsSynchronized { get { return true; } }

        public object SyncRoot { get { return this; } }

        public bool IsReadOnly => throw new NotImplementedException();

        public CircularList()
        {
            buffer = new T[0];
        }

        public CircularList(int bufferSize)
        {
            buffer = new T[bufferSize];
        }

        public CircularList(CircularList<T> other)
        {
            buffer = new T[other.Count];
            Append(other);
        }

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

        public static CircularList<T> operator +(CircularList<T> self, CircularList<T> other)
        {
            return new CircularList<T>(self).Append(other);
        }

        public void ResizeBuffer(int newSize, bool keepData = true)
        {
            if (newSize == buffer.Length)
                return;

            T[] newBuffer = new T[newSize];

            if (keepData == false)
            {
                head = tail = count = 0;
            }
            else if (newSize > count)
            {
                count = 0;
                foreach (T item in this)
                    newBuffer[count++] = item;
                head = 0;
                tail = count % newBuffer.Length;
            }
            else
            {
                count = 0;
                foreach (T item in this)
                {
                    newBuffer[count++] = item;
                    if (count == newSize)
                        break;
                }
                head = 0;
                tail = count % buffer.Length;
            }

            buffer = newBuffer;
        }

        public T Get(int index)
        {
            if (index >= count || index < 0)
                throw new IndexOutOfRangeException();

            if (head + index < tail)
            {
                return buffer[(head + index) % buffer.Length];
            }
            else
            {
                return buffer[(head + index) % buffer.Length];
            }
        }

        public void Set(int index, T item)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("Index: " + index);
            }

            buffer[(head + index) % buffer.Length] = item;
        }

        public void Append(T item)
        {
            buffer[tail] = item;
            tail = (tail + 1) % buffer.Length;

            if (count < buffer.Length)
                count++;
            else
                head++;
        }

        public CircularList<T> Append(CircularList<T> items)
        {
            if (items.count >= buffer.Length)
            {
                count = 0;
                for (int i = items.count - buffer.Length; i < items.count; i++)
                    buffer[count++] = items.buffer[i];
                head = 0;
                tail = count % buffer.Length;
            }
            else
            {
                foreach (T item in items)
                {
                    buffer[tail] = item;
                    tail = (tail + 1) % buffer.Length;
                    if (count < buffer.Length)
                        count++;
                    else
                        head++;
                }
            }
            return this;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("index");

            if (head + index < tail)
            {
                for (int i = tail; i > head + index; i--)
                    buffer[i] = buffer[i - 1];
                buffer[head + index] = item;
                tail = (tail + 1) % buffer.Length;
                count++;
            }
            else
            {
                if (head + index >= buffer.Length)
                {
                    for (int i = tail; i > (head + index - buffer.Length); i--)
                        buffer[i] = buffer[i - 1];
                    buffer[head + index - buffer.Length] = item;
                    if (head == tail)
                        head = (head + 1) % buffer.Length;
                    else
                        count++;
                    tail++;
                }
                else
                {
                    for (int i = tail; i > 0; i--)
                        buffer[i] = buffer[i - 1];
                    buffer[0] = buffer[buffer.Length - 1];
                    for (int i = buffer.Length - 1; i > (head + index); i--)
                        buffer[i] = buffer[i - 1];
                    buffer[head + index] = item;
                    if (head == tail)
                        head = (head + 1) % buffer.Length;
                    else
                        count++;
                    tail++;
                }
            }
        }

        public void Insert(int index, CircularList<T> other)
        {
            if (index < 0 || index + other.count >= count)
                throw new IndexOutOfRangeException("index");

            if (other.count >= buffer.Length)
                Append(other);
            else
            {
                int diff = other.count;

                for (int i = tail - 1; tail > head + index; i--)
                {
                    buffer[(i + diff) % buffer.Length] = buffer[i];
                }
                tail = (tail + diff) % buffer.Length;
                // If the overwritten section was not being utilized, do not need to
                // update the head value
                if (other.count > buffer.Length - count)
                {
                    // Need to increment head by the number of overwritten items
                    head += (other.count - (buffer.Length - count));
                }

                for (int i = 0; i < diff; i++)
                {
                    buffer[(head + index + i) % buffer.Length] = other[i];
                }
            }
        }

        public T TakeFirst()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("Trying to take from empty buffer");

            count--;
            if (head + 1 < buffer.Length)
            {
                head++;
                return buffer[head - 1];
            }
            else
            {
                head = 0;
                return buffer[buffer.Length - 1];
            }
        }

        public T TakeAt(int index)
        {
            if (count <= 0 || index >= count)
                throw new IndexOutOfRangeException();

            T item = buffer[(head + index) % buffer.Length];

            for (int i = head + index; i > head; i--)
            {
                buffer[i % buffer.Length] = buffer[(i - 1) % buffer.Length];
            }
            head = (head + 1) % buffer.Length;
            count--;
            return item;
        }

        public T TakeLast()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("Trying to take from empty buffer");

            count--;
            if (tail == 0)
            {
                tail = buffer.Length - 1;
                return buffer[0];
            }
            else
            {
                tail--;
                if (tail < 0)
                    tail = buffer.Length - 1;
                return buffer[tail + 1];
            }
        }

        public bool Remove(T item)
        {
            if (count > 0)
            {
                if (head < tail)
                {
                    for (int i = head; i < tail; i++)
                    {
                        if (buffer[i].Equals(item))
                        {
                            for (int j = tail; j >= i; j--)
                            {
                                buffer[j - 1] = buffer[j];
                            }
                            count--;
                            tail--;
                            if (tail < 0)
                                tail = buffer.Length - 1;
                            return true;
                        }
                    }
                }
                else
                {
                    for (int i = head; i < buffer.Length; i++)
                    {
                        if (buffer[i].Equals(item))
                        {
                            for (int j = buffer.Length - 1; j > head; j--)
                                buffer[j] = buffer[j - 1];
                            count--;
                            head++;
                            return true;
                        }
                    }
                    for (int i = 0; i < tail; i++)
                    {
                        if (buffer[i].Equals(item))
                        {
                            for (int j = 0; j < tail; j++)
                                buffer[j] = buffer[j + 1];
                            count--;
                            tail--;
                            if (tail < 0)
                                tail = buffer.Length - 1;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void RemoveFirst()
        {
            if (count > 0)
            {
                head = (head + 1) % buffer.Length;
                count--;
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void RemoveLast()
        {
            if (count > 0)
            {
                if (tail == 0)
                    tail = buffer.Length - 1;
                else
                    tail--;
                count--;
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void RemoveAt(int index)
        {
            if (index > count)
                throw new IndexOutOfRangeException("Index out of bounds");

            // Data does not wrap around, push all elements back 1 index
            if (head < tail)
            {
                for (int i = head + index; i < tail; i++)
                    buffer[i] = buffer[i + 1];
                tail--;
                if (tail < 0)
                    tail = buffer.Length - 1;
            }
            // Data wraps, determine if the index is in the first or second section
            else
            {
                // In first section, move elements up 1 index
                if (index + head < buffer.Length)
                {
                    for (int i = head + index; i > head; i--)
                        buffer[i] = buffer[i - 1];
                    head++;
                }
                // In second section, move elements down 1 index
                else
                {
                    for (int i = index - (buffer.Length - head); i < tail; i++)
                        buffer[i] = buffer[i + 1];
                    tail--;
                    if (tail < 0)
                        tail = buffer.Length - 1;
                }
            }
            count--;
        }

        public void RemoveRange(int start, int length)
        {
            if (start + length >= count || start < 0)
                throw new IndexOutOfRangeException("Index out of range");

            count -= length;
            start = (start + head) % buffer.Length;
            for (int i = 0; i < length; i++)
            {
                buffer[(start) % buffer.Length] = buffer[(start + length) % buffer.Length];
                start++;
            }
            if (tail - length < 0)
                tail = buffer.Length - (length - tail);
            else
                tail -= length;
        }

        public void RemoveAll(T item)
        {
            if (head >= tail)
            {
                for (int i = buffer.Length - 1; i >= head; i--)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j > head; j--)
                        {
                            buffer[j] = buffer[j - 1];
                        }
                        head++;
                        count--;
                        i++;
                    }
                }
                for (int i = 0; i < tail; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j < tail; j++)
                            buffer[j] = buffer[j + 1];
                        tail--;
                        if (tail < 0)
                            tail = buffer.Length - 1;
                        count--;
                        i--;
                    }
                }
            }
            else
            {
                for (int i = head; i < tail; i++)
                {
                    if (buffer[i].Equals(item))
                    {
                        for (int j = i; j < tail; j++)
                            buffer[j] = buffer[j + 1];
                        tail--;
                        count--;
                        i--;
                    }
                }
            }
        }

        public bool Contains(T item)
        {
            foreach (T it in this)
                if (it.Equals(item))
                    return true;
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
                ret += it.ToString() + ", ";
            ret = ret.Remove(ret.Length - 2, 2);
            return ret + " } Count: " + count;
        }

        public void Add(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Append(item);
            }   
        }

        public void Insert(int index, IEnumerable<T> other, int length)
        {
            foreach (T item in other)
            {
                Insert(index, item);
                index++;
            }
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

        public T[] ToArray()
        {
            T[] array = new T[count];
            CopyTo(array, 0);
            return array;
        }

        public void Add(T item)
        {
            Append(item);
        }

        public void Clear()
        {
            Array.Clear(buffer, 0, count);
            count = 0;
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
