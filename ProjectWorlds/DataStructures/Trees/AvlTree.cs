using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    public class AvlTree<T> : ITree<T> where T : System.IComparable
    {
        internal class Node
        {
            public Node left;
            public Node right;
            public int height;
            public T value;

            public Node(T value)
            {
                height = 1;
                this.value = value;
                left = right = null;
            }
        }

        public int Count { get { return count; } }

        private Node root = null;
        private int count = 0;

        public bool Contains(T item)
        {
            Node cur = root;
            int comp;
            while (cur != null)
            {
                comp = item.CompareTo(cur.value);
                if (comp == 0)
                    break;
                else if (comp < 0)
                    cur = cur.left;
                else
                    cur = cur.right;
            }
            return cur != null;
        }

        private Node RotateRight(Node y)
        {
            Node x = y.left;
            Node T2 = x.right;
            x.right = y;
            y.left = T2;
            y.height = (Height(y.left) > Height(y.right) ? Height(y.left) : Height(y.right)) + 1;
            x.height = (Height(x.left) > Height(x.right) ? Height(x.left) : Height(x.right)) + 1;
            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.right;
            Node T2 = y.left;
            y.left = x;
            x.right = T2;
            y.height = (Height(y.left) > Height(y.right) ? Height(y.left) : Height(y.right)) + 1;
            x.height = (Height(x.left) > Height(x.right) ? Height(x.left) : Height(x.right)) + 1;
            return y;
        }

        public void Insert(T item)
        {
            root = insertNode(root, item);
        }

        private Node insertNode(Node node, T item)
        {

            // Find the position and insert the node
            if (node == null)
                return (new Node(item));

            int comp = item.CompareTo(node.value);
            if (comp < 0)
                node.left = insertNode(node.left, item);
            else if (comp > 0)
                node.right = insertNode(node.right, item);
            else
                return node;

            // Update the balance factor of each node
            // And, balance the tree
            node.height = (Height(node.left) > Height(node.right) ? Height(node.left) : Height(node.right)) + 1;
            int balanceFactor = BalanceFactor(node);
            if (balanceFactor > 1)
            {
                comp = item.CompareTo(node.left.value);
                if (comp < 0)
                {
                    return RotateRight(node);
                }
                else if (comp > 0)
                {
                    node.left = RotateLeft(node.left);
                    return RotateRight(node);
                }
            }
            if (balanceFactor < -1)
            {
                comp = item.CompareTo(node.right.value);
                if (comp > 0)
                {
                    return RotateLeft(node);
                }
                else if (comp < 0)
                {
                    node.right = RotateRight(node.right);
                    return RotateLeft(node);
                }
            }
            return node;
        }

        private Node MinNode(Node node)
        {
            Node current = node;
            while (current.left != null)
                current = current.left;
            return current;
        }

        private int Height(Node node)
        {
            return node != null ? node.height : 0;
        }

        private int BalanceFactor(Node node)
        {
            return node == null ? 0 : Height(node.left) - Height(node.right);
        }

        public bool Remove(T item)
        {
            return deleteNode(root, item) != null;
        }

        // Delete a node
        Node deleteNode(Node root, T item)
        {
            // Find the node to be deleted and remove it
            if (root == null)
                return root;

            int comp = item.CompareTo(root.value);
            if (comp < 0)
                root.left = deleteNode(root.left, item);
            else if (comp > 0)
                root.right = deleteNode(root.right, item);
            else
            {
                if ((root.left == null) || (root.right == null))
                {
                    Node temp = null;
                    if (temp == root.left)
                        temp = root.right;
                    else
                        temp = root.left;
                    if (temp == null)
                    {
                        temp = root;
                        root = null;
                    }
                    else
                        root = temp;
                }
                else
                {
                    Node temp = MinNode(root.right);
                    root.value = temp.value;
                    root.right = deleteNode(root.right, temp.value);
                }
            }
            if (root == null)
                return root;

            // Update the balance factor of each node and balance the tree
            root.height = (Height(root.left) > Height(root.right) ? Height(root.left) : Height(root.right)) + 1;
            int balanceFactor = BalanceFactor(root);
            if (balanceFactor > 1)
            {
                if (BalanceFactor(root.left) >= 0)
                {
                    return RotateRight(root);
                }
                else
                {
                    root.left = RotateLeft(root.left);
                    return RotateRight(root);
                }
            }
            if (balanceFactor < -1)
            {
                if (BalanceFactor(root.right) <= 0)
                {
                    return RotateLeft(root);
                }
                else
                {
                    root.right = RotateRight(root.right);
                    return RotateLeft(root);
                }
            }
            return root;
        }

        public T Min()
        {
            Node cur = root;

            if (root == null)
                return default(T);

            while (cur.left != null)
                cur = cur.left;

            return cur.left.value;
        }

        public T Max()
        {
            Node cur = root;

            if (root == null)
                return default(T);

            while (cur.right != null)
                cur = cur.right;

            return cur.right.value;
        }

        public void Clear()
        {
            count = 0;
            root = null;
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            if (root != null)
                ToCollection(root, ref collection);
        }

        private void ToCollection(Node cur, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (cur != null)
            {
                ToCollection(cur.left, ref collection);
                collection.Add(cur.value);
                ToCollection(cur.right, ref collection);
            }
        }

        public override string ToString()
        {
            if (root != null)
            {
                return ToStringRecursive(root) + "\nCount: " + count.ToString();
            }
            else
                return "Count: " + count;
        }

        private string ToStringRecursive(Node cur)
        {
            if (cur == null)
                return "";

            string ret = string.Empty;

            ret += ToStringRecursive(cur.left);
            ret += " " + cur.value.ToString() + " ";
            ret += ToStringRecursive(cur.right);

            return ret;
        }

        public string TreeString()
        {
            string str = "";
            int ind = 0;

            Queue<Node> layer = null;
            Queue<Node> next = new Queue<Node>();
            next.AppendFront(root);

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

                        str += BalanceFactor(cur);
                        str += '<' + cur.value.ToString() + '>';
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
