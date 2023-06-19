using System;
using System.Collections;
using System.Collections.Generic;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Dictionaries
{
    public class SkipDict<TKey, TValue> : ProjectWorlds.DataStructures.Lists.SkipList<Pair<TKey, TValue>>, System.Collections.IDictionary, System.Collections.Generic.IDictionary<TKey, TValue>
    {
        public object this[object key]
        {
            get
            {
                if (typeof(TKey) != key.GetType() || Count == 0)
                {
                    return default(TValue);
                }

                TKey k = (TKey)key;
                Node temp = head;
                
                while (true)
                {
                    // The node is before the requested value
                    if (temp.Value.Key.GetHashCode() < k.GetHashCode())
                    {
                        if (temp.next != null)
                        { 
                            if (temp.next.Key.GetHashCode() <= key.GetHashCode())
                            {
                                temp = temp.next;
                            }
                            else
                            {
                                if (temp.down != null)
                                {
                                    temp = temp.down;
                                }
                                else
                                {
                                    return default(TValue);
                                }
                            }
                        }
                        else
                        {
                            if (temp.down != null)
                            {
                                temp = temp.down;
                            }
                            else
                            {
                                return default(TValue);
                            }
                        }

                        temp = temp.next;
                    }
                    // The node is after the requested value
                    else if (temp.Value.Key.GetHashCode() > k.GetHashCode())
                    {
                        return default(TValue);
                    }
                    else
                    {
                        if (temp.Value.Key.Equals(k))
                        {
                            return temp.Value.Value;
                        }
                        else
                        {
                            if (temp.next != null && temp.next.Value.Key.GetHashCode() == key.GetHashCode())
                            {
                                temp = temp.next;
                            }
                            else
                            {
                                if (temp.down != null)
                                {
                                    temp = temp.down;
                                }
                                else
                                {
                                    return default(TValue);
                                }
                            }
                        }
                    }
                }
            }
            set
            {
                if (Contains(key))
                {
                    Node temp = head;
                    while (temp.Value.Key.GetHashCode() < key.GetHashCode())
                    {
                        if (temp.next != null && temp.next.GetHashCode() <= key.GetHashCode())
                        {
                            temp = temp.next;
                        }
                        else
                        {
                            temp = temp.down;
                        }
                    }
                    while (!temp.Value.Key.Equals(key))
                    {
                        if (temp.next != null && temp.next.Key.GetHashCode() == key.GetHashCode())
                        {
                            temp = temp.next;
                        }
                        else
                        {
                            temp = temp.down;
                        }
                    }
                    if (temp.Value.Key.Equals(key))
                    {
                        while (temp != null)
                        {
                            temp.Value.Value = (TValue)value;
                            temp = temp.down;
                        }
                    }
                }
                else
                {
                    Add(new Pair<TKey,TValue>((TKey)key, (TValue)value));
                }
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public ICollection Keys
        {
            get
            {
                ArrayList<TKey> keys = new ArrayList<TKey>(count);
                Node temp = head;
                while (temp.down != null)
                {
                    temp = temp.down;
                }
                int index = 0;
                while (temp != null)
                {
                    keys[index++] = temp.Value.Key;
                }
                return (ICollection)keys;
            }
        }

        public ICollection Values
        {
            get
            {
                ArrayList<TValue> keys = new ArrayList<TValue>(count);
                Node temp = head;
                while (temp.down != null)
                {
                    temp = temp.down;
                }
                int index = 0;
                while (temp != null)
                {
                    keys[index++] = temp.Value.Value;
                }
                return (ICollection)keys;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        protected object synrRoot = new object();
        public object SyncRoot
        {
            get
            {
                return synrRoot;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => throw new NotImplementedException();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new NotImplementedException();

        public TValue this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        public bool Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        public void CopyTo(Array array, int index)
        {
            // ToDo
            throw new NotImplementedException();
        }

        public void Remove(object keyValue)
        {
            Remove((TKey)keyValue);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            // ToDo
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            if (!Contains(key))
            {
                Add(new Pair<TKey, TValue>((TKey)key, (TValue)value));
            }
            else
            {
                // ToDo, add flag to alloe/disallow duplicate values
                throw new System.ArgumentException("Does not support adding duplicates");
            }
        }

        public bool ContainsKey(TKey key)
        {
            Node temp = head;
            while (temp != null)
            {
                if (temp.Value.Key.Equals(key))
                {
                    return true;
                }
                else
                {
                    if (temp.next == null || temp.next.Key.GetHashCode() > key.GetHashCode())
                    {
                        temp = temp.down;
                    }
                    else
                    {
                        temp = temp.next;
                    }
                }
            }
            return false;
        }

        public bool Remove(TKey keyValue)
        {
            int key = keyValue.GetHashCode();
            //if(head.Value.Equals(item))
            if (head.Key == key)
            {
                if (count == 1)
                {
                    head = null;
                }
                else
                {
                    // Get the next item in the list
                    Node cur = head;
                    while (cur.down != null)
                    {
                        cur = cur.down;
                    }
                    Pair<TKey, TValue> repl = cur.next.Value;
                    key = cur.next.Key;

                    cur = head;
                    while (cur != null)
                    {
                        cur.Value = repl;
                        cur.Key = key;
                        // If the next item contains the item, then 
                        if (cur.next != null && cur.next.Value.Key.Equals(keyValue))
                        {
                            cur.next = cur.next.next;
                        }
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
                {
                    count--;
                }
                return removedItem;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (typeof(TKey) != key.GetType() || Count == 0)
            {
                value = default(TValue);
                return false;
            }

            TKey k = (TKey)key;
            Node temp = head;

            while (true)
            {
                // The node is before the requested value
                if (temp.Value.Key.GetHashCode() < k.GetHashCode())
                {
                    if (temp.next != null)
                    {
                        if (temp.next.Key.GetHashCode() <= key.GetHashCode())
                        {
                            temp = temp.next;
                        }
                        else
                        {
                            if (temp.down != null)
                            {
                                temp = temp.down;
                            }
                            else
                            {
                                value = default(TValue);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (temp.down != null)
                        {
                            temp = temp.down;
                        }
                        else
                        {
                            value = default(TValue);
                            return false;
                        }
                    }

                    temp = temp.next;
                }
                // The node is after the requested value
                else if (temp.Value.Key.GetHashCode() > k.GetHashCode())
                {
                    value = default(TValue);
                    return false;
                }
                else
                {
                    if (temp.Value.Key.Equals(k))
                    {
                        value = temp.Value.Value;
                        return true;
                    }
                    else
                    {
                        if (temp.next != null && temp.next.Value.Key.GetHashCode() == key.GetHashCode())
                        {
                            temp = temp.next;
                        }
                        else
                        {
                            if (temp.down != null)
                            {
                                temp = temp.down;
                            }
                            else
                            {
                                value = default(TValue);
                                return false;
                            }
                        }
                    }
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(new Pair<TKey, TValue>(item.Key, item.Value));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (Pair<TKey,TValue> pair in this)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)pair.Key, (TValue)pair.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
