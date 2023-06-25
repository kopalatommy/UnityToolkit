using System;
using System.Collections;

namespace ProjectWorlds.DataStructures.Lists
{
    public class ArrayList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, IListExtended<T>
    {
        public class Enumerator : System.Collections.Generic.IEnumerator<T>, IDisposable
        {
            private ArrayList<T> list = null;
            int index = -1;
            bool isDisposed = false;

            public Enumerator(ArrayList<T> list)
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
                Dispose(false);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!isDisposed)
                {
                    if (disposing)
                    {
                        // Clear all property values that maybe have been set
                        // when the class was instantiated
                        index = -1;
                        list = null;
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
                    return (T)list[index];
                }
            }
        }

        public int Count { get { return count; } }
        public T this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        private T[] buffer = null;
        private int count = 0;

        public bool IsReadOnly { get { return false; } }

        public bool IsFixedSize { get { return false; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return this; } }

        public ArrayList()
        {
            buffer = new T[0];
        }

        public ArrayList(int bufferSize)
        {
            buffer = new T[bufferSize];
        }

        public ArrayList(ArrayList<T> other)
        {
            buffer = new T[other.Count];
            Add(other);
        }

        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            else if (array.Length < count)
            {
                throw new ArgumentException("length");
            }

            foreach (T item in this)
            {
                array.SetValue(item, index++);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            else if (array.Length < count)
            {
                throw new ArgumentException("length");
            }

            foreach (T item in this)
            {
                array.SetValue(item, index++);
            }
        }

        public static ArrayList<T> operator +(ArrayList<T> self, ArrayList<T> other)
        {
            ArrayList<T> toRet = new ArrayList<T>(self);
            toRet.Add(other);
            return toRet;
        }

        /// <summary>
        /// Expands the size of the internal buffer
        /// </summary>
        /// <param name="len">New size of buffer</param>
        public void Reserve(int len)
        {
            if (len > buffer.Length)
            {
                T[] newBuffer = new T[len];
                Array.Copy(buffer, newBuffer, count);
                buffer = newBuffer;
            }
        }

        private void IncreaseBuffer()
        {
            if (buffer.Length == 0)
            {
                Reserve(1);
            }
            if (count < 512)
            {
                Reserve(count * 2);
            }
            else
            {
                Reserve(count + 512);
            }
        }

        public T Get(int index)
        {
            if (index >= count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[index];
        }

        public T Front()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[0];
        }

        public T Back()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            return buffer[count - 1];
        }

        /*public void Set(int index, object value)
        {
            Set(index, (T)value);
        }*/

        public void Set(int index, T value)
        {
            if (index >= count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            buffer[index] = value;
        }

        /*public int Add(object item)
        {
            if (count >= buffer.Length)
            {
                IncreaseBuffer();
            }
            buffer[count++] = (T)item;
            return count - 1;
        }*/

        public void Add(T item)
        {
            if (count >= buffer.Length)
            {
                IncreaseBuffer();
            }
            buffer[count++] = item;
        }

        /*public ArrayList<T> Add(ArrayList<T> other)
        {
            if(count + other.Count > buffer.Length)
                Reserve(count + other.Count);
            Array.Copy(other.buffer, 0, buffer, count, other.count);
            count += other.count;
            return this;
        }*/

        public void Add(System.Collections.Generic.IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Add(item);
            }
        }

        /*public void Insert(int index, object item)
        {
            Insert(index, (T)item);
        }*/

        public void Insert(int index, T item)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException("index");
            }
            else if (index == count)
            {
                Add(item);
            }
            else
            {
                if (count == buffer.Length)
                {
                    IncreaseBuffer();
                }

                for (int i = count; i > index; i--)
                {
                    buffer[i] = buffer[i - 1];
                }
                buffer[index] = item;
                count++;
            }
        }

        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException("index");
            }

            foreach (T item in other)
            {
                Add(item);
            }
        }

        public void Insert(int index, System.Collections.Generic.IEnumerable<T> other, int length)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException("index");
            }

            if ((count + index + length + 1) > buffer.Length)
            {
                Reserve(count + index + length + 1);
            }

            System.Collections.Generic.IEnumerator<T> enumer = other.GetEnumerator();

            if (index == count)
            {
                // Push all elements after index down the buffer
                for (int i = index; i < index + length; i++)
                {
                    if (enumer.MoveNext())
                    {
                        buffer[i] = enumer.Current;
                        count++;

                    }
                    // The provided length was greater than that of the provided collection. This created a void
                    // in the list and is undefined behavior
                    else
                    {
                        throw new InvalidOperationException("length > length of other");
                    }
                }
            }
            else
            {
                for (int i = count + length; i >= index + length; i--)
                {
                    buffer[i] = buffer[i - length];
                }

                for (int i = 0; i < length && enumer.MoveNext(); i++)
                {
                    buffer[index + i] = enumer.Current;
                    count++;
                }
            }
        }

        public T TakeFirst()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException("Empty list");
            }

            T ret = buffer[0];

            for (int i = 1; i < count; i++)
            {
                buffer[i - 1] = buffer[i];
            }

            count--;
            return ret;
        }

        public T TakeAt(int index)
        {
            if (count == 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            T ret = buffer[index];

            for (int i = index + 1; i < count; i++)
            {
                buffer[i - 1] = buffer[i];
            }
            count--;
            return ret;
        }

        public T TakeLast()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException("Empty list");
            }

            count--;
            return buffer[count];
        }

        public bool RemoveFirst(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        buffer[j] = buffer[j + 1];
                    }
                    count--;
                    return true;
                }
            }
            return false;
        }

        /*public void Remove(object item)
        {
            Remove((T)item);
        }*/

        public bool Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        buffer[j] = buffer[j + 1];
                    }
                    count--;
                    return true;
                }
            }
            return false;
        }

        public bool RemoveLast(T item)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (buffer[i].Equals(item))
                {
                    for (int j = i; j < count - 1; j++)
                        buffer[j] = buffer[j + 1];
                    count--;
                    return true;
                }
            }
            return false;
        }

        public void RemoveFirst()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("Empty list");

            for (int i = 1; i < count; i++)
                buffer[i - 1] = buffer[i];
            count--;
        }

        public void RemoveAt(int index)
        {
            if (count == 0 || index >= count)
                throw new IndexOutOfRangeException();

            for (int i = index + 1; i < count; i++)
                buffer[i - 1] = buffer[i];
            count--;
        }

        public void RemoveLast()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("Empty list");

            count--;
        }

        public void RemoveRange(int start, int length)
        {
            if (start + length >= count || start < 0)
                throw new IndexOutOfRangeException("Index out of range");

            Array.Copy(buffer, start + length, buffer, start, count - start - length);
            count -= length;
        }

        public void RemoveAll(T item)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (buffer[i].Equals(item))
                {
                    for (int j = i; j < count - 1; j++)
                        buffer[j] = buffer[j + 1];
                    count--;
                }
            }
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /*public bool Contains(object item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }*/

        public int IndexOf(T item)
        {
            return FirstIndexOf(item);
        }

        /*public int IndexOf(object item)
        {
            return FirstIndexOf((T)item);
        }*/

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

        public void Clear()
        {
            count = 0;
        }

        public T[] ToArray()
        {
            T[] array = new T[count];
            Array.Copy(buffer, array, count);
            return array;
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
    }
}
