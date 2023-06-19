using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    public class AVLTreeV2<T> : ITree<T> where T : IComparable
    {
        internal class Node
        {
            public int height;
            public Node left, right;
            public T item;

            public Node(T item)
            {
                this.item = item;
                height = 1;
            }
        }

        public int Count
        {
            get { return count; }
        }

        private Node root = null;
        private int count = 0;

        public bool Contains(T item)
        {
            Node cur = root;
            int comp;
            while (cur != null)
            {
                comp = item.CompareTo(cur.item);
                if (comp == 0)
                    break;
                else if (comp < 0)
                    cur = cur.left;
                else
                    cur = cur.right;
            }
            return cur != null;
        }

        int height(Node node)
        {
            if (node == null)
                return 0;
            return node.height;
        }

        int max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        Node rightRotate(Node y)
        {
            Node x = y.left;
            Node T2 = x.right;
            x.right = y;
            y.left = T2;
            y.height = max(height(y.left), height(y.right)) + 1;
            x.height = max(height(x.left), height(x.right)) + 1;
            return x;
        }

        Node leftRotate(Node x)
        {
            Node y = x.right;
            Node T2 = y.left;
            y.left = x;
            x.right = T2;
            x.height = max(height(x.left), height(x.right)) + 1;
            y.height = max(height(y.left), height(y.right)) + 1;
            return y;
        }

        // Get balance factor of a node
        int getBalanceFactor(Node N)
        {
            if (N == null)
                return 0;
            return height(N.left) - height(N.right);
        }

        public void Insert(T item)
        {
            bool success = false;
            root = insertNode(root, item, out success);
            if (success)
                count++;
        }

        // Insert a node
        private Node insertNode(Node node, T item, out bool success)
        {

            // Find the position and insert the node
            if (node == null)
            {
                success = true;
                return (new Node(item));
            }

            int comp = item.CompareTo(node.item);
            if (comp < 0)
                node.left = insertNode(node.left, item, out success);
            else if (comp > 0)
                node.right = insertNode(node.right, item, out success);
            else
            {
                success = true;
                return node;
            }

            // Update the balance factor of each node
            // And, balance the tree
            node.height = 1 + max(height(node.left), height(node.right));
            int balanceFactor = getBalanceFactor(node);
            if (balanceFactor > 1)
            {
                comp = item.CompareTo(node.left.item);
                if (comp < 0)
                {
                    return rightRotate(node);
                }
                else if (comp > 0)
                {
                    node.left = leftRotate(node.left);
                    return rightRotate(node);
                }
            }
            if (balanceFactor < -1)
            {
                comp = item.CompareTo(node.right.item);
                if (comp > 0)
                {
                    return leftRotate(node);
                }
                else if (comp < 0)
                {
                    node.right = rightRotate(node.right);
                    return leftRotate(node);
                }
            }
            return node;
        }

        Node nodeWithMimumValue(Node node)
        {
            Node current = node;
            while (current.left != null)
                current = current.left;
            return current;
        }

        public bool Remove(T item)
        {
            bool success = false;
            root = deleteNode(root, item, ref success);

            if (success)
                count--;
            return success;
        }

        // Delete a node
        Node deleteNode(Node root, T item, ref bool success)
        {
            // Find the node to be deleted and remove it
            if (root == null)
            {
                success = false;
                return root;
            }

            int comp = item.CompareTo(root.item);
            if (comp < 0)
                root.left = deleteNode(root.left, item, ref success);
            else if (comp > 0)
                root.right = deleteNode(root.right, item, ref success);
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
                    Node temp = nodeWithMimumValue(root.right);
                    root.item = temp.item;
                    root.right = deleteNode(root.right, temp.item, ref success);
                }
            }
            if (root == null)
            {
                success = true;
                return root;
            }

            // Update the balance factor of each node and balance the tree
            root.height = max(height(root.left), height(root.right)) + 1;
            int balanceFactor = getBalanceFactor(root);
            if (balanceFactor > 1)
            {
                if (getBalanceFactor(root.left) >= 0)
                {
                    return rightRotate(root);
                }
                else
                {
                    root.left = leftRotate(root.left);
                    return rightRotate(root);
                }
            }
            if (balanceFactor < -1)
            {
                if (getBalanceFactor(root.right) <= 0)
                {
                    return leftRotate(root);
                }
                else
                {
                    root.right = rightRotate(root.right);
                    return leftRotate(root);
                }
            }
            success = true;
            return root;
        }

        public T Min()
        {
            Node cur = root;

            if (root == null)
                return default(T);

            while (cur.left != null)
                cur = cur.left;

            return cur.left.item;
        }

        public T Max()
        {
            Node cur = root;

            if (root == null)
                return default(T);

            while (cur.right != null)
                cur = cur.right;

            return cur.right.item;
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
                collection.Add(cur.item);
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
            ret += " " + cur.item.ToString() + " ";
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

                        str += getBalanceFactor(cur);
                        str += '<' + cur.item.ToString() + '>';
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
