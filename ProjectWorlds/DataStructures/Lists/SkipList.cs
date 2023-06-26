using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWorlds.DataStructures.Lists
{
    public class SkipList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, IListExtended<T>
    {
        public class Enumerator : IEnumerator, IEnumerator<T>
        {
            public T Current
            {
                get
                {
                    return cur.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return cur.Value;
                }
            }

            protected SkipList<T> list = null;
            protected Node cur = null;
            protected bool isDisposed = false;
            private bool starting = true;

            public Enumerator(SkipList<T> list)
            {
                this.list = list;
                Reset();
            }

            public bool MoveNext()
            {
                if (starting)
                {
                    starting = false;
                    cur = list.head;

                    while (cur != null && cur.down != null)
                    {
                        cur = cur.down;
                    }

                    return cur != null;
                }

                if (cur != null)
                {
                    cur = cur.next;
                    return cur != null;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                cur = null;
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
                        isDisposed = true;
                    }

                    // Indicate that the instance has been disposed.
                    isDisposed = true;
                }
            }
        }

        public class Node
        {
            public Node down = null;
            public Node next = null;

            public int Key
            {
                get { return key; }
                set { key = value; }
            }
            public T Value
            {
                get { return value; }
                set { this.value = value; }
            }
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            private int key;
            private T value;
            private int height;

            public Node(int key, T value, Node next, Node down)
            {
                this.key = key;
                this.value = value;
                this.next = next;
                this.down = down;
            }
        }

        protected int height = 1;
        protected Node head = null;
        protected int count = 0;

        System.Random random = new System.Random();

        public int Height { get { return height; } }

        public Node Head { get { return head; } }

        public int Count { get { return count; } }

        public bool IsReadOnly => throw new NotImplementedException();

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

        public SkipList()
        {

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Add(T item)
        {
            int key = item.GetHashCode();
            if (head == null)
            {
                height = count = 1;
                head = new Node(key, item, null, null);
                head.Height = height;
            }
            else if (item.GetHashCode() < head.Value.GetHashCode())
            {
                T oldHead = head.Value;

                Node temp = head;
                while (temp != null)
                {
                    temp.Value = item;
                    temp.Key = key;
                    temp = temp.down;
                }

                Add(oldHead);
            }
            else
            {
                int height = DetermineNodeHeight();
                Node cur = head;

                // Go to the new node's height
                while (cur.Height > height)
                {
                    cur = cur.down;
                }

                // This builds down, needs to keep track of the previously created node
                // to maintan the chain
                Node created = null;
                while (true)
                {
                    // Get the node that is before the new nodes location
                    while (cur.next != null && cur.next.Key < key)
                    {
                        cur = cur.next;
                    }
                    cur.next = new Node(key, item, cur.next, null);
                    cur.next.Height = cur.Height;
                    if (created == null)
                    {
                        created = cur.next;
                    }
                    else
                    {
                        created.down = cur.next;
                        created = created.down;
                    }
                    if (cur.down != null)
                    {
                        cur = cur.down;
                    }
                    else
                    {
                        break;
                    }
                }
                count++;
            }
        }

        protected int DetermineNodeHeight()
        {
            int maxNodeHeight = height + 1;
            int nodeHeight = 1;
            while (random.NextDouble() < 0.5 && nodeHeight < maxNodeHeight)
            {
                nodeHeight++;
            }

            if (nodeHeight > height)
            {
                height = nodeHeight;
                if (head.Height < height)
                {
                    Node n = new Node(head.Key, head.Value, null, head);
                    n.Height = height;
                    head = n;
                }
            }

            return nodeHeight;
        }

        public Node Search(int key)
        {
            Node cur = head;

            while (cur != null)
            {
                while (cur.next != null && cur.next.Key <= key)
                {
                    cur = cur.next;
                    if (cur.Key == key)
                        return cur;
                }

                if (cur.down != null)
                    cur = cur.down;
                else
                    break;
            }

            return cur;
        }

        public bool Contains(T item)
        {
            Node node = Search(item.GetHashCode());

            return node != null && node.Value.Equals(item);
        }

        public bool Remove(T item)
        {
            int key = item.GetHashCode();
            //if(head.Value.Equals(item))
            if (head.Key == key)
            {
                if (count == 1)
                    head = null;
                else
                {
                    // Get the next item in the list
                    Node cur = head;
                    while (cur.down != null)
                        cur = cur.down;
                    T repl = cur.next.Value;
                    key = cur.next.Key;

                    cur = head;
                    while (cur != null)
                    {
                        cur.Value = repl;
                        cur.Key = key;
                        // If the next item contains the item, then 
                        if (cur.next != null && cur.next.Value.Equals(item))
                            cur.next = cur.next.next;
                        cur = cur.down;
                    }
                }
                count--;
                return true;
            }
            else
            {
                bool removedItem = false;
                Node cur = head;

                while (cur != null)
                {
                    while (cur.next != null)
                    {
                        // If the item doesn't match, then move down the layer
                        if (cur.next.Key < key)
                        {
                            cur = cur.next;
                        }
                        // Check if the next item matches the one to remove
                        else if (cur.next.Key == key)
                        {
                            cur.next = cur.next.next;
                            removedItem = true;
                            break;
                        }
                        else
                            break;
                    }
                    cur = cur.down;
                }
                if (removedItem)
                    count--;
                return removedItem;
            }
        }

        public void Clear()
        {
            head = null;
            count = 0;
            height = 1;
        }

        public override string ToString()
        {
            string ret = "{ ";

            Node cur = head;
            while (cur.down != null)
            {
                cur = cur.down;
            }

            while (cur != null)
            {
                ret += cur.Value.ToString();
                if (cur.next != null)
                {
                    ret += ", ";
                }
                cur = cur.next;
            }
            return ret + " } Count: " + count;
        }

        public string DumpList()
        {
            string ret = "Height: " + height + "\n";

            Node cur = head;

            while (cur != null)
            {
                ret += cur.Height + " | ";
                Node lev = cur;
                while (lev != null)
                {
                    ret += lev.Value + ", ";
                    lev = lev.next;
                }
                ret = ret.Remove(ret.Length - 2, 2) + "|\n";
                cur = cur.down;
            }

            int h = 0;
            cur = head;
            while (cur != null)
            {
                h++;
                cur = cur.down;
            }
            ret += "H: " + h + " Count: " + count;

            return ret;
        }

        public string DumpTop()
        {
            Node cur = head;
            string ret = ">";

            while (cur != null)
            {
                while (cur.next != null)
                {
                    ret += " " + cur.Value + " ";
                    cur = cur.next;
                    if (cur.next != null)
                        ret += '>';
                }
                cur = cur.down;
                if (cur != null)
                    ret += "* " + cur.Value + " ";
            }

            return ret;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Does not use index, only here for interface
        public void Set(int index, T value)
        {
            Add(value);
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            Node temp = head;
            while (temp.down != null)
            {
                temp = temp.down;
            }

            for (; index > 0; index--)
            {
                temp = temp.next;
            }
            return temp.Value;
        }

        public T Front()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            return head.Value;
        }

        public T Back()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            Node temp = head;
            while (temp.down != null)
            {
                temp = temp.down;
            }
            while (temp.next != null)
            {
                temp = temp.next;
            }
            return temp.Value;
        }

        public void Add(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Add(item);
            }
        }

        public void Insert(int index, IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Add(item);
            }
        }

        public void Insert(int index, IEnumerable<T> other, int length)
        {
            foreach (T item in other)
            {
                Add(item);

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

            if (count == 1)
            {
                T item = head.Value;
                Clear();
                return item;
            }
            else
            {
                // Get the next node in the list
                Node next = head;
                T item = head.Value;
                while (next.down != null)
                {
                    next = next.down;
                }
                next = next.next;


                // Remove the next node from the list
                Node temp = head;
                while (temp != null)
                {
                    temp.Value = next.Value;
                    temp.Key = next.Key;

                    if (temp.next != null && temp.next.Key == next.Key)
                    {
                        temp.next = temp.next.next;
                    }
                    temp = temp.down;
                }
                count--;
                return item;
            }
        }

        public T TakeAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                if (count == 1)
                {
                    T item = head.Value;
                    Clear();
                    return item;
                }
                else
                {
                    // Get the next node in the list
                    Node next = head;
                    T item = head.Value;
                    while (next.down != null)
                    {
                        next = next.down;
                    }
                    next = next.next;


                    // Remove the next node from the list
                    Node temp = head;
                    while (temp != null)
                    {
                        temp.Value = next.Value;
                        temp.Key = next.Key;

                        if (temp.next != null && temp.next.Key == next.Key)
                        {
                            temp.next = temp.next.next;
                        }
                        temp = temp.down;
                    }
                    count--;
                    return item;
                }
            }

            // Get the item to be removed
            Node toRemove = head;
            while (toRemove.down != null)
            {
                toRemove = toRemove.down;
            }

            for (; index > 0; index--)
            {
                toRemove = toRemove.next;
            }

            Node highest = head;
            while (highest != null)
            {
                if (highest.next == null)
                {
                    highest = highest.down;
                }
                else if (highest.next.Key < toRemove.Key)
                {
                    highest = highest.next;
                }
                else if (highest.next.Key > toRemove.Key)
                {
                    highest = highest.down;
                }
                else
                {
                    highest.next = highest.next.next;
                    highest = highest.down;
                }
            }
            count--;
            return toRemove.Value;
        }

        public T TakeLast()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            // Get the item to be removed
            Node toRemove = head;
            while (toRemove.down != null)
            {
                toRemove = toRemove.down;
            }

            for (int index = count - 1; index > 0; index--)
            {
                toRemove = toRemove.next;
            }

            Node highest = head;
            while (highest.Key < toRemove.Key)
            {
                if (highest.next != null)
                {
                    highest = highest.next;
                }
                else
                {
                    highest = highest.down;
                }
            }

            // Perform the actual removal
            while (highest != null)
            {
                if (highest.next == null)
                {
                    highest = highest.down;
                }
                else if (highest.next.Key == toRemove.Key)
                {
                    highest.next = highest.next.next;
                    highest = highest.down;
                }
                else
                {
                    highest = highest.next;
                }
            }
            count--;
            return toRemove.Value;
        }

        public void RemoveFirst()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (count == 1)
            {
                Clear();
            }
            else
            {
                // Get the next node in the list
                Node next = head;
                T item = head.Value;
                while (next.down != null)
                {
                    next = next.down;
                }
                next = next.next;


                // Remove the next node from the list
                Node temp = head;
                while (temp != null)
                {
                    temp.Value = next.Value;
                    temp.Key = next.Key;

                    if (temp.next != null && temp.next.Key == next.Key)
                    {
                        temp.next = temp.next.next;
                    }
                    temp = temp.down;
                }
                count--;
            }
        }

        public void RemoveLast()
        {
            if (count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            // Get the item to be removed
            Node toRemove = head;
            while (toRemove.down != null)
            {
                toRemove = toRemove.down;
            }

            for (int index = count - 1; index > 0; index--)
            {
                toRemove = toRemove.next;
            }

            Node highest = head;
            while (highest.Key < toRemove.Key)
            {
                if (highest.next != null)
                {
                    highest = highest.next;
                }
                else
                {
                    highest = highest.down;
                }
            }

            // Perform the actual removal
            while (highest != null)
            {
                if (highest.next == null)
                {
                    highest = highest.down;
                }
                else if (highest.next.Key == toRemove.Key)
                {
                    highest.next = highest.next.next;
                    highest = highest.down;
                }
                else
                {
                    highest = highest.next;
                }
            }
            count--;
        }

        public void RemoveRange(int start, int length)
        {
            if (start == 0 && count == length)
            {
                Clear();
            }
            else if (length == 1)
            {
                RemoveAt(start);
            }
            else if (start == 0)
            {
                Node temp = head;
                while (temp.down != null)
                {
                    temp = temp.down;
                }

                Node toRemove = temp;
                for (; length > 0; length--)
                {
                    toRemove = toRemove.next;
                }

                temp = head;
                // Drop all items that are before toRemove
                while (head.Value.GetHashCode() < toRemove.Value.GetHashCode())
                {
                    head = head.next;
                }

                // Test if the bottom node of the current is the last one to be removed
                while (true)
                {
                    Node t = head;
                    while (t.down != null)
                    {
                        t = t.down;
                    }

                    // If the bottom node is the last item to be removed, 
                    // move head once more to finish removing the items
                    if (t == toRemove)
                    {
                        head = head.next;
                        break;
                    }
                    // Not the right node, move to the next one and repeat process
                    else
                    {
                        head = head.next;
                    }
                }
            }
            else
            {
                Node beforeToRemove = head;
                Node afterToRemove = head;
                // Move the before and after references
                for (; start > 0; start--)
                {
                    beforeToRemove = afterToRemove = beforeToRemove.next;
                }
                // Fixes an edge case where all items have the same hash code
                IList<Node> removeCol = new ArrayList<Node>(length);

                // Move the after reference the rest of the way
                for (; length > 0; length--)
                {
                    afterToRemove = afterToRemove.next;
                    removeCol.Add(afterToRemove);
                }
                afterToRemove = afterToRemove.next;

                // Start testing layers to check if items are in the toRemove range
                Node layer = head;
                while (layer != null)
                {
                    Node temp = layer;
                    Node before = layer;
                    // Move to the node before the toRemove range (might not be in this layer)
                    while (temp.Value.GetHashCode() <= beforeToRemove.Value.GetHashCode())
                    {
                        // If the hash code is equal, check the bottom of the next node
                        // to see if it is in the to remove
                        if (temp.next.Value.GetHashCode() == beforeToRemove.GetHashCode())
                        {
                            Node t = temp.next;
                            while (t.down != null)
                            {
                                t = t.down;
                            }
                            // The next node needs to be removed, break from loop
                            if (removeCol.Contains(t))
                            {
                                break;
                            }
                            // The next node should not be removed, move to next node
                            else
                            {
                                before = temp;
                                temp = temp.next;
                            }
                        }
                        else
                        {
                            before = temp;
                            temp = temp.next;
                        }
                    }
                    // If the layer contains items in the toRemove range
                    while (temp != null)
                    {
                        // If the current node doesnt have a next, then the rest of the layer
                        // after next is within the range being removed. Set the next value of 
                        // before to null
                        if (temp.next == null)
                        {
                            before.next = null;
                            break;
                        }
                        // Test if the next node is a part of the toRemove collection
                        else if (temp.next.Value.GetHashCode() == afterToRemove.Value.GetHashCode())
                        {
                            while (true)
                            {
                                // Get the bottom node
                                Node t = temp.next;
                                while (t.down != null)
                                {
                                    t = t.down;
                                }
                                // If the bottom node is apart of the collection to be 
                                // removed, move to the next node 
                                if (removeCol.Contains(temp))
                                {
                                    temp = temp.next;
                                }
                                // After the remove range, update the next value in before to
                                // remove the range from the layer
                                else
                                {
                                    before.next = temp;
                                    break;
                                }
                            }
                            break;
                        }
                        else if (temp.next.GetHashCode() < afterToRemove.Value.GetHashCode())
                        {
                            temp = temp.next;
                        }
                        // Check if the layer doesnt contain the range
                        else if (temp.next.GetHashCode() > afterToRemove.Value.GetHashCode())
                        {
                            // Nothing to update
                            break;
                        }
                    }
                    layer = layer.down;
                }
                count -= length;
            }
        }

        public void RemoveAll(T item)
        {
            int hashCode = item.GetHashCode();
            Node layer = head;
            while (layer != null)
            {
                Node temp = layer;
                bool didChangeLayer = false;
                bool isBottomLayer = layer.down == null;
                // Move to the portion of the layer that contains the item
                while (temp != null && temp.Value.GetHashCode() < item.GetHashCode())
                {
                    didChangeLayer = true;
                    layer = temp.down;
                    temp = temp.next;
                }
                while (temp.next != null && temp.Value.GetHashCode() == hashCode)
                {
                    if (temp.Value.Equals(item))
                    {
                        temp.next = temp.next.next;
                        // Only update te count value on the bottom layer
                        if (isBottomLayer)
                        {
                            count--;
                        }
                    }
                    else
                    {
                        temp = temp.next;
                    }
                }    
                if (!didChangeLayer)
                {
                    layer = layer.down;
                }
            }
        }

        public int FirstIndexOf(T item)
        {
            if (count == 0)
            {
                return -1;
            }

            int hashCode = item.GetHashCode();
            Node temp = head;
            while (temp.down != null)
            {
                temp = temp.down;
            }

            int index = 0;
            while (item != null)
            {
                if (temp.Value.GetHashCode() < hashCode)
                {
                    // Before item in list
                    temp = temp.next;
                }
                else if (temp.Value.GetHashCode() == hashCode)
                {
                    // Could be item
                    if (temp.Value.Equals(item))
                    {
                        return index;
                    }
                    else
                    {
                        temp = temp.next;
                    }
                }
                else
                {
                    // Item not in list
                    return -1;
                }

                index++;
            }
            return -1;
        }

        public int LastIndexOf(T item)
        {
            if (count == 0)
            {
                return -1;
            }

            int hashCode = item.GetHashCode();

            Node temp = head;
            while (temp.down != null)
            {
                temp = temp.down;
            }

            int indexToReturn = -1;
            int index = 0;
            while (temp != null)
            {
                if (temp.Key == hashCode)
                {
                    indexToReturn = index;
                }
                index++;
                temp = temp.next;
            }
            return indexToReturn;
        }

        public T[] ToArray()
        {
            T[] array = new T[this.Count];
            Node temp = head;
            while (temp.down != null)
            {
                temp = temp.down;
            }
            int index = 0;
            while (temp != null)
            {
                // UnityEngine.Debug.Log(index + " / " + Count + " - " + temp.Value);
                array[index++] = temp.Value;
                temp = temp.next;
            }
            return array;
        }

        public int IndexOf(T item)
        {
            return FirstIndexOf(item);
        }

        // Since this is an ordered list, this just calls add
        public void Insert(int index, T item)
        {
            Add(item);
        }

        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                T temp = head.Value;
                if (count == 1)
                {
                    head = null;
                }
                else
                {
                    // Get the bottom node for this column
                    Node bottom = head;
                    while (bottom.down != null)
                    {
                        bottom = bottom.down;
                    }

                    // Get the next item in the list
                    T next = bottom.next.Value;
                    bottom = head;
                    // Go over all nodes, and replace the current column with
                    // the next item. Drop the next column from the list
                    while (bottom.down != null)
                    {
                        // Replace item in coulumn
                        bottom.Value = next;
                        // Drop the next node if the node contains the next item
                        if (bottom.next.Value.Equals(next))
                        {
                            bottom.next = bottom.next.next;
                        }
                    }

                }
                count--;
            }
            else
            {
                Node last = head;
                // Move to bottom row
                while (last.down != null)
                {
                    last = last.down;
                }

                // Move to second to last node
                for (; index > 0; index--)
                {
                    last = last.next;
                }
                Node toRemove = last.next;

                Node top = head;
                while (top.Value.GetHashCode() < last.Value.GetHashCode())
                {
                    top = top.next;
                }

                Node tNode = top;
                while (tNode.down != null)
                {
                    tNode = tNode.down;
                }
                if (tNode == last)
                {
                    while (top != null)
                    {
                        if (top.next.Value.Equals(toRemove.Value))
                        {
                            top.next = top.next.next;
                        }
                        top = top.down;
                    }
                }

                count--;
            }
        }
    }
}
