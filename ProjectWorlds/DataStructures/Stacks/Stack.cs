using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.DataStructures.Stacks
{
    public class Stack<T>
    {
        internal class Node
        {
            public Node child;
            public T value;
        }

        private int count = 0;
        Node top = null;

        private int maxCount = -1;

        public int Count
        {
            get { return count; }
        }

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        public void Clear()
        {
            top = null;
            count = 0;
        }

        public virtual void Push(T item)
        {
            if (maxCount == -1 || count < maxCount)
            {
                top = new Node { value = item, child = top };
                count++;
            }
        }

        public virtual T Peek()
        {
            if (top == null)
                throw new System.Exception("Stack is empty");
            else
                return top.value;
        }

        public virtual T Pop()
        {
            if (top == null)
                throw new System.Exception("Stack is empty");
            else
            {
                T val = top.value;
                top = top.child;
                count--;
                return val;
            }
        }
    }
}
