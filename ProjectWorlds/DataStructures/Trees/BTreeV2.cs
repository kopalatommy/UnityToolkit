using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    public class BTreeV2<T> : ITree<T> where T : IComparable
    {
        internal class Node
        {
            // Items tracked by the node
            public T[] items = null;
            // Children attached to the node
            public Node[] children = null;
            // Number of items tracked by the node
            public int numItems = 0;
            // Does this node have children
            public bool isLeaf = false;

            public Node(int degree, bool leaf)
            {
                // Copy the minimum degree and leaf property
                this.isLeaf = leaf;

                // Allocate memory for the maximum number of possible keys and child pointers
                items = new T[2 * degree - 1];
                children = new Node[2 * degree];

                // Initialize the number of keys as 0
                numItems = 0;
            }
        }

        public int Count { get { return count; } }

        private Node root = null;
        private int count = 0;
        private int degree = 0;

        public BTreeV2(int degree)
        {
            this.degree = degree;
            count = 0;
            root = null;
        }

        public void Insert(T item)
        {
            // If tree is empty, then the insertion is just creating a new root
            if (root == null)
            {
                // Allocate memory for root 
                root = new Node(degree, true);
                root.items[0] = item;
                root.numItems = 1;
            }
            else
            {
                // If root is full, then tree grows in height 
                if (root.numItems == 2 * degree - 1)
                {
                    // Allocate memory for new root 
                    Node s = new Node(degree, false);

                    // Make old root as child of new root 
                    s.children[0] = root;

                    // Split the old root and move 1 key to the new root
                    SplitChild(s, 0, root);

                    // New root has two children now. Decide which of the 
                    // two children is going to have new key
                    int i = 0;
                    if (s.items[0].CompareTo(item) < 0)
                    {
                        i++;
                    }
                    InsertNonFull(s.children[i], item);

                    // Change root
                    root = s;
                }
                else
                    InsertNonFull(root, item);
            }
            count++;
        }

        private void SplitChild(Node cur, int index, Node full)
        {
            Node nNode = new Node(degree, full.isLeaf);
            nNode.numItems = degree - 1;

            // Copy the first half of the nodes from the full node to the current node
            for (int i = 0; i < degree - 1; i++)
                nNode.items[i] = full.items[i + degree];

            // If the nod being split is not a leaf, then also need to copy the children nodes
            if (!full.isLeaf)
                for (int i = 0; i < degree; i++)
                    nNode.children[i] = full.children[i + degree];

            full.numItems = degree - 1;

            // Make room for the new node
            for (int i = cur.numItems; i >= index + 1; i--)
                cur.children[i + 1] = cur.children[i];

            cur.children[index + 1] = nNode;

            for (int i = cur.numItems - 1; i >= index; i--)
                cur.items[i + 1] = cur.items[i];

            // Copy the middle key of full to this node 
            cur.items[index] = full.items[degree - 1];

            cur.numItems++;
        }

        private void InsertNonFull(Node cur, T item)
        {
            int index = cur.numItems - 1;

            if (cur.isLeaf)
            {
                while (index >= 0 && cur.items[index].CompareTo(item) > 0)
                {
                    cur.items[index + 1] = cur.items[index];
                    index--;
                }

                cur.items[index + 1] = item;
                cur.numItems++;
            }
            else
            {
                while (index >= 0 && cur.items[index].CompareTo(item) > 0)
                    index--;

                if (cur.children[index + 1].numItems == 2 * degree - 1)
                {
                    SplitChild(cur, index + 1, cur.children[index + 1]);

                    if (cur.items[index + 1].CompareTo(item) < 0)
                        index++;
                }
                InsertNonFull(cur.children[index + 1], item);
            }
        }

        public bool Remove(T item)
        {
            if (root == null)
                return false;

            bool ret = Remove(root, item);

            if (root.numItems == 0)
                root = root.children[0];

            if (ret)
                count--;
            return ret;
        }

        private bool Remove(Node cur, T item)
        {
            int index = FindKey(cur, item);

            if (index < cur.numItems && cur.items[index].CompareTo(item) == 0)
            {
                if (cur.isLeaf)
                {
                    RemoveFromLeaf(cur, index);
                    return true;
                }
                else
                    return RemoveFromNonLeaf(cur, index);
            }
            else
            {
                if (cur.isLeaf)
                    return false;

                bool flag = (index == cur.numItems);

                if (cur.children[index].numItems < degree)
                    Fill(cur, index);

                if (flag && index > cur.numItems)
                    return Remove(cur.children[index - 1], item);
                else
                    return Remove(cur.children[index], item);
            }
        }

        private int FindKey(Node cur, T item)
        {
            int index = 0;
            while (index < cur.numItems && cur.items[index].CompareTo(item) < 0)
                index++;
            return index;
        }

        private void RemoveFromLeaf(Node cur, int index)
        {
            int i = 0;
            try
            {
                for (i = index + 1; i < cur.numItems; i++)
                    cur.items[i - 1] = cur.items[i];
                cur.numItems--;
            }
            catch
            {
                throw new InvalidOperationException("Num items: " + cur.numItems + " : " + cur.items.Length + " : " + i);
            }
        }

        private bool RemoveFromNonLeaf(Node cur, int index)
        {
            T item = cur.items[index];

            if (cur.children[index].numItems >= degree)
            {
                T pred = GetPredecessor(cur, index);
                cur.items[index] = pred;
                return Remove(cur.children[index], pred);
            }
            else if (cur.children[index + 1].numItems >= degree)
            {
                T succ = GetSuccessor(cur, index);
                cur.items[index] = succ;
                return Remove(cur.children[index + 1], succ);
            }
            else
            {
                Merge(cur, index);
                return Remove(cur.children[index], item);
            }
        }

        private T GetPredecessor(Node cur, int index)
        {
            cur = cur.children[index];
            while (!cur.isLeaf)
                cur = cur.children[cur.numItems];
            return cur.items[cur.numItems - 1];
        }

        private T GetSuccessor(Node cur, int index)
        {
            cur = cur.children[index + 1];
            while (!cur.isLeaf)
                cur = cur.children[0];
            return cur.items[0];
        }

        private void Fill(Node cur, int index)
        {
            if (index != 0 && cur.children[index - 1].numItems >= degree)
                BorrowFromPrev(cur, index);
            else if (index != cur.numItems && cur.children[index + 1].numItems >= degree)
                BorrowFromNext(cur, index);
            else
            {
                if (index != cur.numItems)
                    Merge(cur, index);
                else
                    Merge(cur, index - 1);
            }
        }

        private void BorrowFromPrev(Node cur, int index)
        {
            Node child = cur.children[index];
            Node sibling = cur.children[index - 1];

            for (int i = child.numItems - 1; i >= 0; i--)
                child.items[i + 1] = child.items[i];

            if (!child.isLeaf)
            {
                for (int i = child.numItems; i >= 0; i--)
                    child.children[i + 1] = child.children[i];
            }

            child.items[0] = cur.items[index - 1];

            if (!child.isLeaf)
                child.children[0] = sibling.children[sibling.numItems];

            cur.items[index - 1] = sibling.items[sibling.numItems - 1];

            child.numItems++;
            sibling.numItems--;
        }

        private void BorrowFromNext(Node cur, int index)
        {
            Node child = cur.children[index];
            Node sibling = cur.children[index + 1];

            child.items[child.numItems] = cur.items[index];

            if (!child.isLeaf)
                child.children[child.numItems + 1] = sibling.children[0];

            cur.items[index] = sibling.items[0];

            for (int i = 1; i < sibling.numItems; i++)
                sibling.items[i - 1] = sibling.items[i];

            if (!sibling.isLeaf)
            {
                for (int i = 1; i <= sibling.numItems; i++)
                    sibling.children[i - 1] = sibling.children[i];
            }

            child.numItems++;
            sibling.numItems--;
        }

        private void Merge(Node cur, int index)
        {
            Node child = cur.children[index];
            Node sibling = cur.children[index + 1];

            child.items[degree - 1] = cur.items[index];

            for (int i = 0; i < sibling.numItems; i++)
                child.items[i + degree] = sibling.items[i];

            if (!child.isLeaf)
            {
                for (int i = 0; i <= sibling.numItems; i++)
                    child.children[i + degree] = sibling.children[i];
            }

            for (int i = index + 1; i < cur.numItems; i++)
                cur.items[i - 1] = cur.items[i];

            for (int i = index + 2; i <= cur.numItems; i++)
                cur.children[i - 1] = cur.children[i];

            child.numItems += sibling.numItems + 1;
            cur.numItems--;
        }

        public bool Contains(T item)
        {
            if (root == null)
                return false;
            else
            {
                Node temp = Find(root, item);
                if (temp != null)
                {
                    for (int i = 0; i < temp.numItems; i++)
                        if (temp.items[i].CompareTo(item) == 0)
                            return true;
                }

                return false;
            }
        }

        private Node Find(Node cur, T item)
        {
            int i = 0;
            while (i < cur.numItems && item.CompareTo(cur.items[i]) > 0)
                i++;

            if (i != cur.numItems && cur.items[i].CompareTo(item) == 0)
                return cur;

            if (cur.isLeaf)
                return null;

            return Find(cur.children[i], item);
        }

        public T Min()
        {
            Node cur = root;

            while (cur != null)
            {
                if (cur.isLeaf)
                    return cur.items[0];
                else
                    cur = cur.children[0];
            }
            return default(T);
        }

        public T Max()
        {
            Node cur = root;

            while (cur != null)
            {
                if (cur.isLeaf)
                    return cur.items[cur.numItems - 1];
                else
                    cur = cur.children[cur.numItems];
            }
            return default(T);
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            if (root != null)
            {
                ToCollection(root, ref collection);
            }
        }

        // ToDo, remove recursion by using stacks?
        private void ToCollection(Node cur, ref System.Collections.Generic.ICollection<T> collection)
        {
            int i;
            for (i = 0; i < cur.numItems; i++)
            {
                if (!cur.isLeaf)
                {
                    ToCollection(cur.children[i], ref collection);
                }
                collection.Add(cur.items[i]);
            }

            if (!cur.isLeaf)
            {
                ToCollection(cur.children[i], ref collection);
            }
        }

        public void Clear()
        {
            if (root != null)
            {
                Clear(root);
            }

            count = 0;
        }

        private void Clear(Node cur)
        {
            if (!cur.isLeaf)
            {
                for (int i = 0; i <= cur.numItems; i++)
                {
                    Clear(cur.children[i]);
                    cur.children[i] = null;
                }
            }
            cur.items = null;
            cur.children = null;
        }

        public string TreeString()
        {
            string toRet = string.Empty;

            Queue<Node> curLayer = new Queue<Node>();
            Queue<Node> next = new Queue<Node>();

            next.AppendBack(root);

            int layer = 0;
            bool cont = true;
            while (cont)
            {
                curLayer = next;
                next = new Queue<Node>();
                cont = false;

                toRet += layer + ": ";

                while (curLayer.Count > 0)
                {
                    Node node = curLayer.Dequeue();

                    if (node == null)
                    {
                        toRet += "<NULL>";
                        continue;
                    }
                    if (node.isLeaf)
                    {
                        toRet += "L";
                    }
                    else
                    {
                        toRet += "N";
                    }
                    toRet += "," + node.numItems;
                    toRet += "<";
                    for (int i = 0; i < node.numItems; i++)
                    {
                        toRet += node.items[i].ToString();
                        if (i + 1 < node.numItems)
                        {
                            toRet += ',';
                        }
                        if (!node.isLeaf)
                        {
                            next.AppendBack(node.children[i]);
                        }
                    }
                    next.AppendBack(node.children[node.numItems]);
                    toRet += '>';
                    if (!node.isLeaf)
                    {
                        cont = true;
                    }
                }
                toRet += '\n';
                layer++;
            }
            return toRet;
        }
    }
}
