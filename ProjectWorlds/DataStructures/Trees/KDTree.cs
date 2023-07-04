using UnityEngine;
using System;
using ProjectWorlds.DataStructures.Heaps;
using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Queues;
using ProjectWorlds.MemoryManagement;

namespace ProjectWorlds.DataStructures.Trees
{
    public class KDTree<T>
    {
        public struct KDBounds
        {
            public Vector3 Size { get { return max - min; } }

            public Bounds Bounds
            {
                get
                {
                    return new Bounds((min + max) / 2, max - min);
                }
            }

            public Vector3 min, max;

            public KDBounds(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }

            public Vector3 ClosestPoint(Vector3 point)
            {
                if (point.x < min.x)
                {
                    point.x = min.x;
                }
                else if (point.x > max.x)
                {
                    point.x = max.x;
                }

                if (point.y < min.y)
                {
                    point.y = min.y;
                }
                else if (point.y > max.y)
                {
                    point.y = max.y;
                }

                if (point.z < min.z)
                {
                    point.z = min.z;
                }
                else if (point.z > max.z)
                {
                    point.z = max.z;
                }

                return point;
            }
        }

        public class Node
        {
            public int Count { get { return end - start; } }
            public bool IsLeaf { get { return partitionAxis == -1; } }

            public float partitionCoordinate;
            public int partitionAxis = -1;

            public Node negative;
            public Node positive;

            public int start;
            public int end;

            public KDBounds bounds;
        }

        #region Variables

        public int Count { get { return count; } }

        // Array of points that make up the kd tree
        Vector3[] points;
        // Index array
        private int[] permutation;
        // Array of values in the tree
        private T[] values;
        // Number of points in tree
        private int count;

        private int maxPointsPerLeaf;
        // Pool of nodes
        ObjectPool<Node> pool;

        // Root node of tree
        Node root = null;

        #endregion

        public KDTree(int maxPointsPerLeaf = 32, int initialPoolSize = 64)
        {
            count = 0;
            points = new Vector3[count];
            values = new T[count];
            permutation = new int[count];
            this.maxPointsPerLeaf = maxPointsPerLeaf;
            pool = new ObjectPool<Node>(64);
        }

        public void AddPoint(Vector3 point, T value, bool build = true)
        {
            count++;
            Array.Resize(ref points, count);
            Array.Resize(ref permutation, count);
            Array.Resize(ref values, count);

            points[count - 1] = point;
            permutation[count - 1] = 0;
            values[count - 1] = value;

            if (build)
            {
                Rebuild();
            }
        }

        public void AddPoints(Vector3[] points, T[] values, bool build = true)
        {
            Array.Resize(ref this.points, count + points.Length);
            Array.Resize(ref permutation, count + points.Length);
            Array.Resize(ref this.values, count + points.Length);

            Array.Copy(points, 0, this.points, count, points.Length);
            Array.Copy(values, 0, this.values, count, values.Length);

            count += points.Length;

            if (build)
            {
                Rebuild();
            }
        }

        public void SetPoints(Vector3[] points, T[] values, bool build = true)
        {
            count = points.Length;
            Array.Resize(ref this.points, count);
            Array.Resize(ref permutation, count);
            Array.Resize(ref this.values, count);

            Array.Copy(points, this.points, count);
            Array.Copy(values, this.values, count);

            if (build)
            {
                Rebuild();
            }
        }

        public void Rebuild()
        {
            for (int i = 0; i < count; i++)
            {
                permutation[i] = i;
            }

            BuildTree();
        }

        private void BuildTree()
        {
            root = pool.Pop();
            root.bounds = MakeBounds();
            root.start = 0;
            root.end = count;

            SplitNode(root);
        }

        KDBounds MakeBounds()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            int even = count & ~1;
            //int even = count / 2;
            for (int i = 0; i < even; i += 2)
            {
                int j = i + 1;

                if (points[i].x > points[j].x)
                {
                    if (points[i].x > max.x)
                    {
                        max.x = points[i].x;
                    }
                    if (points[j].x < min.x)
                    {
                        min.x = points[j].x;
                    }
                }
                else
                {
                    if (points[i].x < min.x)
                    {
                        min.x = points[i].x;
                    }
                    if (points[j].x > max.x)
                    {
                        max.x = points[j].x;
                    }
                }

                if (points[i].y > points[j].y)
                {
                    if (points[i].y > max.y)
                    {
                        max.y = points[i].y;
                    }
                    if (points[j].y < min.y)
                    {
                        min.y = points[j].y;
                    }
                }
                else
                {
                    if (points[i].y < min.y)
                    {
                        min.y = points[i].y;
                    }
                    if (points[j].y > max.y)
                    {
                        max.y = points[j].y;
                    }
                }

                if (points[i].z > points[j].z)
                {
                    if (points[i].z > max.z)
                    {
                        max.z = points[i].z;
                    }
                    if (points[j].z < min.z)
                    {
                        min.z = points[j].z;
                    }
                }
                else
                {
                    if (points[i].z < min.z)
                    {
                        min.z = points[i].z;
                    }
                    if (points[j].z > max.z)
                    {
                        max.z = points[j].z;
                    }
                }
            }

            // Account for last element when only 1 item
            if (even != count)
            {
                if (min.x > points[even].x)
                {
                    min.x = points[even].x;
                }
                if (max.x < points[even].x)
                {
                    max.x = points[even].x;
                }

                if (min.y > points[even].y)
                {
                    min.y = points[even].y;
                }
                if (max.y < points[even].y)
                {
                    max.y = points[even].y;
                }

                if (min.z > points[even].z)
                {
                    min.z = points[even].z;
                }
                if (max.z < points[even].z)
                {
                    max.z = points[even].z;
                }
            }

            return new KDBounds(min, max);
        }

        private void SplitNode(Node node)
        {
            KDBounds bounds = node.bounds;
            Vector3 nodeBoundsSize = bounds.Size;

            int splitAxis = 0;
            float axisSize = nodeBoundsSize.x;

            if (axisSize < nodeBoundsSize.y)
            {
                splitAxis = 1;
                axisSize = nodeBoundsSize.y;
            }

            if (axisSize < nodeBoundsSize.z)
            {
                splitAxis = 2;
            }

            float boundsStart = bounds.min[splitAxis];
            float boundsEnd = bounds.max[splitAxis];

            float splitPivot = CalculatePivot(node.start, node.end, boundsStart, boundsEnd, splitAxis);

            node.partitionAxis = splitAxis;
            node.partitionCoordinate = splitPivot;

            int splittingIndex = Partition(node.start, node.end, splitPivot, splitAxis);

            Vector3 negMax = bounds.max;
            negMax[splitAxis] = splitPivot;

            Node negNode = pool.Pop();
            if (negNode == null)
            {
                Debug.Log("Neg node is null");
            }
            negNode.bounds = bounds;
            negNode.bounds.max = negMax;
            negNode.start = node.start;
            negNode.end = splittingIndex;
            node.negative = negNode;

            Vector3 posMin = bounds.min;
            posMin[splitAxis] = splitPivot;

            Node posNode = pool.Pop();
            posNode.bounds = bounds;
            posNode.bounds.min = posMin;
            posNode.start = splittingIndex;
            posNode.end = node.end;
            node.positive = posNode;

            if (negNode.Count != 0 && posNode.Count != 0)
            {
                if (ContinueSplit(negNode))
                {
                    SplitNode(negNode);
                }
                if (ContinueSplit(posNode))
                {
                    SplitNode(posNode);
                }
            }
        }

        private float CalculatePivot(int start, int end, float boundsStart, float boundsEnd, int axis)
        {
            float midPoint = (boundsStart + boundsEnd) / 2;

            bool negative = false;
            bool positive = false;

            for (int i = start; i < end; i++)
            {
                if (points[permutation[i]][axis] < midPoint)
                {
                    negative = true;
                }
                else
                {
                    positive = true;
                }

                if (negative && positive)
                {
                    return midPoint;
                }
            }

            if (negative)
            {
                float negMax = float.MinValue;
                for (int i = start; i < end; i++)
                {
                    if (negMax < points[permutation[i]][axis])
                    {
                        negMax = points[permutation[i]][axis];
                    }
                }
                return negMax;
            }
            else
            {
                float posMin = float.MaxValue;
                for (int i = start; i < end; i++)
                {
                    if (posMin > points[permutation[i]][axis])
                    {
                        posMin = points[permutation[i]][axis];
                    }
                }
                return posMin;
            }
        }

        private int Partition(int start, int end, float partitionPivot, int axis)
        {
            int leftIndex = start - 1;
            int rightIndex = end;

            int temp;
            while (true)
            {
                do
                {
                    leftIndex++;
                } while (leftIndex < rightIndex && points[permutation[leftIndex]][axis] < partitionPivot);

                do
                {
                    rightIndex--;
                }
                while (leftIndex < rightIndex && points[permutation[rightIndex]][axis] >= partitionPivot);

                if (leftIndex < rightIndex)
                {
                    temp = permutation[leftIndex];
                    permutation[leftIndex] = permutation[rightIndex];
                    permutation[rightIndex] = temp;
                }
                else
                {
                    return leftIndex;
                }
            }
        }

        private bool ContinueSplit(Node node)
        {
            return node.Count > maxPointsPerLeaf;
        }

        internal class QueryNode : IComparable
        {
            public Node node;
            public Vector3 closestPoint;
            public float distance;

            public QueryNode()
            {

            }

            public QueryNode(Node node, Vector3 closest, float distance)
            {
                this.node = node;
                closestPoint = closest;
                this.distance = distance;
            }

            public void Populate(Node node, Vector3 closest, float distance)
            {
                this.node = node;
                closestPoint = closest;
                this.distance = distance;
            }

            public void Populate(Node node, Vector3 closestPoint, Vector3 queryPos)
            {
                this.node = node;
                this.closestPoint = closestPoint;
                this.distance = Vector3.SqrMagnitude(closestPoint - queryPos);
            }

            public void Populate(Node node, Vector3 closestPoint)
            {
                this.node = node;
                this.closestPoint = closestPoint;
                this.distance = float.MaxValue;
            }

            public int CompareTo(QueryNode other)
            {
                if (other.distance == distance)
                {
                    return 0;
                }
                else if (distance < other.distance)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            public int CompareTo(object obj)
            {
                if (obj is QueryNode)
                {
                    return CompareTo((QueryNode)obj);
                }
                else
                {
                    return -1;
                }
            }
        }

        private ObjectPool<QueryNode> queryPool;
        private MinHeap<QueryNode> queryMinHeap;

        private void ResetQuery()
        {
            if (queryMinHeap == null)
            {
                queryMinHeap = new MinHeap<QueryNode>();
            }
            else
            {
                queryMinHeap.Clear();
            }
            if (queryPool == null)
            {
                queryPool = new ObjectPool<QueryNode>();
            }
            else
            {
                queryPool.Clear();
            }
        }

        public void ClosestPoint(Vector3 queryLoc, ArrayList<T> resultValues, ArrayList<float> resultDistances = null)
        {
            ResetQuery();

            if (points.Length == 0)
            {
                Debug.Log("Trying to search empty tree");
                return;
            }

            int smallestIndex = 0;
            float smallestSquaredRadius = float.PositiveInfinity;

            QueryNode qN = queryPool.Pop();
            qN.Populate(root, root.bounds.ClosestPoint(queryLoc), Vector3.SqrMagnitude(root.bounds.ClosestPoint(queryLoc) - queryLoc));
            queryMinHeap.Insert(qN);

            QueryNode queryNode = null;
            Node node = null;

            int partitionAxis;
            float partitionCoord;

            Vector3 closestPoint;

            while (queryMinHeap.Count > 0)
            {
                queryNode = queryMinHeap.TakeMin();

                if (queryNode.distance > smallestSquaredRadius)
                {
                    continue;
                }

                node = queryNode.node;

                if (!node.IsLeaf)
                {
                    partitionAxis = node.partitionAxis;
                    partitionCoord = node.partitionCoordinate;

                    closestPoint = queryNode.closestPoint;

                    if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.negative, closestPoint, Vector3.SqrMagnitude(closestPoint - queryLoc));
                        queryMinHeap.Insert(qN);

                        closestPoint[partitionAxis] = partitionCoord;
                        if (node.positive.Count != 0)
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.positive, closestPoint, Vector3.SqrMagnitude(closestPoint - queryLoc));
                            queryMinHeap.Insert(qN);
                        }
                    }
                    else
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.positive, closestPoint, Vector3.SqrMagnitude(closestPoint - queryLoc));
                        queryMinHeap.Insert(qN);

                        closestPoint[partitionAxis] = partitionCoord;
                        if (node.positive.Count != 0)
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.negative, closestPoint, Vector3.SqrMagnitude(closestPoint - queryLoc));
                            queryMinHeap.Insert(qN);
                        }
                    }
                }
                else
                {
                    float dist;
                    int index;
                    for (int i = node.start; i < node.end; i++)
                    {
                        index = permutation[i];
                        dist = Vector3.SqrMagnitude(points[index] - queryLoc);
                        if (dist < smallestSquaredRadius)
                        {
                            smallestSquaredRadius = dist;
                            smallestIndex = index;
                        }
                    }
                }
            }
            resultValues.Add(values[smallestIndex]);
            if (resultDistances != null)
            {
                resultDistances.Add(smallestSquaredRadius);
            }
        }

        public void Interval(Vector3 min, Vector3 max, ArrayList<T> resultValues)
        {
            ResetQuery();

            Vector3[] searchPoints = points;
            int[] searchPermutation = permutation;

            Queue<QueryNode> queryQueue = new Queue<QueryNode>();

            QueryNode qN = queryPool.Pop();
            qN.Populate(root, root.bounds.ClosestPoint((min + max) / 2));
            queryQueue.AppendBack(qN);

            Node node;
            QueryNode queryNode;
            while (queryQueue.Count > 0)
            {
                queryNode = queryQueue.TakeFirst();
                node = queryNode.node;

                if (!node.IsLeaf)
                {
                    int partitionIndex = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 closestPoint = queryNode.closestPoint;

                    if ((closestPoint[partitionIndex] - partitionCoord) < 0)
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.negative, closestPoint);
                        queryQueue.AppendBack(qN);

                        closestPoint[partitionIndex] = partitionCoord;

                        if (node.positive.Count != 0 &&
                            closestPoint[partitionIndex] <= max[partitionIndex])
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.positive, closestPoint);
                            queryQueue.AppendBack(qN);
                        }
                    }
                    else
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.positive, closestPoint);
                        queryQueue.AppendBack(qN);

                        closestPoint[partitionIndex] = partitionCoord;

                        if (node.negative.Count != 0 &&
                            closestPoint[partitionIndex] >= min[partitionIndex])
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.negative, closestPoint);
                            queryQueue.AppendBack(qN);
                        }
                    }
                }
                else
                {
                    if (node.bounds.min[0] >= min[0] &&
                        node.bounds.min[1] >= min[1] &&
                        node.bounds.min[2] >= min[2] &&

                        node.bounds.max[0] <= max[0] &&
                        node.bounds.max[1] <= max[1] &&
                        node.bounds.max[2] <= max[2])
                    {
                        for (int i = node.start; i < node.end; i++)
                        {
                            resultValues.Add(values[permutation[i]]);
                        }
                    }
                    else
                    {
                        for (int i = node.start; i < node.end; i++)
                        {
                            int index = permutation[i];
                            Vector3 v = points[index];

                            if (v[0] >= min[0] &&
                                v[1] >= min[1] &&
                                v[2] >= min[2] &&

                                v[0] <= max[0] &&
                                v[1] <= max[1] &&
                                v[2] <= max[2])
                            {
                                resultValues.Add(values[index]);
                            }
                        }
                    }
                }
            }
        }

        public void KNearest(Vector3 queryLoc, int k, IListExtended<T> resultIndices, IListExtended<float> resultDistances = null)
        {
            if (k > count)
            {
                k = count;
            }

            OrderedList<int, float> smallestIndices = new OrderedList<int, float>();
            Queue<QueryNode> queue = new Queue<QueryNode>();

            ResetQuery();

            float BSSR = float.PositiveInfinity;

            QueryNode qN = queryPool.Pop();
            qN.Populate(root, root.bounds.ClosestPoint(queryLoc), queryLoc);
            queue.AppendBack(qN);

            int partitionAxis;
            float partitionCoord;
            Vector3 closestPoint;

            QueryNode cur;
            Node node;
            while (queue.Count > 0)
            {
                cur = queue.TakeFirst();

                // Location is farther away than the max in the k
                // nearest collection
                if (cur.distance > BSSR)
                {
                    UnityEngine.Debug.Log("Skipping node; " + cur.node.start + " - " + cur.node.end);
                    UnityEngine.Debug.Log("BSSR: " + BSSR + " dist: " + cur.distance);
                    continue;
                }

                node = cur.node;

                if (!node.IsLeaf)
                {
                    partitionAxis = node.partitionAxis;
                    partitionCoord = node.partitionCoordinate;
                    closestPoint = cur.closestPoint;

                    if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.negative, closestPoint, queryLoc);
                        queue.AppendBack(qN);

                        closestPoint[partitionAxis] = partitionCoord;

                        if (node.positive.Count != 0)
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.positive, closestPoint, queryLoc);
                            queue.AppendBack(qN);
                        }
                    }
                    else
                    {
                        qN = queryPool.Pop();
                        qN.Populate(node.positive, closestPoint, queryLoc);
                        queue.AppendBack(qN);

                        closestPoint[partitionAxis] = partitionCoord;

                        if (node.positive.Count != 0)
                        {
                            qN = queryPool.Pop();
                            qN.Populate(node.negative, closestPoint, queryLoc);
                            queue.AppendBack(qN);
                        }
                    }
                }
                else
                {
                    float sqrDist;
                    for (int i = node.start; i < node.end; i++)
                    {
                        int index = permutation[i];
                        sqrDist = Vector3.SqrMagnitude(points[index] - queryLoc);

                        if (sqrDist <= BSSR)
                        {
                            smallestIndices.AppendBack(index, sqrDist);

                            if (smallestIndices.Count >= k)
                            {
                                BSSR = smallestIndices[smallestIndices.Count - k].value;
                            }
                        }
                    }
                }
            }

            for (int i = smallestIndices.Count - k; i < smallestIndices.Count; i++)
            {
                ComparablePair<int, float> temp = smallestIndices[i];
                resultIndices.Add(values[temp.key]);
                if (resultDistances != null)
                {
                    resultDistances.Add(temp.value);
                }
            }
        }

        public void Radius(Vector3 queryLoc, float radius, IListExtended<T> resultValues)
        {
            float squaredDist = radius * radius;

            ResetQuery();

            Queue<QueryNode> queue = new Queue<QueryNode>();

            QueryNode queryNode = queryPool.Pop();
            queryNode.Populate(root, root.bounds.ClosestPoint(queryLoc));
            queue.AppendBack(queryNode);

            Node node;
            QueryNode toAdd;
            while (queue.Count > 0)
            {
                queryNode = queue.Dequeue();
                node = queryNode.node;

                if (!node.IsLeaf)
                {
                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 closestPoint = queryNode.closestPoint;

                    if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                    {
                        toAdd = queryPool.Pop();
                        toAdd.Populate(node.negative, node.negative.bounds.ClosestPoint(closestPoint));
                        queue.AppendBack(toAdd);

                        closestPoint[partitionAxis] = partitionCoord;

                        float sqrDist = Vector3.SqrMagnitude(closestPoint - queryLoc);

                        if (node.positive.Count != 0 && sqrDist <= squaredDist)
                        {
                            toAdd = queryPool.Pop();
                            toAdd.Populate(node.positive, node.positive.bounds.ClosestPoint(closestPoint));
                            queue.Add(toAdd);
                        }
                    }
                    else
                    {
                        toAdd = queryPool.Pop();
                        toAdd.Populate(node.positive, node.positive.bounds.ClosestPoint(closestPoint));
                        queue.Add(toAdd);

                        closestPoint[partitionAxis] = partitionCoord;

                        float sqrDist = Vector3.SqrMagnitude(closestPoint - queryLoc);

                        if (node.negative.Count != 0 && sqrDist <= squaredDist)
                        {
                            toAdd = queryPool.Pop();
                            toAdd.Populate(node.negative, node.negative.bounds.ClosestPoint(queryLoc));
                            queue.Add(toAdd);
                        }
                    }
                }
                else
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        int index = permutation[i];

                        if (Vector3.SqrMagnitude(points[index] - queryLoc) <= squaredDist)
                        {
                            resultValues.Add(values[index]);
                        }
                    }
                }
            }
        }
    }
}
