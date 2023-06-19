using System;
using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Queues;

namespace ProjectWorlds.DataStructures.Trees
{
    public class RedBlackTree<T> : ITree<T> where T : IComparable
    {
        public enum NodeColor
        {
            Red,
            Black,
            None
        }
        internal class Node
        {
            public T value;
            public Node parent;
            public Node left;
            public Node right;
            public NodeColor nodeColor = NodeColor.None;

            public Node(T value, Node parent, NodeColor nodeColor, Node leftChild = null, Node rightChild = null)
            {
                this.value = value;
                this.parent = parent;
                this.left = leftChild;
                this.right = rightChild;
                this.nodeColor = nodeColor;
            }
        }

        public int Count { get { return count; } }

        private Node root = null;
        private int count = 0;
        private Node nullNode = null;

        public RedBlackTree()
        {
            nullNode = new Node(default(T), null, NodeColor.None);

            root = nullNode;
            root.left = nullNode;
            root.right = nullNode;
            root.parent = nullNode;

        }

        public bool Contains(T value)
        {
            Node cur = root;

            if (cur == null)
                return false;

            while (cur.nodeColor != NodeColor.None)
            {
                int comp = value.CompareTo(cur.value);
                if (comp == 0)
                    return true;
                else if (comp < 0)
                    cur = cur.left;
                else
                    cur = cur.right;
            }
            return false;
        }

        public T Min()
        {
            Node cur = root;
            if (root == null)
                return default(T);

            while (cur.left.nodeColor != NodeColor.None)
                cur = cur.left;
            return cur.left.value;
        }

        public T Max()
        {
            Node cur = root;
            if (cur == null)
                return default(T);

            while (cur.right.nodeColor != NodeColor.None)
                cur = cur.right;
            return cur.right.value;
        }

        public void Insert(T value)
        {
            Node cur = root;
            bool insertLeft = true;
            while (cur.nodeColor != NodeColor.None)
            {
                int comp = value.CompareTo(cur.value);
                if (comp <= 0)
                {
                    if (cur.left.nodeColor != NodeColor.None)
                        cur = cur.left;
                    else
                        break;
                }
                else
                {
                    if (cur.right.nodeColor != NodeColor.None)
                        cur = cur.right;
                    else
                    {
                        insertLeft = false;
                        break;
                    }
                }
            }

            Node nNode = new Node(value, cur, NodeColor.Red, nullNode, nullNode);
            // If cur == none, then the tree is empty. Make the new node the root
            if (cur.nodeColor == NodeColor.None)
            {
                root = nNode;
                root.nodeColor = NodeColor.Black;
            }
            else
            {
                if (insertLeft)
                    cur.left = nNode;
                else
                    cur.right = nNode;

                if (nNode.parent.parent.nodeColor != NodeColor.None)
                    FixInsert(nNode);
            }
            count++;
        }

        private void FixInsert(Node cur)
        {
            Node uncle;
            while (cur.parent.nodeColor == NodeColor.Red)
            {
                if (cur.parent == cur.parent.parent.right)
                {
                    uncle = cur.parent.parent.left;
                    if (uncle.nodeColor == NodeColor.Red)
                    {
                        uncle.nodeColor = NodeColor.Black;
                        cur.parent.nodeColor = NodeColor.Black;
                        cur.parent.parent.nodeColor = NodeColor.Red;
                        cur = cur.parent.parent;
                    }
                    else
                    {
                        if (cur == cur.parent.left)
                        {
                            cur = cur.parent;
                            RotateRight(cur);
                        }
                        cur.parent.nodeColor = NodeColor.Black;
                        cur.parent.parent.nodeColor = NodeColor.Red;
                        RotateLeft(cur.parent.parent);
                    }
                }
                else
                {
                    uncle = cur.parent.parent.right;

                    if (uncle.nodeColor == NodeColor.Red)
                    {
                        uncle.nodeColor = NodeColor.Black;
                        cur.parent.nodeColor = NodeColor.Black;
                        cur.parent.parent.nodeColor = NodeColor.Red;
                        cur = cur.parent.parent;
                    }
                    else
                    {
                        if (cur == cur.parent.right)
                        {
                            cur = cur.parent;
                            RotateLeft(cur);
                        }
                        cur.parent.nodeColor = NodeColor.Black;
                        cur.parent.parent.nodeColor = NodeColor.Red;
                        RotateRight(cur.parent.parent);
                    }
                }
                if (cur == root)
                {
                    break;
                }
            }
            root.nodeColor = NodeColor.Black;
        }

        private void RotateLeft(Node cur)
        {
            Node sibling = cur.right;
            cur.right = sibling.left;
            if (sibling.left.nodeColor != NodeColor.None)
                sibling.left.parent = cur;
            sibling.parent = cur.parent;
            // Only the root has a null parent
            if (cur.parent == null)
                root = sibling;
            else
            {
                if (cur == cur.parent.left)
                    cur.parent.left = sibling;
                else
                    cur.parent.right = sibling;
            }
            sibling.left = cur;
            cur.parent = sibling;
        }

        private void RotateRight(Node cur)
        {
            Node sibling = cur.left;
            cur.left = sibling.right;
            if (sibling.right.nodeColor != NodeColor.None)
                sibling.right.parent = cur;
            sibling.parent = cur.parent;
            // Only the root has a null parent
            if (cur.parent == null)
                root = sibling;
            else
            {
                if (cur == cur.parent.right)
                    cur.parent.right = sibling;
                else
                    cur.parent.left = sibling;
            }
            sibling.right = cur;
            cur.parent = sibling;
        }

        private Node Find(T value, Node cur)
        {
            if (cur == null || cur.nodeColor == NodeColor.None)
                return null;
            else
            {
                if (cur.value.CompareTo(value) == 0)
                    return cur;
                else if (cur.value.CompareTo(value) > 0)
                    return Find(value, cur.left);
                else
                    return Find(value, cur.right);
            }
        }

        public bool Remove(T value)
        {
            if (root == null)
                return false;
            else
            {
                if (Remove(value, root))
                {
                    count--;
                    return true;
                }
                else
                    return false;
            }
        }

        private bool Remove(T value, Node cur)
        {
            Node node = Find(value, cur);
            if (node != null)
            {
                Node z = node;

                Node x = nullNode;
                Node y = nullNode;

                if (z.left.nodeColor == NodeColor.None || z.right.nodeColor == NodeColor.None)
                    y = z;
                else
                    y = GetSuccessor(z);

                if (y.left.nodeColor != NodeColor.None)
                    x = y.left;
                else
                    x = y.right;

                cur.parent = y.parent;

                if (y.parent.nodeColor == NodeColor.None)
                    root = x;
                else if (y.parent.left.nodeColor != NodeColor.None && y.parent.left == y)
                    y.parent.left = x;
                else if (y.parent.right.nodeColor != NodeColor.None && y.parent.right == y)
                    y.parent.right = x;

                if (y != z)
                    z.value = y.value;



                if (y.nodeColor != NodeColor.Black)
                    FixRemove(x);

                return true;
            }
            return false;
        }

        private Node GetSuccessor(Node cur)
        {
            if (cur.right.nodeColor != NodeColor.None)
            {
                while (cur.nodeColor != NodeColor.None)
                    cur = cur.left;
                return cur;
            }
            else
            {
                Node parent = cur.parent;
                while (parent.nodeColor != NodeColor.None && cur == parent.right)
                {
                    cur = parent;
                    parent = parent.parent;
                }
                return parent;
            }
        }

        private void FixRemove(Node cur)
        {
            Node sibling;
            while (cur != root && cur.nodeColor == NodeColor.Black)
            {
                if (cur == cur.parent.left)
                {
                    sibling = cur.parent.right;
                    if (sibling.nodeColor == NodeColor.Red)
                    {
                        sibling.nodeColor = NodeColor.Black;
                        cur.parent.nodeColor = NodeColor.Red;
                        RotateLeft(cur.parent);
                        sibling = cur.parent.right;
                    }
                    if (sibling.left.nodeColor == NodeColor.Black && sibling.right.nodeColor == NodeColor.Black)
                    {
                        sibling.nodeColor = NodeColor.Red;
                        cur = cur.parent;
                    }
                    else
                    {
                        if (sibling.right.nodeColor == NodeColor.Black)
                        {
                            sibling.left.nodeColor = NodeColor.Black;
                            sibling.nodeColor = NodeColor.Red;
                            RotateRight(sibling);
                            sibling = cur.parent.right;
                        }
                        sibling.nodeColor = cur.parent.right.nodeColor;
                        cur.parent.nodeColor = NodeColor.Black;
                        sibling.right.nodeColor = NodeColor.Black;
                        RotateLeft(cur.parent);
                        cur = root;
                    }
                }
                else
                {
                    sibling = cur.parent.left;
                    if (sibling.nodeColor == NodeColor.Red)
                    {
                        sibling.nodeColor = NodeColor.Black;
                        cur.parent.nodeColor = NodeColor.Red;
                        RotateRight(cur.parent);
                        sibling = cur.parent.left;
                    }
                    if (sibling.left.nodeColor == NodeColor.Black && sibling.right.nodeColor == NodeColor.Black)
                    {
                        sibling.nodeColor = NodeColor.Red;
                        cur = cur.parent;
                    }
                    else
                    {
                        if (sibling.left.nodeColor == NodeColor.Black)
                        {
                            sibling.right.nodeColor = NodeColor.Black;
                            sibling.nodeColor = NodeColor.Red;
                            RotateLeft(sibling);
                            sibling = cur.parent.left;
                        }
                        sibling.nodeColor = cur.parent.left.nodeColor;
                        cur.parent.nodeColor = NodeColor.Black;
                        sibling.left.nodeColor = NodeColor.Black;
                        RotateRight(cur.parent);
                        cur = root;
                    }
                }
            }
            // If removed the last node
            if (cur.nodeColor != NodeColor.None)
                cur.nodeColor = NodeColor.Black;
        }

        private bool isNil(Node node)
        {
            return node.nodeColor == NodeColor.None;
        }

        private Node MinNode(Node cur)
        {
            while (cur.left.nodeColor != NodeColor.None)
            {
                cur = cur.left;
            }
            return cur;
        }

        private void rbTransplant(Node u, Node v)
        {
            if (u.parent == null)
                root = v;
            else if (u == u.parent.left)
                u.parent.left = v;
            else
                u.parent.right = v;
            v.parent = u.parent;
        }

        public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
        {
            if (root != null)
                ToCollection(root, ref collection);
        }

        // ToDo, remove recursion by using stacks?
        private void ToCollection(Node cur, ref System.Collections.Generic.ICollection<T> collection)
        {
            if (cur.nodeColor != NodeColor.None)
            {
                ToCollection(cur.left, ref collection);
                collection.Add(cur.value);
                ToCollection(cur.right, ref collection);
            }
        }

        public void Clear()
        {
            if (root != null)
                Clear(root);

            count = 0;
        }

        private void Clear(Node cur)
        {
            if (cur.nodeColor != NodeColor.None)
            {
                Clear(cur.left);
                Clear(cur.right);
                cur.left = cur.right = null;
            }
        }

        public string TreeString()
        {
            /*string toRet = string.Empty;

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

                if(curLayer.Count != Math.Pow(2, layer))
                    throw new Exception("Bad num nodes");

                while (curLayer.Count > 0)
                {
                    Node node = curLayer.Dequeue();

                    if (curLayer == next)
                        throw new Exception("HERE");

                    if (node == null)
                    {
                        toRet += "<NULL>";
                        next.AppendBack((Node)null);
                        next.AppendBack((Node)null);
                        continue;
                    }
                    else
                        cont = true;
                    if (node.nodeColor == NodeColor.Red)
                        toRet += "R";
                    else
                        toRet += "B";
                    toRet += "<";
                    toRet += node.value.ToString();
                    toRet += '>';
                    next.AppendBack(node.left);
                    next.AppendBack(node.right);
                }
                toRet += '\n';
                layer++;

                if (layer > 100)
                    throw new Exception("Not changing layers?");
            }
            return toRet;*/

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
                    if (cur != null && cur.nodeColor != NodeColor.None)
                    {
                        next.AppendBack(cur.left);
                        next.AppendBack(cur.right);

                        if (!cont && (cur.left.nodeColor != NodeColor.None || cur.right.nodeColor != NodeColor.None))
                            cont = true;

                        str += (cur.nodeColor == NodeColor.Black ? 'B' : 'R');
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
            if (cur == null || cur.nodeColor == NodeColor.None)
                return "";

            string ret = string.Empty;

            ret += ToStringRecursive(cur.left);
            ret += " " + cur.value.ToString() + " ";
            ret += ToStringRecursive(cur.right);

            return ret;
        }

        public string PrintTree()
        {
            return printHelper(root, "", true);
        }

        private string printHelper(Node root, string indent, bool last)
        {
            string ret = string.Empty;
            // print the tree structure on the screen
            if (root != null && root.nodeColor != NodeColor.None)
            {
                ret += indent;
                if (last)
                {
                    ret += "R----";
                    indent += "     ";
                }
                else
                {
                    ret += "L----";
                    indent += "|    ";
                }

                string sColor = root.nodeColor == NodeColor.Red ? "RED" : "BLACK";
                ret += root.value.ToString() + "(" + sColor + ")\n";
                ret += printHelper(root.left, indent, false);
                ret += printHelper(root.right, indent, true);
            }
            // cout<<root.left.data<<endl;
            return ret;
        }
    }
}
