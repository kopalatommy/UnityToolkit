using System;
using ProjectWorlds.DataStructures.Queues;
using ProjectWorlds.DataStructures.Stacks;

namespace ProjectWorlds.DataStructures.Trees
{
	public class RedBlackTreeV2<T> : ITree<T> where T : IComparable
	{
		class Node
		{
			public static byte BLACK = 0;
			public static byte RED = 1;
			public T value;

			public Node parent;
			public Node left;
			public Node right;
			public byte color;

			Node()
			{
				color = BLACK;
				parent = null;
				left = null;
				right = null;
			}

			public Node(T key)
			{
				this.value = key;
			}
		}

		private Node nil = new Node(default(T));
		private Node root = null;

		private int count = 0;

		public int Count { get { return count; } }

		public RedBlackTreeV2()
		{
			root = nil;
			root.left = nil;
			root.right = nil;
			root.parent = nil;
		}

		private void RotateLeft(Node x)
		{
			Node y;
			y = x.right;
			x.right = y.left;

			if (!isNil(y.left))
				y.left.parent = x;
			y.parent = x.parent;

			if (isNil(x.parent))
				root = y;
			else if (x.parent.left == x)
				x.parent.left = y;
			else
				x.parent.right = y;

			y.left = x;
			x.parent = y;
		}

		private void RotateRight(Node y)
		{
			Node x = y.left;
			y.left = x.right;

			if (!isNil(x.right))
				x.right.parent = y;
			x.parent = y.parent;

			if (isNil(y.parent))
				root = x;
			else if (y.parent.right == y)
				y.parent.right = x;
			else
				y.parent.left = x;

			x.right = y;
			y.parent = x;
		}

		public void Insert(T value)
		{
			Node toAdd = new Node(value);

			Node parent = nil;
			Node cur = root;

			while (!isNil(cur))
			{
				parent = cur;

				if (toAdd.value.CompareTo(cur.value) < 0)
					cur = cur.left;
				else
					cur = cur.right;
			}
			toAdd.parent = parent;

			if (isNil(parent))
				root = toAdd;
			else if (toAdd.value.CompareTo(parent.value) < 0)
				parent.left = toAdd;
			else
				parent.right = toAdd;

			toAdd.left = nil;
			toAdd.right = nil;
			toAdd.color = Node.RED;

			FixInsert(toAdd);

			count++;
		}

		private void FixInsert(Node inserted)
		{
			Node cousin = nil;
			while (inserted.parent.color == Node.RED)
			{
				if (inserted.parent == inserted.parent.parent.left)
				{
					cousin = inserted.parent.parent.right;

					if (cousin.color == Node.RED)
					{
						inserted.parent.color = Node.BLACK;
						cousin.color = Node.BLACK;
						inserted.parent.parent.color = Node.RED;
						inserted = inserted.parent.parent;
					}
					else if (inserted == inserted.parent.right)
					{

						// leftRotaet around z's parent
						inserted = inserted.parent;
						RotateLeft(inserted);
					}
					else
					{
						inserted.parent.color = Node.BLACK;
						inserted.parent.parent.color = Node.RED;
						RotateRight(inserted.parent.parent);
					}
				}
				else
				{
					cousin = inserted.parent.parent.left;

					if (cousin.color == Node.RED)
					{
						inserted.parent.color = Node.BLACK;
						cousin.color = Node.BLACK;
						inserted.parent.parent.color = Node.RED;
						inserted = inserted.parent.parent;
					}
					else if (inserted == inserted.parent.left)
					{
						// rightRotate around z's parent
						inserted = inserted.parent;
						RotateRight(inserted);
					}
					else
					{
						// recolor and rotate around z's grandpa
						inserted.parent.color = Node.BLACK;
						inserted.parent.parent.color = Node.RED;
						RotateLeft(inserted.parent.parent);
					}
				}
			}
			root.color = Node.BLACK;
		}

		private Node MinNode(Node node)
		{
			while (!isNil(node.left))
				node = node.left;
			return node;
		}

		private Node Successor(Node x)
		{
			if (!isNil(x.left))
				return MinNode(x.right);

			Node y = x.parent;
			while (!isNil(y) && x == y.right)
			{
				x = y;
				y = y.parent;
			}
			return y;
		}

		public bool Remove(T value)
		{
			Node node = search(value);
			if (node != null)
			{
				Remove(node);
				count--;
				return true;
			}
			return false;
		}

		private void Remove(Node v)
		{
			Node z = v;

			Node x = nil;
			Node y = nil;

			if (isNil(z.left) || isNil(z.right))
				y = z;
			else
				y = Successor(z);

			if (!isNil(y.left))
				x = y.left;
			else
				x = y.right;

			x.parent = y.parent;

			if (isNil(y.parent))
				root = x;
			else if (!isNil(y.parent.left) && y.parent.left == y)
				y.parent.left = x;
			else if (!isNil(y.parent.right) && y.parent.right == y)
				y.parent.right = x;

			if (y != z)
				z.value = y.value;

			if (y.color == Node.BLACK)
				FixRemove(x);
		}

		private void FixRemove(Node cur)
		{
			Node sibling;
			while (cur != root && cur.color == Node.BLACK)
			{
				if (cur == cur.parent.left)
				{
					sibling = cur.parent.right;

					if (sibling.color == Node.RED)
					{
						sibling.color = Node.BLACK;
						cur.parent.color = Node.RED;
						RotateLeft(cur.parent);
						sibling = cur.parent.right;
					}

					if (sibling.left.color == Node.BLACK && sibling.right.color == Node.BLACK)
					{
						sibling.color = Node.RED;
						cur = cur.parent;
					}
					else
					{
						if (sibling.right.color == Node.BLACK)
						{
							sibling.left.color = Node.BLACK;
							sibling.color = Node.RED;
							RotateRight(sibling);
							sibling = cur.parent.right;
						}
						sibling.color = cur.parent.color;
						cur.parent.color = Node.BLACK;
						sibling.right.color = Node.BLACK;
						RotateLeft(cur.parent);
						cur = root;
					}
				}
				else
				{
					sibling = cur.parent.left;

					if (sibling.color == Node.RED)
					{
						sibling.color = Node.BLACK;
						cur.parent.color = Node.RED;
						RotateRight(cur.parent);
						sibling = cur.parent.left;
					}

					if (sibling.right.color == Node.BLACK && sibling.left.color == Node.BLACK)
					{
						sibling.color = Node.RED;
						cur = cur.parent;
					}
					else
					{
						if (sibling.left.color == Node.BLACK)
						{
							sibling.right.color = Node.BLACK;
							sibling.color = Node.RED;
							RotateLeft(sibling);
							sibling = cur.parent.left;
						}

						sibling.color = cur.parent.color;
						cur.parent.color = Node.BLACK;
						sibling.left.color = Node.BLACK;
						RotateRight(cur.parent);
						cur = root;
					}
				}
			}
			cur.color = Node.BLACK;
		}

		public bool Contains(T value)
		{
			return search(value) != null;
		}

		private Node search(T key)
		{
			Node current = root;

			int comp;
			while (!isNil(current))
			{
				comp = current.value.CompareTo(key);
				if (comp == 0)
					return current;
				else if (comp < 0)
					current = current.right;
				else
					current = current.left;
			}
			return null;
		}

		private bool isNil(Node node)
		{
			return node == nil;
		}

		public T Min()
		{
			Node cur = root;
			if (isNil(cur))
				return default(T);

			while (!isNil(cur))
				cur = cur.left;
			return cur.left.value;
		}

		public T Max()
		{
			Node cur = root;
			if (isNil(cur))
            {
				return default(T);
			}

			while (!isNil(cur))
            {
				cur = cur.right;
			}
			return cur.right.value;
		}

		public void Clear()
		{
			Stack<Node> stack = new Stack<Node>();

			if (!isNil(root))
            {
				stack.Push(root);
			}

			while (stack.Count > 0)
			{
				Node cur = stack.Pop();
				if (!isNil(cur.left))
                {
					stack.Push(cur);
				}
				if (!isNil(cur.right))
                {
					stack.Push(cur.right);
				}
				cur.parent = cur.left = cur.right = null;
			}
		}

		public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
		{
			if (root != null)
				ToCollection(root, ref collection);
		}

		private void ToCollection(Node cur, ref System.Collections.Generic.ICollection<T> collection)
		{
			if (!isNil(cur))
			{
				ToCollection(cur.left, ref collection);
				collection.Add(cur.value);
				ToCollection(cur.right, ref collection);
			}
		}

		public void GreaterThan(T value, ref System.Collections.Generic.ICollection<T> collection)
		{
			GreaterThan(root, value, ref collection);
		}

		private void GreaterThan(Node node, T value, ref System.Collections.Generic.ICollection<T> collection)
		{
			if (!isNil(node))
			{
				if (node.value.CompareTo(value) > 0)
				{
					GreaterThan(node.left, value, ref collection);
					collection.Add(node.value);
					GreaterThan(node.right, value, ref collection);
				}
				else
				{
					GreaterThan(node.right, value, ref collection);
				}
			}
		}

		public void LessThan(T value, ref System.Collections.Generic.ICollection<T> collection)
		{
			LessThan(root, value, ref collection);
		}

		private void LessThan(Node node, T value, ref System.Collections.Generic.ICollection<T> collection)
		{
			if (!isNil(node))
			{
				if (node.value.CompareTo(value) < 0)
				{
					LessThan(node.left, value, ref collection);
					collection.Add(node.value);
					LessThan(node.right, value, ref collection);
				}
				else
				{
					LessThan(node.right, value, ref collection);
				}
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
			if (isNil(cur))
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
					if (!isNil(cur))
					{
						next.AppendBack(cur.left);
						next.AppendBack(cur.right);

						if (!cont && (!isNil(cur.left) || !isNil(cur.right)))
							cont = true;

						str += (cur.color == Node.BLACK ? 'B' : 'R');
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
