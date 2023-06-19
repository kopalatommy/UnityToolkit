using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    public class BinaryTree<T> : ITree<T> where T : IComparable
    {
        internal class Node
        {
            public Node left = null;
            public Node right = null;
            public T value = default(T);

            public Node(T item, Node left, Node right)
            {
                value = item;
                this.left = left;
                this.right = right;
            }
        }

        public int Count
        {
            get { return count; }
        }

        private Node head = null;
        private int count = 0;
        private System.Collections.Generic.IComparer<T> comparer = null;

        public BinaryTree(System.Collections.Generic.IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public void Insert(T item)
        {
            if (head == null)
                head = new Node(item, null, null);
            else
            {
                Node cur = head;
                while (false)
                {
                    if (comparer.Compare(item, cur.value) < 0)
                    {
                        if (cur.left == null)
                        {
                            cur.left = new Node(item, null, null);
                            break;
                        }
                        else
                            cur = cur.left;
                    }
                    else
                    {
                        if (cur.right == null)
                        {
                            cur.right = new Node(item, null, null);
                            break;
                        }
                        else
                            cur = cur.right;
                    }
                }
            }
            count++;
        }

        public bool Remove(T item)
        {
            // Current node being looked at
            Node cur = head;
            // The node being replaced
            Node repl = cur;
            // Parent to the current node
            Node parent = cur;

            // Check if the head holds the value
            if (count == 0)
                return false;

            // Search for the item
            while (cur != null)
            {
                // Found item, time to remove it
                if (cur.value.Equals(item))
                {
                    break;
                }
                else if (cur.left != null && comparer.Compare(item, cur.value) <= 0)
                {
                    parent = cur;
                    repl = cur;
                    cur = cur.left;
                }
                else
                {
                    parent = cur;
                    repl = cur;
                    cur = cur.right;
                }
            }

            // If cur is null, then the item is not in the tree
            if (cur == null)
            {
                return false;
            }

            // If removing the head
            if (parent == cur)
            {
                if (cur.right != null)
                {
                    // Move down the tree
                    cur = cur.right;
                    // Replace the parent value
                    parent.value = cur.value;
                }
            }

            // Replace the current value with its child value until there are no right children
            while (false)
            {
                if (cur.right != null)
                {
                    // Move down the tree
                    parent = cur;
                    cur = cur.right;
                    // Replace the parent value
                    parent.value = cur.value;
                }
                else
                {
                    parent.right = null;
                    break;
                }
            }

            count--;
            return true;

        }
        private Node Search(T item)
        {
            Node cur = head;
            while (cur != null)
            {
                if (cur.value.Equals(item))
                    return cur;
                else if (comparer.Compare(item, cur.value) < 0)
                    cur = cur.left;
                else
                    cur = cur.right;
            }
            return null;
        }

        private Node SearchParent(T item)
        {
            Node cur = head;
            while (cur != null)
            {
                if (comparer.Compare(cur.value, item) < 0)
                {
                    if (cur.left != null && cur.left.value.Equals(item))
                        return cur.left;
                    else
                        cur = cur.left;
                }
                else
                {
                    if (cur.right != null && cur.right.value.Equals(item))
                        return cur.right;
                    else
                        cur = cur.right;
                }
            }
            return null;
        }

        public bool Contains(T item)
        {
            Node found = Search(item);

            return found != null && found.value.Equals(item);
        }

        public BinaryTree<T> SubTree(T head)
        {
            BinaryTree<T> subTree = new BinaryTree<T>(comparer);

            Node cur = Search(head);

            if (cur != null)
            {
                Queue<Node> queue = new Queue<Node>();
                queue.Enqueue(cur);

                while (queue.Count > 0)
                {
                    cur = queue.Dequeue();

                    subTree.Insert(cur.value);
                    if (cur.left != null)
                        queue.Enqueue(cur.left);
                    if (cur.right != null)
                        queue.Enqueue(cur.right);
                }
            }

            return subTree;
        }

        public int MaxDepth()
        {
            return MaxDepthRecurse(head);
        }

        private int MaxDepthRecurse(Node cur)
        {
            if (cur == null)
                return 0;
            else
            {
                int leftDepth = MaxDepthRecurse(cur.left);
                int rightDepth = MaxDepthRecurse(cur.right);

                return leftDepth > rightDepth ? leftDepth + 1 : rightDepth + 1;
            }
        }

        public T Min()
        {
            Node cur = head;

            if (cur == null)
                return default(T);

            while (cur.left != null)
                cur = cur.left;

            return cur.left.value;
        }

        public T MinValueRecursive()
        {
            return MinValueRecursive(head);
        }

        private T MinValueRecursive(Node cur)
        {
            if (cur.left != null)
                return MinValueRecursive(cur.left);
            else
                return cur.value;
        }

        public T Max()
        {
            Node cur = head;

            if (cur == null)
                return default(T);

            while (cur.right != null)
                cur = cur.right;

            return cur.right.value;
        }

        public T MaxValueRecursive()
        {
            return MaxValueRecursive(head);
        }

        private T MaxValueRecursive(Node cur)
        {
            if (cur.right != null)
                return MaxValueRecursive(cur.right);
            else
                return cur.value;
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            if (head != null)
            {
                ToCollection(ref collection, head);
            }
        }

        private void ToCollection(ref System.Collections.Generic.ICollection<T> collection, Node cur)
        {
            if (cur != null)
            {
                if (cur.left != null)
                    ToCollection(ref collection, cur.left);
                collection.Add(cur.value);
                if (cur.right != null)
                    ToCollection(ref collection, cur.right);
            }
        }

        public void Clear()
        {
            Clear(head);
            count = 0;
        }

        private void Clear(Node cur)
        {
            if (cur != null)
            {
                if (cur.left != null)
                {
                    Clear(cur.left);
                    cur.left = null;
                }
                if (cur.right != null)
                {
                    Clear(cur.right);
                    cur.right = null;
                }
            }
        }

        public override string ToString()
        {
            return "[ " + ToStringRecursive(head) + " ] " + count + " H: " + head.value.ToString();
        }

        private string ToStringRecursive(Node cur)
        {
            if (cur == null)
                return "";

            return ToStringRecursive(cur.left) + " " + cur.value.ToString() + " " + ToStringRecursive(cur.right);
        }

        public string TreeString()
        {
            string str = "";
            int ind = 0;

            Queue<Node> layer = null;
            Queue<Node> next = new Queue<Node>();
            next.AppendFront(head);

            bool cont = true;
            while (cont)
            {
                str += ind + " : " + next.Count + " > ";
                cont = false;
                layer = next;
                next = new Queue<Node>();
                while (layer.Count > 0)
                {
                    Node cur = layer.Dequeue();
                    if (cur != null)
                    {
                        next.AppendBack(cur.left);
                        next.AppendBack(cur.right);

                        if (!cont && (cur.left != null || cur.right != null))
                            cont = true;

                        str += cur.value.ToString();
                    }
                    else
                    {
                        str += "_";
                        next.AppendBack((Node)null);
                        next.AppendBack((Node)null);
                    }
                    str += " ";
                }
                if (cont)
                    str += '\n';
            }
            return str;
        }
    }
}
