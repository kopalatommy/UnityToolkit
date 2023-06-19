using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    public class LinkedList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, IListExtended<T>
    {
        internal class Node
        {
            public T item;
            public Node next = null;

            public Node(T item, Node next = null)
            {
                this.item = item;
                this.next = next;
            }
        }

        public class Enumerator : IEnumerator, IEnumerator<T>
        {
            private LinkedList<T> list = null;
            private Node currentNode = null;
            private bool starting = true;
            private bool isDisposed = false;

            public Enumerator(LinkedList<T> list)
            {
                this.list = list;
                currentNode = null;
                starting = true;
            }

            public bool MoveNext()
            {
                if (currentNode != null)
                {
                    if (starting)
                    {
                        starting = false;
                        currentNode = list.head;
                        return true;
                    }
                    else if (currentNode.next != null)
                    {
                        currentNode = currentNode.next;
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                currentNode = null;
                starting = true;
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
                        currentNode = null;
                        isDisposed = true;
                    }

                    // Indicate that the instance has been disposed.
                    isDisposed = true;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public T Current
            {
                get
                {
                    try
                    {
                        return currentNode.item;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public T this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public bool IsReadOnly { get { return false; } }

        public bool IsSynchronized { get { return true; } }

        public object SyncRoot { get { return this; } }

        private Node head = null;
        private int count = 0;

        // Default constructor
        public LinkedList()
        {

        }

        // Copy constructor
        public LinkedList(LinkedList<T> other)
        {
            Add(other);
        }

        // Return an enumerator for the list
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Copies the data in the list to an array object
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

        /// <summary>
        /// Returns a new list that contains the items of both lists
        /// </summary>
        /// <param name="self">First list</param>
        /// <param name="other">Second list</param>
        /// <returns>New list containing the elements from both lists</returns>
        public static LinkedList<T> operator +(LinkedList<T> self, LinkedList<T> other)
        {
            return new LinkedList<T>(self).Add(other);
        }

        /// <summary>
        /// Returns the item at the specified index
        /// </summary>
        /// <param name="index">Index of access</param>
        /// <returns>Item at index</returns>
        public T Get(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException();

            return GoTo(head, index).item;
        }

        /// <summary>
        /// Sets the item of the node at the specified index
        /// </summary>
        /// <param name="index">Index to change</param>
        /// <param name="item">New item</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void Set(int index, T item)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            GoTo(head, index).item = item;
        }

        /// <summary>
        /// Adds the item to the end of the list
        /// </summary>
        /// <param name="item">Item to append</param>
        public void Add(T item)
        {
            if (head == null)
            {
                head = new Node(item, null);
            }
            else
            {
                GoTo(head, Count - 1).next = new Node(item, null);
            }
            count++;
        }

        /// <summary>
        /// Appends the items from the other list
        /// </summary>
        /// <param name="other">The list to add</param>
        /// <returns>Reference to this list</returns>
        public LinkedList<T> Add(LinkedList<T> other)
        {
            if (head == null)
            {
                Node oth = other.head;
                head = new Node(oth.item, null);
                Node cur = head;
                oth = oth.next;
                count++;

                while (oth != null)
                {
                    cur.next = new Node(oth.item, null);
                    cur = cur.next;
                    oth = oth.next;
                    count++;
                }

                //AppendRecurse(null, other.head);
            }
            else
            {
                Node oth = other.head;
                Node cur = GoTo(head, count - 1);

                while (oth != null)
                {
                    cur.next = new Node(oth.item, null);
                    cur = cur.next;
                    oth = oth.next;
                    count++;
                }
            }
            return this;
        }

        public void Add(IEnumerable<T> other)
        {
            Node temp = head;
            foreach (T item in other)
            {
                if (temp == null)
                {
                    head = temp = new Node(item);
                }
                else
                {
                    temp.next = new Node(item);
                }
                count++;
            }
        }

        /// <summary>
        /// Adds an item to the list at the speified index
        /// </summary>
        /// <param name="index">Index to add the itme at</param>
        /// <param name="item">The item to add</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void Insert(int index, T item)
        {
            if (index == 0)
            {
                Node n = new Node(item, head);
                head = n;
            }
            else
            {
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException();

                Node cur = GoTo(head, index - 1);
                cur.next = new Node(item, cur.next);
            }
            count++;
        }

        public void Insert(int index, IEnumerable<T> other)
        {
            Node temp = head != null ? GoTo(head, index) : null;
            foreach (T item in other)
            {
                if (temp == null)
                {
                    temp = head = new Node(item);
                }
                else
                {
                    temp.next = new Node(item, temp.next);
                }
                count++;
            }
        }

        public void Insert(int index, IEnumerable<T> other, int length)
        {
            Node temp = head != null ? GoTo(head, index) : null;
            foreach (T item in other)
            {
                if (temp == null)
                {
                    temp = head = new Node(item);
                }
                else
                {
                    temp.next = new Node(item, temp.next);
                }
                count++;
                if (--length == 0)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the first item in the list and returns it
        /// </summary>
        /// <returns>The first item in the list</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T TakeFirst()
        {
            if (count == 0)
                throw new IndexOutOfRangeException();
            else
            {
                T item = head.item;
                head = head.next;
                count--;
                return item;
            }
        }

        /// <summary>
        /// Removes and returns the item at the specified index
        /// </summary>
        /// <param name="index">Index of the item to take</param>
        /// <returns>The item at the index</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T TakeAt(int index)
        {
            if (index > count)
                throw new IndexOutOfRangeException();
            else
            {
                if (index == 0)
                    return TakeFirst();
                else
                {
                    Node ind = GoTo(head, index - 1);
                    T item = ind.next.item;
                    count--;
                    ind.next = ind.next.next;
                    return item;
                }
            }
        }

        /// <summary>
        /// Removes the first instance of the item from the list
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if the item was removed</returns>
        public bool Remove(T item)
        {
            if (head == null)
                return false;
            else
            {
                Node cur = head;

                if (cur.next.item.Equals(item))
                {
                    count--;
                    head = cur.next;
                    cur = cur.next;
                    return true;
                }
                else
                {
                    while (cur.next != null)
                    {
                        if (cur.next.item.Equals(item))
                        {
                            cur.next = cur.next.next;
                            count--;
                            return true;
                        }
                        cur = cur.next;
                    }
                }
                return false;
                //return RemoveRecurse(item, head);
            }
        }

        /// <summary>
        /// Removes the item at the given index
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();
            else if (index == 0)
            {
                if (count == 1)
                    head = null;
                else
                    head = head.next;
            }
            else
            {
                Node cur = index == 1 ? head : GoTo(head, index - 1);

                if (cur.next.next == null)
                    cur.next = null;
                else
                    cur.next = cur.next.next;
            }
            count--;
        }

        /// <summary>
        /// Removes the items for the given range
        /// </summary>
        /// <param name="start">Index of first item to remove</param>
        /// <param name="length">Number of items in range</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void RemoveRange(int start, int length)
        {
            if (start + length < count)
            {
                if (start == 0)
                    head = GoTo(head, length);
                else
                    GoTo(head, start - 1).next = GoTo(head, start + length);
                count -= length;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Searches through list and removes all mathcing items
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void RemoveAll(T item)
        {
            while (head != null && head.item.Equals(item))
            {
                head = head.next;
                count--;
            }

            if (count > 0)
            {
                Node cur = head;
                while (cur.next != null)
                {
                    if (cur.next.item.Equals(item))
                    {
                        cur.next = cur.next.next;
                        count--;
                    }
                    cur = cur.next;
                }
            }
        }

        /// <summary>
        /// Checks if the list contains the item
        /// </summary>
        /// <param name="item">Item to check for</param>
        /// <returns>True if the item if found</returns>
        public bool Contains(T item)
        {
            Node cur = head;
            while (cur != null)
            {
                if (cur.item.Equals(item))
                    return true;
                else
                    cur = cur.next;
            }
            return false;

            //return ContainsRecurse(item, head);
        }

        /// <summary>
        /// Returns the index of the given item
        /// </summary>
        /// <param name="item">Item to get</param>
        /// <returns>Index of item in the list</returns>
        public int IndexOf(T item)
        {
            Node cur = head;
            int ind = 0;
            while (cur != null)
            {
                if (cur.item.Equals(item))
                    return ind;
                else
                    cur = cur.next;
                ind++;
            }
            return -1;

            //return IndexOfRecurse(item, head, 0);
        }

        /// <summary>
        /// Recursively moves down the chain of items
        /// </summary>
        /// <param name="cur">Current node</param>
        /// <param name="index">Remaining length</param>
        /// <returns>Node at the specified index</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private Node GoTo(Node cur, int index)
        {
            if (index == 0)
                return cur;
            else if (cur.next == null)
                throw new IndexOutOfRangeException();
            else if (index >= count)
                throw new ArgumentException("Index > Count");
            /*else
                return GoTo(cur.next, index - 1);*/
            else
            {
                try
                {
                    while (index-- > 0)
                        cur = cur.next;
                }
                catch (NullReferenceException e)
                {
                    UnityEngine.Debug.Log("GoTo encountered NullReferenceException at " + (index + 1));
                    UnityEngine.Debug.Log("List count: " + count);
                    throw e;
                }
                return cur;
            }
        }

        /// <summary>
        /// Get a string representation of the list
        /// </summary>
        /// <returns>String representation of the list</returns>
        public override string ToString()
        {
            string ret = "{ ";

            Node cur = head;
            while (cur != null)
            {
                ret += cur.item.ToString() + ", ";
                cur = cur.next;
            }
            return ret + " } Count: " + count;
        }

        /// <summary>
        /// Resets the list parameters
        /// </summary>
        public void Clear()
        {
            count = 0;
            head = null;
        }

        public void CopyTo(T[] arr, int index)
        {
            Node temp = head;
            while (temp != null)
            {
                arr[index++] = temp.item;
                temp = temp.next;
            }
        }

        public T TakeLast()
        {
            if (head == null)
            {
                throw new System.IndexOutOfRangeException("The list is empty");
            }
            else if (count == 1)
            {
                Node temp = head;
                head = null;
                count--;
                return temp.item;
            }
            else
            {
                Node prev = GoTo(head, count - 1);
                Node temp = prev.next;
                prev.next = null;
                count--;
                return temp.item;
            }
        }

        public void RemoveFirst()
        {
            if (head == null)
            {
                throw new System.IndexOutOfRangeException("The list is empty");
            }

            head = head.next;
            count--;
        }

        public void RemoveLast()
        {
            if (head == null)
            {
                throw new System.IndexOutOfRangeException("The list is empty");
            }
            else if (count == 1)
            {
                Node temp = head;
                head = null;
                count--;
            }
            else
            {
                Node prev = GoTo(head, count - 1);
                Node temp = prev.next;
                prev.next = null;
                count--;
            }
        }

        public int FirstIndexOf(T item)
        {
            int index = 0;
            Node temp = head;
            for (; head != null; index++)
            {
                if (item.Equals(temp.item))
                {
                    return index;
                }
            }
            return -1;
        }

        public int LastIndexOf(T item)
        {
            int c = 0;
            int index = -1;
            foreach (T value in this)
            {
                if (value.Equals(item))
                {
                    index = c;
                }
                c++;
            }
            return index;
        }

        public T[] ToArray()
        {
            T[] array = new T[this.Count];
            int index = 0;
            foreach (T item in this)
            {
                array[index++] = item;
            }
            return array;
        }
    }
}
