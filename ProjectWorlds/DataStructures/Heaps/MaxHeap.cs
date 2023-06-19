using ProjectWorlds.DataStructures.Queues;
using System;
using System.Text;

namespace ProjectWorlds.DataStructures.Heaps
{
    public class MaxHeap<T> where T : IComparable
    {
        public int Count { get { return count; } }

        private T[] buffer;
        private int count;

        public MaxHeap(int capacity)
        {
            buffer = new T[capacity];
            count = 0;
        }

        public bool Insert(T value)
        {
            if (count >= buffer.Length)
                return false;

            // Insert the new item at the end of the buffer
            buffer[count] = value;
            // Get the index of the item and increment count
            int cur = count;

            int parent = ParentIndex(cur);

            // While the new node is larger than the parent, swap the current node with the parent
            while (cur > 0 && buffer[cur].CompareTo(buffer[parent]) > 0)
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

        public T DeleteMax()
        {
            if (count == 0)
                return default(T);

            T temp = buffer[0];

            buffer[0] = buffer[count - 1];
            count--;

            FixHeap(0);

            return temp;
        }

        private void FixHeap(int index)
        {
            int left = (index * 2) + 1;
            int right = (index * 2) + 2;

            //if (left < count && (buffer[right] == null || buffer[index].CompareTo(buffer[right]) <= 0))
            if (left < count && (buffer[index].CompareTo(buffer[left]) < 0))
            {
                T tempValue = buffer[left];
                buffer[left] = buffer[index];
                buffer[index] = tempValue;
            }
            //else if (right < count && (buffer[left] == null || buffer[left].CompareTo(buffer[right]) > 0))
            if (right < count && (buffer[index].CompareTo(buffer[right]) < 0))
            {
                T tempValue = buffer[right];
                buffer[right] = buffer[index];
                buffer[index] = tempValue;
            }
            if (left < count)
                FixHeap(left);
            if (right < count)
                FixHeap(right);
        }

        public bool Contains(T item)
        {
            if (count == 0)
                return false;
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
                    else if (comp > 0)
                    {
                        queue.Enqueue((cur * 2) + 1);
                        queue.Enqueue((cur * 2) + 2);
                    }
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].CompareTo(item) == 0)
                {
                    count--;
                    for (int j = i; j < count; j++)
                        buffer[j] = buffer[j + 1];
                    FixHeap(0);

                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            /*String ret = "[ ";

            for(int i = 0; i < count; i++)
            {
                ret += ' ' + buffer[i].ToString();
                if (i + 1 < count)
                    ret += ',';
            }
            return ret + " ]";*/
            String str = string.Empty;

            int numLayers = Log2(count);

            int c = 0;
            for (int i = 0; i < numLayers + 1; i++)
            {
                int spaces = (int)(Math.Pow(2, numLayers + 1) - Math.Pow(2, i)) * 2;
                for (int j = 0; j < spaces; j++)
                    str += ' ';
                for (int j = 0; j < Math.Pow(2, i); j++)
                {
                    if (c < count)
                        str += buffer[c++].ToString() + " ";
                    else
                        str += "-- ";
                }
                str += '\n';
            }
            return str;
        }

        private int Log2(int x)
        {
            return (int)(Math.Log(x) / Math.Log(2));
        }

        /*public string Dump()
        {
            string ret = string.Empty;
            int height = log2(count) + 1;

            *//*for (int i = 1, len = count; i < len; i++)
            {
                T x = buffer[i];
                int level = log2(i) + 1;
                int spaces = (height - level + 1) * 2;

                ret += stringOfSize(spaces, ' ') + x;

                if ((int)Math.Pow(2, level) - 1 == i) 
                    ret += '\n';
            }*//*
            for (int i = 0; i <= height; i++)
            {
                for (int j = 0; j < Math.Pow(2, i) && j + Math.Pow(2, i) <= count; j++)
                { // Each row has 2^n nodes
                    ret += (buffer[j + (int)Math.Pow(2, i) - 1] + " ");
                }
                ret += '\n';
            }

            return ret;
        }*/

        /*public string printHeap()
        {
            int maxDepth = (int)(Math.Log(count) / Math.Log(2));  // log base 2 of n
            int c;

            StringBuilder hs = new StringBuilder();  // heap string builder
            for (int d = maxDepth; d >= 0; d--)
            {  // number of layers, we build this backwards
                int layerLength = (int)Math.Pow(2, d);  // numbers per layer

                StringBuilder line = new StringBuilder();  // line string builder
                for (int i = layerLength; i < (int)Math.Pow(2, d + 1); i++)
                {
                    // before spaces only on not-last layer
                    if (d != maxDepth)
                    {
                        c = (int)Math.Pow(2, maxDepth - d);
                        while(c-- > 0)
                            line.Append(' ');
                    }
                    // extra spaces for long lines
                    int loops = maxDepth - d;
                    if (loops >= 2)
                    {
                        loops -= 2;
                        while (loops >= 0)
                        {
                            c = (int)Math.Pow(2, loops);
                            while (c-- > 0)
                                line.Append(' ');
                            loops--;
                        }
                    }

                    // add in the number
                    if (i < count)
                        line.Append(buffer[i].ToString());  // add leading zeros
                    else
                        line.Append("--");

                    c = (int)Math.Pow(2, maxDepth - d);
                    while (c-- > 0)
                        line.Append(' ');
                      // after spaces
                                                                              // extra spaces for long lines
                    loops = maxDepth - d;
                    if (loops >= 2)
                    {
                        loops -= 2;
                        while (loops >= 0)
                        {
                            c = (int)Math.Pow(2, loops);
                            while (c-- > 0)
                                line.Append(' ');
                            loops--;
                        }
                    }
                }
                hs.Insert(0, line.ToString() + '\n');   // prepend line
            }
            return hs.ToString();
        }*/

        /*public string Funct()
        {
            String str = string.Empty;

            int numLayers = log2(count);

            int c = 0;
            for(int i = 0; i < numLayers + 1; i++)
            {
                int spaces = (int)(Math.Pow(2, numLayers + 1) - Math.Pow(2, i)) * 2;
                for (int j = 0; j < spaces; j++)
                    str += ' ';
                for(int j = 0; j < Math.Pow(2, i); j++)
                {
                    if(c < count)
                        str += buffer[c++].ToString() + " ";
                    else
                        str += "-- ";
                }
                str += '\n';
            }
            return str;
        }*/

        /*private String stringOfSize(int size, char ch)
        {
            string ret = string.Empty;
            while (size-- >= 0)
                ret += ch;
            return ret;
        }*/

        /*private int log2(int x)
        {
            return (int)(Math.Log(x) / Math.Log(2)); // = log(x) with base 10 / log(2) with base 10
        }*/
    }
}
