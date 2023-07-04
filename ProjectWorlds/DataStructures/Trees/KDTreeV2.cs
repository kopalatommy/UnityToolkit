using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Queues;
using System;

namespace ProjectWorlds.DataStructures.Trees
{
    // Two things to do:
    // A. It should be possible to handle equal locations by 
    // adding in a comp case when comp == 0. Would also have to
    // handle the case of inserting the node in between two others
    // B. It would be interesting to try and have this track items
    // by index like the other version. Would split by mid point in
    // this case. Push all items to leaf nodes.

    public class KDTreeV2<TKey, TValue> where TKey : IComparable<TKey>
    {
        internal class Node
        {
            public bool IsLeaf
            {
                get
                {
                    return leftChild == null && rightChild == null;
                }
            }

            public TKey[] point;
            public TValue value;

            public Node leftChild;
            public Node rightChild;

            public Node()
            {

            }

            public Node(TKey[] point, TValue value)
            {
                this.point = point;
                this.value = value;
            }

            public Node this[int compare]
            {
                get
                {
                    if (compare <= 0)
                    {
                        return leftChild;
                    }
                    else
                    {
                        return rightChild;
                    }
                }
                set
                {
                    if (compare <= 0)
                    {
                        leftChild = value;
                    }
                    else
                    {
                        rightChild = value;
                    }
                }
            }
        }

        internal class HyperRect
        {
            public TKey[] MinPoint
            {
                get
                {
                    return minPoint;
                }
                set
                {
                    minPoint = new TKey[value.Length];
                    value.CopyTo(minPoint, 0);
                }
            }
            public TKey[] MaxPoint
            {
                get
                {
                    return maxPoint;
                }
                set
                {
                    maxPoint = new TKey[value.Length];
                    value.CopyTo(maxPoint, 0);
                }
            }

            private TKey[] minPoint;
            private TKey[] maxPoint;

            public TKey[] ClosestPoint(TKey[] point, IKDTypeMath<TKey> typeMath)
            {
                TKey[] closest = new TKey[point.Length];

                for (int i = 0; i < point.Length; i++)
                {
                    int comp = typeMath.CompareTo(minPoint[i], point[i]);

                    if (comp < 0)
                    {
                        closest[i] = minPoint[i];
                    }
                    else if (comp > 0)
                    {
                        closest[i] = maxPoint[i];
                    }
                    else
                    {
                        closest[i] = point[i];
                    }
                }
                return closest;
            }

            public HyperRect Clone()
            {
                return new HyperRect()
                {
                    minPoint = minPoint,
                    maxPoint = maxPoint
                };
            }

            public static HyperRect Infinite(int numDimensions, IKDTypeMath<TKey> typeMath)
            {
                HyperRect res = new HyperRect()
                {
                    minPoint = new TKey[numDimensions],
                    maxPoint = new TKey[numDimensions]
                };
                for (int i = 0; i < numDimensions; i++)
                {
                    res.minPoint[i] = typeMath.NegateInfinity();
                    res.maxPoint[i] = typeMath.PositiveInfinity();
                }
                return res;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        private int count = 0;
        private int numDimensions = 0;
        private IKDTypeMath<TKey> typeMath = null;
        Node root = null;

        public KDTreeV2(int dimensions, IKDTypeMath<TKey> typeMath)
        {
            this.numDimensions = dimensions;
            this.typeMath = typeMath;
        }

        public bool Add(TKey[] point, TValue value)
        {
            if (point.Length != numDimensions)
            {
                return false;
            }

            return Add(new Node(point, value));
        }

        private bool Add(Node toAdd)
        {
            if (root == null)
            {
                root = toAdd;
            }
            else
            {
                int dimension = -1;
                Node parent = root;

                do
                {
                    dimension = (dimension + 1) % numDimensions;

                    if (typeMath.AreEqual(toAdd.point, parent.point))
                    {
                        parent.value = toAdd.value;
                        break;
                    }
                    int comp = typeMath.CompareTo(toAdd.point[dimension], parent.point[dimension]);

                    if (parent[comp] == null)
                    {
                        parent[comp] = toAdd;
                        break;
                    }
                    else
                    {
                        parent = parent[comp];
                    }
                }
                while (true);
            }
            count++;
            return true;
        }

        public bool TryFindValueAt(TKey[] point, out TValue value)
        {
            Node cur = root;
            int dimension = -1;
            do
            {
                if (cur == null)
                {
                    value = default(TValue);
                    return false;
                }
                else if (typeMath.AreEqual(point, cur.point))
                {
                    value = cur.value;
                    return true;
                }

                dimension = (dimension + 1) % dimension;
                int compare = typeMath.CompareTo(point[dimension], cur.point[dimension]);
                cur = cur[dimension];
            } while (true);
        }

        public TValue FndValueAt(TKey[] point)
        {
            if (TryFindValueAt(point, out TValue value))
            {
                return value;
            }
            else
            {
                return default(TValue);
            }
        }

        public bool TryFindValue(TValue value, out TKey[] point)
        {
            if (root == null)
            {
                point = null;
                return false;
            }

            Queue<Node> searchQueue = new Queue<Node>();
            while (searchQueue.Count > 0)
            {
                Node cur = searchQueue.Dequeue();

                if (cur.value.Equals(value))
                {
                    point = cur.point;
                    return true;
                }
                else
                {
                    Node left = cur[-1];
                    Node right = cur[1];
                    if (left != null)
                    {
                        searchQueue.AppendBack(left);
                    }
                    if (right != null)
                    {
                        searchQueue.AppendBack(right);
                    }
                }
            }
            point = null;
            return false;
        }

        public TKey[] FindValue(TValue value)
        {
            if (TryFindValue(value, out TKey[] point))
            {
                return point;
            }
            else
            {
                return null;
            }
        }

        public OrderedList<TKey, TValue> RadialSearch(TKey[] center, TKey radius, int count)
        {
            OrderedList<TKey, Node> list = new OrderedList<TKey, Node>();
            AddNearestNeighbors(root, center, HyperRect.Infinite(numDimensions, typeMath),
                0, list, typeMath.Multiply(radius, radius), int.MaxValue);

            OrderedList<TKey, TValue> res = new OrderedList<TKey, TValue>();
            foreach (ComparablePair<TKey, Node> pair in list)
            {
                res.AppendBack(pair.key, pair.value.value);
            }

            return res;
        }

        private void AddNearestNeighbors(Node node,
            TKey[] point,
            HyperRect rect,
            int depth,
            OrderedList<TKey, Node> nearest,
            TKey searchRadiusSquared,
            int maxSize
            )
        {
            if (node == null)
            {
                return;
            }

            int dimension = depth % numDimensions;

            HyperRect leftRect = rect.Clone();
            leftRect.MaxPoint[dimension] = node.point[dimension];

            HyperRect rightRect = rect.Clone();
            rightRect.MinPoint[dimension] = node.point[dimension];

            int comp = typeMath.CompareTo(point[dimension], node.point[dimension]);

            // ToDo, might be quicker to do one if else here
            HyperRect nearerRect = comp <= 0 ? leftRect : rightRect;
            HyperRect fartherRect = comp <= 0 ? rightRect : leftRect;

            Node nearerNode = comp <= 0 ? node.leftChild : node.rightChild;
            Node fartherNode = comp <= 0 ? node.rightChild : node.leftChild;

            if (nearerNode != null)
            {
                AddNearestNeighbors(nearerNode,
                    point,
                    nearerRect,
                    depth + 1,
                    nearest,
                    searchRadiusSquared,
                    maxSize);
            }

            TKey squareDist;

            TKey[] closestPointInFurther = fartherRect.ClosestPoint(point, typeMath);
            squareDist = typeMath.SquaredDistance(closestPointInFurther, point);

            if (typeMath.CompareTo(squareDist, searchRadiusSquared) <= 0)
            {
                if (nearest.Count >= maxSize)
                {
                    if (typeMath.CompareTo(squareDist, nearest[0].key) < 0)
                    {
                        AddNearestNeighbors(fartherNode,
                            point,
                            fartherRect,
                            depth + 1,
                            nearest,
                            searchRadiusSquared,
                            maxSize);
                    }
                }
            }
            else
            {
                AddNearestNeighbors(fartherNode,
                            point,
                            fartherRect,
                            depth + 1,
                            nearest,
                            searchRadiusSquared,
                            maxSize);
            }

            squareDist = typeMath.SquaredDistance(node.point, point);

            // If within the search area, try to add the node to the list
            if (typeMath.CompareTo(squareDist, searchRadiusSquared) <= 0)
            {
                // If the list is shorter than the desired quanity, can
                // just add to the list
                if (nearest.Count < maxSize)
                {
                    nearest.AppendBack(squareDist, node);
                }
                // If the list is larger, check if the new dist is less
                // than the current max. Add if new dist is shorter
                else
                {
                    if (typeMath.CompareTo(squareDist, nearest[0].key) < 0)
                    {
                        nearest.RemoveAt(0);
                        nearest.AppendFront(squareDist, node);
                    }
                }
            }
        }

        public bool RemoveAt(TKey[] point)
        {
            if (root == null)
            {
                return false;
            }

            Node cur = root;

            if (typeMath.AreEqual(point, cur.point))
            {
                root = null;
                count--;
                ReinsertChildNodes(cur);
                return true;
            }

            int dimension = -1;

            do
            {
                dimension = (dimension + 1) % numDimensions;

                int comp = typeMath.CompareTo(point[dimension], cur.point[dimension]);

                if (cur[comp] == null)
                {
                    return false;
                }
                if (typeMath.AreEqual(point, cur[comp].point))
                {
                    Node toReturn = cur[comp];
                    cur[comp] = null;
                    count--;
                    ReinsertChildNodes(cur);
                    return true;
                }
                else
                {
                    cur = cur[count];
                }

            } while (cur != null);
            return false;
        }

        private void ReinsertChildNodes(Node removed)
        {
            if (removed.IsLeaf)
            {
                return;
            }

            Queue<Node> toAdd = new Queue<Node>();
            Queue<Node> toCheckQueue = new Queue<Node>();

            if (removed.leftChild != null)
            {
                toCheckQueue.Add(removed.leftChild);
            }
            if (removed.rightChild != null)
            {
                toCheckQueue.Add(removed.rightChild);
            }

            while (toCheckQueue.Count > 0)
            {
                Node cur = toCheckQueue.Dequeue();

                toAdd.Enqueue(cur);

                if (cur.leftChild != null)
                {
                    toCheckQueue.Add(cur.leftChild);
                    cur.leftChild = null;
                }
                if (cur.rightChild != null)
                {
                    toCheckQueue.Add(cur.rightChild);
                    cur.rightChild = null;
                }
            }

            while (toAdd.Count > 0)
            {
                Node cur = toCheckQueue.Dequeue();

                count--;
                Add(cur);
            }
        }

        public OrderedList<TKey, TValue> GetNearestNeighbors(TKey[] point, int queryCount = int.MaxValue)
        {
            if (queryCount > count)
            {
                queryCount = count;
            }
            else if (queryCount <= 0)
            {
                return null;
            }

            OrderedList<TKey, Node> list = new OrderedList<TKey, Node>();
            AddNearestNeighbors(root, point, HyperRect.Infinite(numDimensions, typeMath), 0, list, typeMath.MaxValue(), queryCount);


            OrderedList<TKey, TValue> result = new OrderedList<TKey, TValue>();
            foreach (Pair<Node, TKey> pair in result)
            {
                result.AppendBack(pair.Value, pair.Key.value);
            }
            return result;
        }
    }

    public interface IKDTypeMath<T> where T : IComparable<T>
    {
        int CompareTo(T a, T b);

        T MinValue();
        T MaxValue();

        T Min(T a, T b);
        T Max(T a, T b);

        bool AreEqual(T a, T b);
        bool AreEqual(T[] a, T[] b);

        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);

        T Zero();

        T NegateInfinity();
        T PositiveInfinity();

        T SquaredDistance(T[] a, T[] b);
    }

    public abstract class TypeKDMath<T> : IKDTypeMath<T> where T : IComparable<T>
    {
        public virtual int CompareTo(T a, T b)
        {
            return a.CompareTo(b);
        }

        public abstract T MinValue();
        public abstract T MaxValue();

        public virtual T Min(T a, T b)
        {
            if (a.CompareTo(b) <= 0)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public virtual T Max(T a, T b)
        {
            if (a.CompareTo(b) >= 0)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public abstract bool AreEqual(T a, T b);
        public virtual bool AreEqual(T[] a, T[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!AreEqual(a[i], b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public abstract T Add(T a, T b);
        public abstract T Subtract(T a, T b);
        public abstract T Multiply(T a, T b);

        public abstract T Zero();

        public abstract T NegateInfinity();
        public abstract T PositiveInfinity();

        public abstract T SquaredDistance(T[] a, T[] b);
    }

    public class DoubleKDMath : TypeKDMath<double>
    {
        public override double MinValue()
        {
            return double.MinValue;
        }

        public override double MaxValue()
        {
            return double.MaxValue;
        }

        public override bool AreEqual(double a, double b)
        {
            return a == b;
        }

        public override double Add(double a, double b)
        {
            return a + b;
        }

        public override double Subtract(double a, double b)
        {
            return a - b;
        }

        public override double Multiply(double a, double b)
        {
            return a * b;
        }

        public override double Zero()
        {
            return 0;
        }

        public override double NegateInfinity()
        {
            return double.NegativeInfinity;
        }

        public override double PositiveInfinity()
        {
            return double.PositiveInfinity;
        }

        public override double SquaredDistance(double[] a, double[] b)
        {
            double distance = 0;
            int dimensions = a.Length;

            for (int i = 0; i < dimensions; i++)
            {
                double distOnAxis = Subtract(a[i], b[i]);
                double squaredAxisDist = Multiply(distOnAxis, distOnAxis);

                distance = Add(distance, squaredAxisDist);
            }

            return distance;
        }
    }

    public class FloatKDMath : TypeKDMath<float>
    {
        public override float MinValue()
        {
            return float.MinValue;
        }

        public override float MaxValue()
        {
            return float.MaxValue;
        }

        public override bool AreEqual(float a, float b)
        {
            return a == b;
        }

        public override float Add(float a, float b)
        {
            return a + b;
        }

        public override float Subtract(float a, float b)
        {
            return a - b;
        }

        public override float Multiply(float a, float b)
        {
            return a * b;
        }

        public override float Zero()
        {
            return 0;
        }

        public override float NegateInfinity()
        {
            return float.NegativeInfinity;
        }

        public override float PositiveInfinity()
        {
            return float.PositiveInfinity;
        }

        public override float SquaredDistance(float[] a, float[] b)
        {
            float distance = 0;
            int dimensions = a.Length;

            for (int i = 0; i < dimensions; i++)
            {
                float distOnAxis = Subtract(a[i], b[i]);
                float squaredAxisDist = Multiply(distOnAxis, distOnAxis);

                distance = Add(distance, squaredAxisDist);
            }

            return distance;
        }
    }
}
