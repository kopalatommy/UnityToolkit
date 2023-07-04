using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    public class ComparablePair<TKey, TValue> : IComparable where TKey : IComparable<TKey>
    {
        public TKey key;
        public TValue value;

        public ComparablePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public int CompareTo(object obj)
        {
            return key.CompareTo((TKey)obj);
        }
    }

    // Largest wheight first
    public class OrderedList<T> : DoubleLinkedList<T> where T : IComparable
    {
        public override void Set(int index, T item)
        {
            Add(item);
        }

        public override void Add(T item)
        {
            if (head == null)
            {
                head = tail = new Node(item, null, null);
            }
            else
            {
                if (item.CompareTo(head.item) < 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.CompareTo(tail.item) > 0)
                {
                    tail = new DoubleLinkedList<T>.Node(item, tail, null);
                }
                else
                {
                    Node temp = head;
                    while (temp.Next != null && temp.Next.item.CompareTo(item) < 0)
                    {
                        temp = temp.Next;
                    }
                    temp.Next = new Node(item, temp, temp.Next);
                }
            }
            count++;
        }

        public override void AppendFront(T item)
        {
            Add(item);
        }

        public override void Add(IEnumerable<T> other)
        {
            AppendBack(other);
        }

        public override void AppendBack(T item)
        {
            Add(item);
        }

        public override void AppendBack(IEnumerable<T> other)
        {
            Add(other);
        }

        public override void Insert(int index, T item)
        {
            Add(item);
        }

        public override void Insert(int index, IEnumerable<T> other)
        {
            Add(other);
        }

        public override void Insert(int index, IEnumerable<T> other, int length)
        {
            foreach (T item in other)
            {
                length--;
                if (length < 0)
                {
                    return;
                }

                Add(item);
            }
        }
    }

    public class OrderedList<TKey, TValue> : DoubleLinkedList<ComparablePair<TKey, TValue>> where TKey : IComparable<TKey>
    {
        public void Set(int index, TKey key, TValue item)
        {
            Set(index, new ComparablePair<TKey, TValue>(key, item));
        }

        public override void Set(int index, ComparablePair<TKey, TValue> item)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            Add(item);
        }

        public void Add(TKey key, TValue item)
        {
            Add(new ComparablePair<TKey, TValue>(key, item));
        }

        public override void Add(ComparablePair<TKey, TValue> item)
        {
            if (head == null)
            {
                head = tail = new Node(item, null, null);
            }
            else
            {
                if (item.CompareTo(head.item) < 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.CompareTo(tail.item) > 0)
                {
                    tail = new Node(item, tail, null);
                }
                else
                {
                    Node temp = head;
                    while (temp.Next != null && temp.Next.item.CompareTo(item) < 0)
                    {
                        temp = temp.Next;
                    }
                    temp.Next = new Node(item, temp, temp.Next);

                    if (temp.Next == null)
                    {
                        tail = temp;
                    }
                }
            }
            count++;
        }

        public void AppendFront(TKey key, TValue item)
        {
            Add(new ComparablePair<TKey, TValue>(key, item));
        }

        public override void AppendFront(ComparablePair<TKey, TValue> item)
        {
            Add(item);
        }

        public override void Add(IEnumerable<ComparablePair<TKey, TValue>> other)
        {
            Add(other);
        }

        public void AppendBack(TKey key, TValue item)
        {
            Add(new ComparablePair<TKey, TValue>(key, item));
        }

        public override void AppendBack(ComparablePair<TKey, TValue> item)
        {
            Add(item);
        }

        public override void AppendBack(IEnumerable<ComparablePair<TKey, TValue>> other)
        {
            Add(other);
        }

        public void Insert(int index, TKey key, TValue item)
        {
            Add(new ComparablePair<TKey, TValue>(key, item));
        }

        public override void Insert(int index, ComparablePair<TKey, TValue> item)
        {
            Add(item);
        }

        public override void Insert(int index, IEnumerable<ComparablePair<TKey, TValue>> other)
        {
            Add(other);
        }

        public override void Insert(int index, IEnumerable<ComparablePair<TKey, TValue>> other, int length)
        {
            foreach (ComparablePair<TKey, TValue> item in other)
            {
                length--;
                if (length < 0)
                {
                    return;
                }

                Add(item);
            }
        }
    }
}
