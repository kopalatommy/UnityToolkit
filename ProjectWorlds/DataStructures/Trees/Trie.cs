using ProjectWorlds.DataStructures.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.DataStructures.Trees
{
    public class Trie
    {
        internal class Node
        {
            public bool isValue = false;
            public Dictionary<char, Node> childNodes = null;

            public Node(bool isValue)
            {
                this.isValue = isValue;
                childNodes = new Dictionary<char, Node>();
            }
        }

        public int Count { get { return count; } }

        private Node root = null;
        private int count = 0;

        public Trie()
        {
            count = 0;
            root = new Node(false);
        }

        public bool Insert(string key)
        {
            if (key == null || key.Length == 0)
                return false;

            Node cur = root;
            int len = key.Length;
            for (int i = 0; i < len; i++)
            {
                if (!cur.childNodes.ContainsKey(key[i]))
                    cur.childNodes[key[i]] = new Node(false);
                cur = cur.childNodes[key[i]];
            }

            if (!cur.isValue)
            {
                cur.isValue = true;
                count++;
                return true;
            }
            return false;
        }

        public bool Contains(string key)
        {
            if (key == null || key.Length == 0)
                return false;

            Node cur = root;
            int len = key.Length;
            for (int i = 0; i < len; i++)
            {
                if (!cur.childNodes.ContainsKey(key[i]))
                    return false;
                cur = cur.childNodes[key[i]];
            }

            return cur.isValue;
        }

        public bool Remove(string key)
        {
            if (key == null || key.Length == 0)
                return false;

            Node cur = root;
            int len = key.Length;
            ArrayList<Node> path = new ArrayList<Node>(len);
            for (int i = 0; i < len; i++)
            {
                if (!cur.childNodes.ContainsKey(key[i]))
                    return false;
                path.Add(cur);
                cur = cur.childNodes[key[i]];
            }
            path.Add(cur);
            cur.isValue = false;

            // If the last node is a leaf, it needs to be removed
            if (cur.childNodes.Count == 0)
            {
                // Iterate throught the path to find all nodes only used by this key
                for (int i = len - 1; i >= 0; i--)
                {
                    // If there is more than 1 child node, then do not remove this node b/c it used by more than the key being removed
                    if (path[i].childNodes.Count > 1)
                    {
                        // Drop last node only used by key from Trie
                        path[i + 1].childNodes.Remove(key[i + 1]);
                        break;
                    }
                }
            }
            count--;
            return true;
        }

        public void GetLower(string key, ref ICollection<string> dest)
        {
            if (key == null)
                key = String.Empty;

            Node cur = root;
            int len = key.Length;
            for (int i = 0; i < len; i++)
                if (!cur.childNodes.ContainsKey(key[i]))
                    return;
                else
                    cur = cur.childNodes[key[i]];

            GetLower(cur, ref dest, key);
        }

        private void GetLower(Node cur, ref ICollection<string> dest, string str)
        {
            if (cur.isValue)
                dest.Add(str);
            foreach (KeyValuePair<char, Node> val in cur.childNodes)
            {
                GetLower(val.Value, ref dest, str + val.Key);
            }
        }

        public void Clear()
        {
            count = 0;
            root.childNodes.Clear();
        }
    }
}
