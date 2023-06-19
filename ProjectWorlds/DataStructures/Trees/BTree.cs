using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    public class BTree<T> : ITree<T> where T : IComparable
    {
        internal class Node
        {
            public int degree; // Determines the number of items contained in the node (degree to (degree * 2 - 1))
            public T[] items; // Items stored in the node
            public Node[] children; // Children nodes
            public int numItems; // Number of items held by this node
            public bool isLeaf; // Whether this node is a leaf (contains no children)

            public Node(int degree, bool leaf)
            {
                // Copy the minimum degree and leaf property
                this.degree = degree;
                this.isLeaf = leaf;

                // Allocate memory for the maximum number of possible keys and child pointers
                items = new T[2 * degree - 1];
                children = new Node[2 * degree];

                // Initialize the number of keys as 0
                numItems = 0;
            }

            // This will insert a new item into the subtree rooted within this node.
            // The assumption is the node must be non-full when this function is called.
            public void insertNonFull(T value)
            {
                // Initialize index as index of rightmost element 
                int i = numItems - 1;

                // If this is a leaf node 
                if (isLeaf == true)
                {
                    // The following loop does two things 
                    // a) Finds the location of new item to be inserted 
                    // b) Moves all greater items to one place ahead 
                    while (i >= 0 && items[i].CompareTo(value) > 0)
                    {
                        items[i + 1] = items[i];
                        i--;
                    }

                    // Insert the new item at found location 
                    items[i + 1] = value;
                    numItems = numItems + 1;
                }
                else // If this node is not leaf 
                {
                    // Find the child which is going to have the new item 
                    while (i >= 0 && items[i].CompareTo(value) > 0)
                        i--;

                    // See if the found child is full 
                    if (children[i + 1].numItems == 2 * degree - 1)
                    {
                        // If the child is full, then split it 
                        splitChild(i + 1, children[i + 1]);

                        // After split, the middle item of C[i] goes up and 
                        // C[i] is splitted into two. See which of the two 
                        // is going to have the new item 
                        if (items[i + 1].CompareTo(value) < 0)
                            i++;
                    }
                    children[i + 1].insertNonFull(value);
                }
            }

            // This will split the child y of this node. i is the index of y in the child array
            // children[]. The child y must be full when this function is called.
            public void splitChild(int i, Node y)
            {
                // Create a new node which is going to store (t-1) items 
                // of y 
                Node z = new Node(y.degree, y.isLeaf);
                z.numItems = degree - 1;

                // Copy the last (t-1) items of y to z 
                for (int j = 0; j < degree - 1; j++)
                {
                    z.items[j] = y.items[j + degree];
                }

                // Copy the last t children of y to z 
                if (y.isLeaf == false)
                {
                    for (int j = 0; j < degree; j++)
                    {
                        z.children[j] = y.children[j + degree];
                    }
                }

                // Reduce the number of items in y 
                y.numItems = degree - 1;

                // Since this node is going to have a new child, 
                // create space of new child 
                for (int j = numItems; j >= i + 1; j--)
                {
                    children[j + 1] = children[j];
                }

                // Link the new child to this node 
                children[i + 1] = z;

                // A key of y will move to this node. Find the location of 
                // new key and move all greater keys one space ahead 
                for (int j = numItems - 1; j >= i; j--)
                    items[j + 1] = items[j];

                // Copy the middle key of y to this node 
                items[i] = y.items[degree - 1];

                // Increment count of keys in this node 
                numItems = numItems + 1;
            }

            // A wrapper function to remove the key k in subtree rooted with 
            // this node. 
            public bool remove(T key)
            {
                int idx = findKey(key);

                // The key to be removed is present in this node 
                if (idx < numItems && items[idx].CompareTo(key) == 0)
                {
                    // If the node is a leaf node - removeFromLeaf is called 
                    // Otherwise, removeFromNonLeaf function is called 
                    if (isLeaf)
                    {
                        removeFromLeaf(idx);
                        return true;
                    }
                    else
                        return removeFromNonLeaf(idx);
                }
                else
                {

                    // If this node is a leaf node, then the key is not present in tree 
                    if (isLeaf)
                    {
                        Console.WriteLine("The key {0} is does not exist in the tree", key);
                        return false;
                    }

                    // The key to be removed is present in the sub-tree rooted with this node 
                    // The flag indicates whether the key is present in the sub-tree rooted 
                    // with the last child of this node 
                    bool flag = ((idx == numItems) ? true : false);

                    // If the child where the key is supposed to exist has less that t keys, 
                    // we fill that child 
                    if (children[idx].numItems < degree)
                        fill(idx);

                    // If the last child has been merged, it must have merged with the previous 
                    // child and so we recurse on the (idx-1)th child. Else, we recurse on the 
                    // (idx)th child which now has atleast t keys 
                    if (flag && idx > numItems)
                    {
                        return children[idx - 1].remove(key);
                    }
                    else
                    {
                        return children[idx].remove(key);
                    }
                }
            }

            // A function to remove the key present in idx-th position in 
            // this node which is a leaf 
            public void removeFromLeaf(int idx)
            {
                // Move all the keys after the idx-th pos one place backward 
                for (int i = idx + 1; i < numItems; ++i)
                {
                    items[i - 1] = items[i];
                }

                // Reduce the count of keys 
                numItems--;
            }

            // A function to remove the key present in idx-th position in 
            // this node which is a non-leaf node 
            public bool removeFromNonLeaf(int idx)
            {
                T key = items[idx];

                // If the child that precedes k (C[idx]) has atleast t keys, 
                // find the predecessor 'pred' of k in the subtree rooted at 
                // C[idx]. Replace k by pred. Recursively delete pred 
                // in C[idx] 
                if (children[idx].numItems >= degree)
                {
                    T pred = getPred(idx);
                    items[idx] = pred;
                    return children[idx].remove(pred);
                }

                // If the child C[idx] has less that t keys, examine C[idx+1]. 
                // If C[idx+1] has atleast t keys, find the successor 'succ' of k in 
                // the subtree rooted at C[idx+1] 
                // Replace k by succ 
                // Recursively delete succ in C[idx+1] 
                else if (children[idx + 1].numItems >= degree)
                {
                    T succ = getSucc(idx);
                    items[idx] = succ;
                    return children[idx + 1].remove(succ);
                }

                // If both C[idx] and C[idx+1] has less that t keys,merge k and all of C[idx+1] 
                // into C[idx] 
                // Now C[idx] contains 2t-1 keys 
                // Free C[idx+1] and recursively delete k from C[idx] 
                else
                {
                    merge(idx);
                    return children[idx].remove(key);
                }
            }

            // A function to get the predecessor of the key- where the key 
            // is present in the idx-th position in the node 
            public T getPred(int idx)
            {
                // Keep moving to the right most node until we reach a leaf 
                Node cur = children[idx];
                while (!cur.isLeaf)
                {
                    cur = cur.children[cur.numItems];
                }

                // Return the last key of the leaf 
                return cur.items[cur.numItems - 1];
            }

            // A function to get the successor of the key- where the key 
            // is present in the idx-th position in the node 
            public T getSucc(int idx)
            {
                // Keep moving the left most node starting from C[idx+1] until we reach a leaf 
                Node cur = children[idx + 1];
                while (!cur.isLeaf)
                {
                    cur = cur.children[0];
                }

                // Return the first key of the leaf 
                return cur.items[0];
            }

            // A function to fill up the child node present in the idx-th 
            // position in the C[] array if that child has less than t-1 keys 
            public void fill(int idx)
            {
                // If the previous child(C[idx-1]) has more than t-1 keys, borrow a key 
                // from that child 
                if (idx != 0 && children[idx - 1].numItems >= degree)
                {
                    borrowFromPrev(idx);
                }

                // If the next child(C[idx+1]) has more than t-1 keys, borrow a key 
                // from that child 
                else if (idx != numItems && children[idx + 1].numItems >= degree)
                {
                    borrowFromNext(idx);
                }

                // Merge C[idx] with its sibling 
                // If C[idx] is the last child, merge it with with its previous sibling 
                // Otherwise merge it with its next sibling 
                else
                {
                    if (idx != numItems)
                    {
                        merge(idx);
                    }
                    else
                    {
                        merge(idx - 1);
                    }
                }
            }

            // A function to borrow a key from the C[idx-1]-th node and place 
            // it in C[idx]th node 
            public void borrowFromPrev(int idx)
            {
                Node child = children[idx];
                Node sibling = children[idx - 1];

                // The last key from C[idx-1] goes up to the parent and key[idx-1] 
                // from parent is inserted as the first key in C[idx]. Thus, the loses 
                // sibling one key and child gains one key 

                // Moving all key in C[idx] one step ahead 
                for (int i = child.numItems - 1; i >= 0; --i)
                {
                    child.items[i + 1] = child.items[i];
                }

                // If C[idx] is not a leaf, move all its child pointers one step ahead 
                if (!child.isLeaf)
                {
                    for (int i = child.numItems; i >= 0; --i)
                        child.children[i + 1] = child.children[i];
                }

                // Setting child's first key equal to keys[idx-1] from the current node 
                child.items[0] = items[idx - 1];

                // Moving sibling's last child as C[idx]'s first child 
                if (!child.isLeaf)
                {
                    child.children[0] = sibling.children[sibling.numItems];
                }

                // Moving the key from the sibling to the parent 
                // This reduces the number of keys in the sibling 
                items[idx - 1] = sibling.items[sibling.numItems - 1];

                child.numItems += 1;
                sibling.numItems -= 1;
            }

            // A function to borrow a key from the C[idx+1]-th node and place it 
            // in C[idx]th node 
            public void borrowFromNext(int idx)
            {
                Node child = children[idx];
                Node sibling = children[idx + 1];

                // keys[idx] is inserted as the last key in C[idx] 
                child.items[(child.numItems)] = items[idx];

                // Sibling's first child is inserted as the last child 
                // into C[idx] 
                if (!child.isLeaf)
                {
                    child.children[(child.numItems) + 1] = sibling.children[0];
                }

                //The first key from sibling is inserted into keys[idx] 
                items[idx] = sibling.items[0];

                // Moving all keys in sibling one step behind 
                for (int i = 1; i < sibling.numItems; ++i)
                {
                    sibling.items[i - 1] = sibling.items[i];
                }

                // Moving the child pointers one step behind 
                if (!sibling.isLeaf)
                {
                    for (int i = 1; i <= sibling.numItems; ++i)
                        sibling.children[i - 1] = sibling.children[i];
                }

                // Increasing and decreasing the key count of C[idx] and C[idx+1] 
                // respectively 
                child.numItems += 1;
                sibling.numItems -= 1;
            }

            // A function to merge idx-th child of the node with (idx+1)th child of 
            // the node 
            public void merge(int idx)
            {
                Node child = children[idx];
                Node sibling = children[idx + 1];

                // Pulling a key from the current node and inserting it into (t-1)th 
                // position of C[idx] 
                child.items[degree - 1] = items[idx];

                // Copying the keys from C[idx+1] to C[idx] at the end 
                for (int i = 0; i < sibling.numItems; ++i)
                {
                    child.items[i + degree] = sibling.items[i];
                }

                // Copying the child pointers from C[idx+1] to C[idx] 
                if (!child.isLeaf)
                {
                    for (int i = 0; i <= sibling.numItems; ++i)
                    {
                        child.children[i + degree] = sibling.children[i];
                    }
                }

                // Moving all keys after idx in the current node one step before - 
                // to fill the gap created by moving keys[idx] to C[idx] 
                for (int i = idx + 1; i < numItems; ++i)
                {
                    items[i - 1] = items[i];
                }

                // Moving the child pointers after (idx+1) in the current node one 
                // step before 
                for (int i = idx + 2; i <= numItems; ++i)
                {
                    children[i - 1] = children[i];
                }

                // Updating the key count of child and the current node 
                child.numItems += sibling.numItems + 1;
                numItems--;
            }

            // This will traverse all the nodes in a subtree rooted within this node
            public void traverse()
            {
                // There are n keys and n+1 children, traverse through n keys
                // and first n children
                int i;
                for (i = 0; i < numItems; i++)
                {
                    // If this is not a leaf (it has children) then before we print keys[i]
                    // call traverse on the child first
                    if (isLeaf == false)
                    {
                        children[i].traverse();
                    }
                    Console.Write("{0} ", items[i]);
                }

                // Print the subtree rooted with last child
                if (isLeaf == false)
                {
                    children[i].traverse();
                }
            }

            // This will search for a specified key in subtree rooted with this node
            public Node find(T key)
            {
                // Find the first key greater than or equal to k
                int i = 0;
                while (i < numItems && key.CompareTo(items[i]) > 0)
                {
                    i++;
                }

                // If the found key is equal to the target, return this node
                if (i != numItems && items[i].CompareTo(key) == 0)
                {
                    return this;
                }

                // If key is not found here and this is a leaf node
                if (isLeaf == true)
                {
                    // Because if this is a leaf node that means there are no more children to search
                    return null;
                }

                // Go to the appropiate child
                return children[i].find(key);
            }

            // A function that returns the index of the first key that is greater 
            // or equal to k 
            public int findKey(T key)
            {
                int idx = 0;
                while (idx < numItems && items[idx].CompareTo(key) < 0)
                {
                    ++idx;
                }
                return idx;
            }

            public void printKeys()
            {
                for (int i = 0; i < numItems; i++)
                {
                    Console.Write("{0} ", items[i]);
                }
            }

            public void ToCollection(ref System.Collections.Generic.ICollection<T> collection)
            {
                int i;
                for (i = 0; i < numItems; i++)
                {
                    if (!isLeaf)
                        children[i].ToCollection(ref collection);
                    collection.Add(items[i]);
                }

                if (!isLeaf)
                    children[i].ToCollection(ref collection);
            }

            public void Clear()
            {
                if (!isLeaf)
                {
                    for (int i = 0; i <= numItems; i++)
                    {
                        children[i].Clear();
                        children[i] = null;
                    }
                }
                items = null;
                children = null;
            }
        }


        Node root = null;
        int degree = 0;
        int count = 0;

        public int Count { get { return count; } }

        public BTree(int degree)
        {
            // Store the minimum degree
            this.degree = degree;
            // Set the root to null
            root = null;
        }

        public void traverse()
        {
            if (root != null)
            {
                root.traverse();
            }
        }

        public bool Contains(T item)
        {
            if (root == null)
                return false;
            else
            {
                Node temp = root.find(item);
                if (temp != null)
                {
                    for (int i = 0; i < temp.numItems; i++)
                        if (temp.items[i].CompareTo(item) == 0)
                            return true;
                }

                return false;
            }
        }

        public void Insert(T key)
        {
            // If tree is empty 
            if (root == null)
            {
                // Allocate memory for root 
                root = new Node(degree, true);
                root.items[0] = key; // Insert key 
                root.numItems = 1; // Update number of keys in root
            }
            else // If tree is not empty 
            {
                // If root is full, then tree grows in height 
                if (root.numItems == 2 * degree - 1)
                {
                    // Allocate memory for new root 
                    Node s = new Node(degree, false);

                    // Make old root as child of new root 
                    s.children[0] = root;

                    // Split the old root and move 1 key to the new root
                    s.splitChild(0, root);

                    // New root has two children now. Decide which of the 
                    // two children is going to have new key
                    int i = 0;
                    if (s.items[0].CompareTo(key) < 0)
                    {
                        i++;
                    }
                    s.children[i].insertNonFull(key);

                    // Change root
                    root = s;
                }
                else // If root is not full, call insertNonFull for root 
                {
                    root.insertNonFull(key);
                }
            }
            count++;
        }

        public bool Remove(T key)
        {
            if (root == null)
            {
                Console.WriteLine("The tree is empty");
                return false;
            }

            // Call the remove function for root 
            bool ret = root.remove(key);

            // If the root node has 0 keys, make its first child as the new root 
            // if it has a child, otherwise set root as NULL 
            if (root.numItems == 0)
            {
                Node tmp = root;
                if (root.isLeaf)
                {
                    root = null;
                }
                else
                {
                    root = root.children[0];
                }
            }

            if (ret)
                count--;
            return ret;
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
                root.ToCollection(ref collection);
            }
        }

        public void Clear()
        {
            if (root != null)
                root.Clear();

            count = 0;
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
                        toRet += "L";
                    else
                        toRet += "N";
                    toRet += "," + node.numItems;
                    toRet += "<";
                    for (int i = 0; i < node.numItems; i++)
                    {
                        toRet += node.items[i].ToString();
                        if (i + 1 < node.numItems)
                            toRet += ',';
                        if (!node.isLeaf)
                            next.AppendBack(node.children[i]);
                    }
                    next.AppendBack(node.children[node.numItems]);
                    toRet += '>';
                    if (!node.isLeaf)
                        cont = true;
                }
                toRet += '\n';
                layer++;
            }
            return toRet;
        }

        public override string ToString()
        {
            if (root != null)
            {
                return root.ToString() + "\nCount: " + count.ToString();
            }
            else
                return "Count: " + count;
        }

        private string ToStringRecursive(Node cur)
        {
            if (cur == null)
                return "";

            string ret = string.Empty;

            for (int i = 0; i < cur.numItems; i++)
            {
                if (!cur.isLeaf)
                    ret += ToStringRecursive(cur.children[i]);
                ret += " " + cur.items[i];
            }

            return ret + ToStringRecursive(cur.children[cur.numItems - 1]);
        }
    }
}
