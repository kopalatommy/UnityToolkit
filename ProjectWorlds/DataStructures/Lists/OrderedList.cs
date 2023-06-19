using System;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    // Largest wheight first
    public class OrderedList<TItem, UWheight> : DoubleLinkedList<Pair<TItem, UWheight>> where UWheight : IComparable<UWheight>
    {
        public static OrderedList<TItem, UWheight> operator +(OrderedList<TItem, UWheight> self, IListExtended<Pair<TItem, UWheight>> other)
        {
            OrderedList<TItem, UWheight> toRet = new OrderedList<TItem, UWheight>();

            toRet.Add(self);
            toRet.Add(other);

            return toRet;
        }

        public void AppendFront(TItem item, UWheight wheight)
        {
            AppendFront(new Pair<TItem, UWheight>(item, wheight));
        }

        public override void AppendFront(Pair<TItem, UWheight> toAdd)
        {
            if (head == null)
            {
                head = tail = new Node(toAdd, null, null);
            }
            else if (toAdd.Value.CompareTo(head.item.Value) > 0)
            {
                head = new Node(toAdd, null, head);
            }
            else
            {
                Node cur = head;
                while (cur != null && toAdd.Value.CompareTo(cur.item.Value) < 0)
                {
                    cur = cur.Next;
                }

                if (cur == null)
                {
                    tail = new Node(toAdd, tail, null);
                }
                else
                {
                    // The node will handle assigning node references
                    new Node(toAdd, cur.Prev, cur);
                }
            }
            count++;
        }

        public void AppendBack(TItem item, UWheight wheight)
        {
            AppendBack(new Pair<TItem, UWheight>(item, wheight));
        }

        public override void AppendBack(Pair<TItem, UWheight> toAdd)
        {
            if (head == null)
            {
                head = tail = new Node(toAdd, null, null);
            }
            else if (toAdd.Value.CompareTo(tail.item.Value) < 0)
            {
                tail = new Node(toAdd, tail, null);
            }
            else
            {
                Node cur = tail;
                while (cur != null && toAdd.Value.CompareTo(cur.item.Value) >= 0)
                {
                    cur = cur.Prev;
                }

                if (cur == null)
                {
                    head = new Node(toAdd, null, head);
                }
                else
                {
                    // The node will handle assigning node references
                    new Node(toAdd, cur, cur.Next);
                }
            }
            count++;
        }

        public override void AppendBack(IEnumerable<Pair<TItem, UWheight>> other)
        {
            // Since the list isn't guaranteed to be sorted, a lot of comparisons
            // will be done. Starting in the center should decrease those checks
            // by a fair margin on average
            Node center = GoToForward(head, (count / 2));
            Node cur;
            foreach (Pair<TItem, UWheight> item in other)
            {
                // This list might be empty, need to account for that
                if (head == null)
                {
                    head = tail = new Node(item, null, null);
                }
                // If the node falls outside the bounds of the min and max nodes,
                // an edge case occurs with the preious method of adding items
                else if (item.Value.CompareTo(head.item.Value) > 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.Value.CompareTo(tail.item.Value) < 0)
                {
                    tail = new Node(item, tail, null);
                }
                else
                {
                    cur = center;

                    if (item.Value.CompareTo(cur.item.Value) > 0)
                    {
                        cur = cur.Prev;
                        while (cur != null && item.Value.CompareTo(cur.item.Value) > 0)
                        {
                            cur = cur.Prev;
                        }
                        if (cur == null)
                        {
                            head = new Node(item, null, head);
                        }
                        else
                        {
                            new Node(item, cur, cur.Next);
                        }
                    }
                    else if (item.Value.CompareTo(cur.item.Value) < 0)
                    {
                        cur = cur.Next;
                        while (cur != null && item.Value.CompareTo(cur.item.Value) < 0)
                        {
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            tail = new Node(item, tail, null);
                        }
                        else
                        {
                            new Node(item, cur.Prev, cur);
                        }
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                count++;
            }
        }

        public void Insert(int index, TItem item, UWheight wheight)
        {
            Insert(index, new Pair<TItem, UWheight>(item, wheight));
        }

        public override void Insert(int index, Pair<TItem, UWheight> item)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("index");
            }

            if (index == 0)
            {
                AppendFront(item);
            }
            else if (index == count)
            {
                AppendBack(item);
            }
            else
            {
                Node cur;
                if (index < count / 2)
                {
                    cur = GoToForward(head, index - 1);
                }
                else
                {
                    cur = GoToReverse(tail, count - index);
                }

                if (item.Value.CompareTo(cur.item.Value) > 0)
                {
                    cur = cur.Prev;
                    while (cur != null && item.Value.CompareTo(cur.item.Value) > 0)
                    {
                        cur = cur.Prev;
                    }
                    if (cur == null)
                    {
                        head = new Node(item, null, head);
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                else if (item.Value.CompareTo(cur.item.Value) < 0)
                {
                    cur = cur.Next;
                    while (cur != null && item.Value.CompareTo(cur.item.Value) < 0)
                    {
                        cur = cur.Next;
                    }
                    if (cur == null)
                    {
                        tail = new Node(item, tail, null);
                    }
                    else
                    {
                        new Node(item, cur.Prev, cur);
                    }
                }
                else
                {
                    new Node(item, cur, cur.Next);
                }
                count++;
            }
        }

        // other is not assumed to be in order
        public override void Insert(int index, IEnumerable<Pair<TItem, UWheight>> other, int length)
        {
            // Since the list isn't guaranteed to be sorted, a lot of comparisons
            // will be done. Starting in the center should decrease those checks
            // by a fair margin on average
            Node center = index < (count / 2) ? GoToForward(head, index) : GoToReverse(tail, index);
            Node cur;
            foreach (Pair<TItem, UWheight> item in other)
            {
                // This list might be empty, need to account for that
                if (head == null)
                {
                    head = tail = new Node(item, null, null);
                }
                // If the node falls outside the bounds of the min and max nodes,
                // an edge case occurs with the preious method of adding items
                else if (item.Value.CompareTo(head.item.Value) > 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.Value.CompareTo(tail.item.Value) < 0)
                {
                    tail = new Node(item, tail, null);
                }
                else
                {
                    cur = center;

                    if (item.Value.CompareTo(cur.item.Value) > 0)
                    {
                        cur = cur.Prev;
                        while (cur != null && item.Value.CompareTo(cur.item.Value) > 0)
                        {
                            cur = cur.Prev;
                        }
                        if (cur == null)
                        {
                            head = new Node(item, null, head);
                        }
                        else
                        {
                            new Node(item, cur, cur.Next);
                        }
                    }
                    else if (item.Value.CompareTo(cur.item.Value) < 0)
                    {
                        cur = cur.Next;
                        while (cur != null && item.Value.CompareTo(cur.item.Value) < 0)
                        {
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            tail = new Node(item, tail, null);
                        }
                        else
                        {
                            new Node(item, cur.Prev, cur);
                        }
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                count++;
            }
        }

        public override void Add(IEnumerable<Pair<TItem, UWheight>> other)
        {
            AppendBack(other);
        }
    }

    public class OrderedList<T> : DoubleLinkedList<T> where T : IComparable<T>, ICollection<T>, System.Collections.Generic.IEnumerable<T>, IListExtended<T>
    {
        public static OrderedList<T> operator +(OrderedList<T> self, IListExtended<T> other)
        {
            OrderedList<T> toRet = new OrderedList<T>();

            toRet.Add(self as IEnumerable<T>);
            toRet.Add(other);

            return toRet;
        }

        public override void AppendFront(T toAdd)
        {
            if (head == null)
            {
                head = tail = new Node(toAdd, null, null);
            }
            else if (toAdd.CompareTo(head.item) > 0)
            {
                head = new Node(toAdd, null, head);
            }
            else
            {
                Node cur = head;
                while (cur != null && toAdd.CompareTo(cur.item) < 0)
                {
                    cur = cur.Next;
                }

                if (cur == null)
                {
                    tail = new Node(toAdd, tail, null);
                }
                else
                {
                    // The node will handle assigning node references
                    new Node(toAdd, cur.Prev, cur);
                }
            }
            count++;
        }

        public override void AppendBack(T toAdd)
        {
            if (head == null)
            {
                head = tail = new Node(toAdd, null, null);
            }
            else if (toAdd.CompareTo(tail.item) < 0)
            {
                tail = new Node(toAdd, tail, null);
            }
            else
            {
                Node cur = tail;
                while (cur != null && toAdd.CompareTo(cur.item) >= 0)
                {
                    cur = cur.Prev;
                }

                if (cur == null)
                {
                    head = new Node(toAdd, null, head);
                }
                else
                {
                    // The node will handle assigning node references
                    new Node(toAdd, cur, cur.Next);
                }
            }
            count++;
        }

        public override void AppendBack(IEnumerable<T> other)
        {
            // Since the list isn't guaranteed to be sorted, a lot of comparisons
            // will be done. Starting in the center should decrease those checks
            // by a fair margin on average
            Node center = GoToForward(head, (count / 2));
            Node cur;
            foreach (T item in other)
            {
                // This list might be empty, need to account for that
                if (head == null)
                {
                    head = tail = new Node(item, null, null);
                }
                // If the node falls outside the bounds of the min and max nodes,
                // an edge case occurs with the preious method of adding items
                else if (item.CompareTo(head.item) > 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.CompareTo(tail.item) < 0)
                {
                    tail = new Node(item, tail, null);
                }
                else
                {
                    cur = center;

                    if (item.CompareTo(cur.item) > 0)
                    {
                        cur = cur.Prev;
                        while (cur != null && item.CompareTo(cur.item) > 0)
                        {
                            cur = cur.Prev;
                        }
                        if (cur == null)
                        {
                            head = new Node(item, null, head);
                        }
                        else
                        {
                            new Node(item, cur, cur.Next);
                        }
                    }
                    else if (item.CompareTo(cur.item) < 0)
                    {
                        cur = cur.Next;
                        while (cur != null && item.CompareTo(cur.item) < 0)
                        {
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            tail = new Node(item, tail, null);
                        }
                        else
                        {
                            new Node(item, cur.Prev, cur);
                        }
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                count++;
            }
        }

        public override void Insert(int index, T item)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("index: " + index + " C: " + count);
            }

            if (index == 0)
            {
                AppendFront(item);
            }
            else if (index == count)
            {
                AppendBack(item);
            }
            else
            {
                Node cur;
                if (index < count / 2)
                {
                    cur = GoToForward(head, index - 1);
                }
                else
                {
                    cur = GoToReverse(tail, count - index);
                }

                if (item.CompareTo(cur.item) > 0)
                {
                    cur = cur.Prev;
                    while (cur != null && item.CompareTo(cur.item) > 0)
                    {
                        cur = cur.Prev;
                    }
                    if (cur == null)
                    {
                        head = new Node(item, null, head);
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                else if (item.CompareTo(cur.item) < 0)
                {
                    cur = cur.Next;
                    while (cur != null && item.CompareTo(cur.item) < 0)
                    {
                        cur = cur.Next;
                    }
                    if (cur == null)
                    {
                        tail = new Node(item, tail, null);
                    }
                    else
                    {
                        new Node(item, cur.Prev, cur);
                    }
                }
                else
                {
                    new Node(item, cur, cur.Next);
                }
                count++;
            }
        }

        // other is not assumed to be in order
        public override void Insert(int index, IEnumerable<T> other, int length)
        {
            // Since the list isn't guaranteed to be sorted, a lot of comparisons
            // will be done. Starting in the center should decrease those checks
            // by a fair margin on average
            Node center = index < (count / 2) ? GoToForward(head, index) : GoToReverse(tail, index);
            Node cur;
            foreach (T item in other)
            {
                // This list might be empty, need to account for that
                if (head == null)
                {
                    head = tail = new Node(item, null, null);
                }
                // If the node falls outside the bounds of the min and max nodes,
                // an edge case occurs with the preious method of adding items
                else if (item.CompareTo(head.item) > 0)
                {
                    head = new Node(item, null, head);
                }
                else if (item.CompareTo(tail.item) < 0)
                {
                    tail = new Node(item, tail, null);
                }
                else
                {
                    cur = center;

                    if (item.CompareTo(cur.item) > 0)
                    {
                        cur = cur.Prev;
                        while (cur != null && item.CompareTo(cur.item) > 0)
                        {
                            cur = cur.Prev;
                        }
                        if (cur == null)
                        {
                            head = new Node(item, null, head);
                        }
                        else
                        {
                            new Node(item, cur, cur.Next);
                        }
                    }
                    else if (item.CompareTo(cur.item) < 0)
                    {
                        cur = cur.Next;
                        while (cur != null && item.CompareTo(cur.item) < 0)
                        {
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            tail = new Node(item, tail, null);
                        }
                        else
                        {
                            new Node(item, cur.Prev, cur);
                        }
                    }
                    else
                    {
                        new Node(item, cur, cur.Next);
                    }
                }
                count++;
            }
        }

        public override void Add(IEnumerable<T> other)
        {
            AppendBack(other);
        }
    }
}
