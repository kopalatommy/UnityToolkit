using ProjectWorlds.DataStructures.Lists;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/Nition/UnityOctree

namespace ProjectWorlds.DataStructures.Trees
{
    public class Octree<T>
    {
        internal class Node
        {
            // Returns the center of the node
            public Vector3 Center
            {
                get
                {
                    return center;
                }
            }
            // Returns the length of the node
            public float BaseLength
            {
                get
                {
                    return baseLength;
                }
            }

            // Gets the bounds for the node
            Bounds Bounds
            {
                get
                {
                    return bounds;
                }
            }

            // Returns true if this node has children nodes
            bool HasChildren { get { return children != null; } }

            public bool HasAnyObjects
            {
                get
                {
                    if (objects.Count > 0)
                    {
                        return true;
                    }

                    if (children != null)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (children[i].HasAnyObjects)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }

            // Center of the node
            private Vector3 center;
            // Length of node if it has a looseness of 1.0f
            private float baseLength;

            // 
            private float looseness;
            // Minimum size for this node
            private float minSize;
            // True length of sides of node
            private float adjLength;

            // Bounding box for this node
            private Bounds bounds;

            struct TreeObject
            {
                public T value;
                public Bounds bounds;
            }

            readonly ArrayList<TreeObject> objects = new ArrayList<TreeObject>();

            // Array of children nodes
            Node[] children = null;
            // Bounds of the potential children nodes
            Bounds[] childBounds = null;

            // Max number of children allowed in the node before splitting
            const int MAX_OBJECTS_IN_NODE = 8;

            public Node(float baseLength, float minSize, float looseness, Vector3 center)
            {
                SetValues(baseLength, minSize, looseness, center);
            }

            /// <summary>
            /// Set values used by this class. Child bounds, bounds, size
            /// </summary>
            /// <param name="baseLength"></param>
            /// <param name="minSize"></param>
            /// <param name="looseness"></param>
            /// <param name="center"></param>
            private void SetValues(float baseLength, float minSize, float looseness, Vector3 center)
            {
                this.baseLength = baseLength;
                this.minSize = minSize;
                this.looseness = looseness;
                this.center = center;
                adjLength = baseLength * looseness;

                // Create the bounding box for the node
                bounds = new Bounds(center, new Vector3(adjLength, adjLength, adjLength));

                float quarter = baseLength / 4f;
                float actualChildLength = (BaseLength / 2) * looseness;
                Vector3 actualChildSize = new Vector3(actualChildLength, actualChildLength, actualChildLength);

                childBounds = new Bounds[8];
                childBounds[0] = new Bounds(Center + new Vector3(-quarter, quarter, -quarter), actualChildSize);
                childBounds[1] = new Bounds(Center + new Vector3(quarter, quarter, -quarter), actualChildSize);
                childBounds[2] = new Bounds(Center + new Vector3(-quarter, quarter, quarter), actualChildSize);
                childBounds[3] = new Bounds(Center + new Vector3(quarter, quarter, quarter), actualChildSize);
                childBounds[4] = new Bounds(Center + new Vector3(-quarter, -quarter, -quarter), actualChildSize);
                childBounds[5] = new Bounds(Center + new Vector3(quarter, -quarter, -quarter), actualChildSize);
                childBounds[6] = new Bounds(Center + new Vector3(-quarter, -quarter, quarter), actualChildSize);
                childBounds[7] = new Bounds(Center + new Vector3(quarter, -quarter, quarter), actualChildSize);
            }

            public void SetChildren(Node[] childNodes)
            {
                if (childNodes.Length != 8)
                {
                    return;
                }
                children = childNodes;
            }

            // Returns true if the object bounds falls within the 
            // bounds of the node
            public bool Add(T value, Bounds bounds)
            {
                if (!Encapsulates(this.bounds, bounds))
                {
                    return false;
                }
                else
                {
                    AddInternal(value, bounds);
                    return true;
                }
            }

            private void AddInternal(T value, Bounds bounds)
            {
                int bestFit;
                if (!HasChildren)
                {
                    // Can just add the object if the node is not full
                    if (objects.Count < MAX_OBJECTS_IN_NODE || (baseLength / 2) < minSize)
                    {
                        objects.Add(new TreeObject() { value = value, bounds = bounds });
                        return;
                    }

                    if (children == null)
                    {
                        Split();

                        // Check if the new item fits in one of the
                        // children nodes
                        for (int i = objects.Count - 1; i >= 0; i--)
                        {
                            TreeObject obj = objects[i];

                            bestFit = DetermineBestFit(obj.bounds.center);

                            // If it fits, then add the item to the node
                            if (Encapsulates(children[bestFit].bounds, obj.bounds))
                            {
                                children[bestFit].AddInternal(obj.value, obj.bounds);
                                objects.Remove(obj);
                            }
                        }
                    }
                }

                bestFit = DetermineBestFit(bounds.center);
                if (Encapsulates(children[bestFit].bounds, bounds))
                {
                    children[bestFit].AddInternal(value, bounds);
                }
                else
                {
                    objects.Add(new TreeObject()
                    {
                        value = value,
                        bounds = bounds
                    });
                }
            }

            private bool Encapsulates(Bounds outer, Bounds inner)
            {
                return outer.Contains(inner.min) && outer.Contains(inner.max);
            }

            /// <summary>
            /// Splits the node into eight sub sections
            /// </summary>
            private void Split()
            {
                float quarter = baseLength / 4f;
                float newLength = baseLength / 2f;

                children = new Node[8];
                children[0] = new Node(newLength, minSize, looseness, Center + new Vector3(-quarter, quarter, -quarter));
                children[1] = new Node(newLength, minSize, looseness, Center + new Vector3(quarter, quarter, -quarter));
                children[2] = new Node(newLength, minSize, looseness, Center + new Vector3(-quarter, quarter, quarter));
                children[3] = new Node(newLength, minSize, looseness, Center + new Vector3(quarter, quarter, quarter));
                children[4] = new Node(newLength, minSize, looseness, Center + new Vector3(-quarter, -quarter, -quarter));
                children[5] = new Node(newLength, minSize, looseness, Center + new Vector3(quarter, -quarter, -quarter));
                children[6] = new Node(newLength, minSize, looseness, Center + new Vector3(-quarter, -quarter, quarter));
                children[7] = new Node(newLength, minSize, looseness, Center + new Vector3(quarter, -quarter, quarter));
            }

            /// <summary>
            /// Determines which child node to place an object in
            /// </summary>
            /// <param name="boundsCenter">New object bounds</param>
            /// <returns>Index of one of the eight child nodes</returns>
            public int DetermineBestFit(Vector3 boundsCenter)
            {
                return (boundsCenter.x <= Center.x ? 0 : 1) + (boundsCenter.y >= Center.y ? 0 : 4) + (boundsCenter.z <= Center.z ? 0 : 2);
            }

            public bool Remove(T value)
            {
                bool removed = false;

                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].value.Equals(value))
                    {
                        removed = objects.Remove(objects[i]);
                        break;
                    }
                }

                if (!removed && children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        removed = children[i].Remove(value);
                        if (removed)
                        {
                            break;
                        }
                    }
                }

                if (removed && children != null)
                {
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                }
                return removed;
            }

            public bool Remove(T value, Bounds valueBounds)
            {
                if (Encapsulates(bounds, valueBounds))
                {
                    return RemoveInternal(value, valueBounds);
                }
                else
                {
                    return false;
                }
            }

            private bool RemoveInternal(T value, Bounds valueBounds)
            {
                bool removed = false;

                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].value.Equals(value))
                    {
                        removed = objects.Remove(objects[i]);
                        break;
                    }
                }

                if (!removed && children != null)
                {
                    int bestFit = DetermineBestFit(valueBounds.center);
                    removed = children[bestFit].RemoveInternal(value, valueBounds);
                }

                if (removed && children != null)
                {
                    if (ShouldMerge())
                    {
                        Merge();
                    }
                }
                return removed;
            }

            private bool ShouldMerge()
            {
                int totalObjects = objects.Count;
                if (children != null)
                {
                    foreach (Node node in children)
                    {
                        // If the child has children, then there are too
                        // many child nodes to merge this node
                        if (node.children != null)
                        {
                            return false;
                        }
                        totalObjects += node.objects.Count;
                    }
                }
                return totalObjects <= MAX_OBJECTS_IN_NODE;
            }

            private void Merge()
            {
                for (int i = 0; i < 8; i++)
                {
                    Node cur = children[i];
                    int numObjects = cur.objects.Count;
                    for (int j = numObjects - 1; j >= 0; j--)
                    {
                        objects.Add(cur.objects[j]);
                    }
                }
                children = null;
            }

            public bool IsColliding(Bounds toCheck)
            {
                // Does toCheck intersect with this node
                if (!bounds.Intersects(toCheck))
                {
                    return false;
                }

                foreach (TreeObject cur in objects)
                {
                    if (cur.bounds.Intersects(toCheck))
                    {
                        return true;
                    }
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i].IsColliding(toCheck))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool IsColliding(Ray toCheck, float maxDistance = float.MaxValue)
            {
                float distance;

                // Does the ray intersect with the node
                if (!bounds.IntersectRay(toCheck, out distance) || distance > maxDistance)
                {
                    return false;
                }

                foreach (TreeObject cur in objects)
                {
                    if (cur.bounds.IntersectRay(toCheck, out distance) && distance <= maxDistance)
                    {
                        return true;
                    }
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i].IsColliding(toCheck, maxDistance))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool GetColliding(Bounds toCheck, IList<T> result)
            {
                // Does this node intersect with toCheck
                if (!bounds.Intersects(toCheck))
                {
                    return false;
                }

                foreach (TreeObject cur in objects)
                {
                    if (cur.bounds.Intersects(toCheck))
                    {
                        result.Add(cur.value);
                    }
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        children[i].GetColliding(toCheck, result);
                    }
                }

                return result.Count > 0;
            }

            public bool GetColliding(Ray toCheck, IList<T> result, float maxDistance = float.MaxValue)
            {
                float distance;

                // Check if the node intersects with the ray
                if (!bounds.IntersectRay(toCheck, out distance) || distance > maxDistance)
                {
                    return false;
                }

                foreach (TreeObject cur in objects)
                {
                    if (cur.bounds.IntersectRay(toCheck, out distance) || distance <= maxDistance)
                    {
                        result.Add(cur.value);
                    }
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        children[i].GetColliding(toCheck, result, maxDistance);
                    }
                }
                return result.Count > 0;
            }

            public Node ShrinkIfPossible(float minLength)
            {
                if (baseLength < (2 * minLength))
                {
                    return this;
                }
                else if (objects.Count == 0 && (children == null))
                {
                    return this;
                }

                int bestFit = -1;
                for (int i = 0; i < objects.Count; i++)
                {
                    TreeObject cur = objects[i];
                    int curBestFit = DetermineBestFit(cur.bounds.center);
                    if (i == 0 || curBestFit == bestFit)
                    {
                        if (Encapsulates(childBounds[curBestFit], cur.bounds))
                        {
                            if (bestFit < 0)
                            {
                                bestFit = curBestFit;
                            }
                        }
                        else
                        {
                            return this;
                        }
                    }
                    else
                    {
                        return this;
                    }
                }
                return this;
            }

            public bool Contains(T value)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].value.Equals(value))
                    {
                        return true;
                    }
                }

                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i].Contains(value))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool Contains(T value, Bounds valueBounds)
            {
                if (Encapsulates(bounds, valueBounds))
                {
                    return ContainsInternal(value, valueBounds);
                }
                else
                {
                    return false;
                }
            }

            private bool ContainsInternal(T value, Bounds valueBounds)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].value.Equals(value))
                    {
                        return true;
                    }
                }

                if (children != null)
                {
                    int bestFit = DetermineBestFit(valueBounds.center);
                    if (children[bestFit].ContainsInternal(value, valueBounds))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int Count { get { return count; } }

        private int count = 0;

        // Value between 1 and 2. Increased the base side of a node
        float looseness = 1;

        // Initial size of the octree 
        float initialSize;

        // Min size a node length can be
        float minSize;

        private Node root = null;

        public Octree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize, float looseness)
        {
            if (minNodeSize > initialWorldSize)
            {
                minNodeSize = initialWorldSize;
            }

            initialSize = initialWorldSize;
            minSize = minNodeSize;
            this.looseness = Mathf.Clamp(looseness, 1f, 2f);
            root = new Node(initialSize, minSize, this.looseness, initialWorldPos);
        }

        public bool Add(T obj, Bounds bounds)
        {
            int count = 0;
            while (!root.Add(obj, bounds))
            {
                Grow(bounds.center - root.Center);
                if (count++ > 20)
                {
                    return false;
                }
            }
            this.count++;
            return true;
        }

        public bool Remove(T value)
        {
            bool removed = root.Remove(value);

            if (removed)
            {
                count--;
                Shrink();
            }
            return removed;
        }

        public bool Remove(T value, Bounds bounds)
        {
            bool removed = root.Remove(value, bounds);

            if (removed)
            {
                count--;
                Shrink();
            }
            return removed;
        }

        private void Grow(Vector3 direction)
        {
            int xDirection = direction.x >= 0 ? 1 : -1;
            int yDirection = direction.y >= 0 ? 1 : -1;
            int zDirection = direction.z >= 0 ? 1 : -1;

            Node oldRoot = root;

            float half = root.BaseLength / 2;
            float newLength = root.BaseLength * 2;
            Vector3 newCenter = root.Center + new Vector3(xDirection * half, yDirection * half, zDirection * half);

            root = new Node(newLength, minSize, looseness, newCenter);

            if (oldRoot.HasAnyObjects)
            {
                int rootPos = root.DetermineBestFit(oldRoot.Center);
                Node[] children = new Node[8];
                for (int i = 0; i < 8; i++)
                {
                    if (i == rootPos)
                    {
                        children[i] = oldRoot;
                    }
                    else
                    {
                        xDirection = i % 2 == 0 ? -1 : 1;
                        yDirection = i > 3 ? -1 : 1;
                        zDirection = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;
                        children[i] = new Node(oldRoot.BaseLength, minSize, looseness, newCenter + new Vector3(xDirection * half, yDirection * half, zDirection * half));
                    }
                }
                root.SetChildren(children);
            }
        }

        private void Shrink()
        {
            root = root.ShrinkIfPossible(initialSize);
        }

        public bool IsColliding(Bounds toCheck)
        {
            return root.IsColliding(toCheck);
        }

        public bool IsColliding(Ray toCheck, float maxDistance = float.MaxValue)
        {
            return root.IsColliding(toCheck, maxDistance);
        }

        public bool GetColliding(Bounds toCheck, IList<T> result)
        {
            return root.GetColliding(toCheck, result);
        }

        public bool GetColliding(Ray toCheck, IList<T> result, float maxDistance = float.MaxValue)
        {
            return GetColliding(toCheck, result, maxDistance);
        }

        public bool Contains(T value)
        {
            return root.Contains(value);
        }

        public bool Contains(T value, Bounds bounds)
        {
            return root.Contains(value, bounds);
        }
    }
}
