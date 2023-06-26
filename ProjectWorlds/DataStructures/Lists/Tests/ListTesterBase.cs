using ProjectWorlds.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class ListTesterBase<T> : TesterBase where T : IListExtended<int>, new()
    {
        public ListTesterBase(string testName) : base(testName)
        {

        }

        protected virtual T CreateListDefault()
        {
            return new T();
        }

        #region Get

        [RunTest(true)]
        public virtual bool GetTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool GetTest2()
        {
            T list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
            {
                list.Add(i);
            }

            for (int i = 99; i >= 0; i--)
            {
                if (list[99 - i] != i)
                {
                    UnityEngine.Debug.Log(list[99 - i] + " != " + i + " - " + list);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool GetTest3()
        {
            T list = CreateListDefault();

            try
            {
                int t = list[1];
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool GetTest4()
        {
            T list = CreateListDefault();

            try
            {
                int t = list[-1];
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        #endregion

        #region Set

        [RunTest(true)]
        public virtual bool SetTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            // Replace the 5th item with -1
            list.Set(5, -1);

            return list[5] == -1 && list.Count == 100;
        }

        [RunTest(true)]
        // Test for the proper exception
        public virtual bool SetTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            try
            {
                list.Set(11, -1);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        // Test for the proper exception
        public virtual bool SetTest3()
        {
            T list = CreateListDefault();

            try
            {
                list.Set(11, -1);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        // Test for the proper exception
        public virtual bool SetTest4()
        {
            T list = CreateListDefault();

            try
            {
                list.Set(-1, -1);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        // Test setting each index
        public virtual bool SetTest5()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(0);
            }

            for (int i = 0; i < 100; i++)
            {
                list.Set(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 100;
        }

        #endregion // Set

        #region Add

        [RunTest(true)]
        public virtual bool AddTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(0);
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public virtual bool AddTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 100;
        }

        #endregion

        #region Insert

        [RunTest(true)]
        public virtual bool InsertTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                if (i != 4)
                {
                    list.Add(i);
                }
            }

            list.Insert(4, 4);

            for (int i = 0; i < 10; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public virtual bool InsertTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Insert(i, i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("InsertTest2: " + list[i] + " != " + i);
                    UnityEngine.Debug.Log(list);
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public virtual bool InsertTest3()
        {
            T list = CreateListDefault();

            try
            {
                list.Insert(10, 10);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool InsertTest4()
        {
            T list = CreateListDefault();

            try
            {
                list.Insert(-1, 10);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        #endregion

        #region Insert Range

        [RunTest(true)]
        public virtual bool InsertRangeTest1()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 5; i++)
            {
                list.Add(i);
            }
            for (int i = 5; i < 10; i++)
            {
                toInsert.Add(i);
            }

            list.Insert(5, toInsert, toInsert.Count);

            for (int i = 0; i < 10; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("Insert range test 1 failed @ 1");
                    UnityEngine.Debug.Log(list[i] + " != " + i);
                    UnityEngine.Debug.Log(list.ToString());
                    return false;
                }
            }

            return list.Count == 10;
        }

        [RunTest(true)]
        public virtual bool InsertRangeTest2()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                toInsert.Add(i);
            }

            list.Insert(0, toInsert, toInsert.Count);

            if (list.Count != 100)
            {
                UnityEngine.Debug.Log("InsertRangeTest2 failed @ 1: Count is wrong: " + list.Count + " != 100: " + list);
                return false;
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("InsertRangeTest2 failed @ 2: " + list[i] + " != " + i + " - " + list);
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public virtual bool InsertRangeTest2B()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 5; i++)
            {
                list.Add(i);
            }

            for (int i = 5; i < 95; i++)
            {
                toInsert.Add(i);
            }

            for (int i = 95; i < 100; i++)
            {
                list.Add(i);
            }

            list.Insert(5, toInsert, toInsert.Count);

            if (list.Count != 100)
            {
                UnityEngine.Debug.Log("InsertRangeTest2B failed @ 1: Count is wrong: " + list.Count + " != 100: " + list);
                UnityEngine.Debug.Log(list);
                return false;
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("InsertRangeTest2B failed @ 2: " + list[i] + " != " + i + " - " + list);
/*                    foreach (int t in list)
                    {
                        UnityEngine.Debug.Log(t);
                    }*/
                    return false;
                }
            }

            return list.Count == 100;
        }

        [RunTest(true)]
        public virtual bool InsertRangeTest3()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                toInsert.Add(i);
            }

            try
            {
                list.Insert(1, toInsert, toInsert.Count);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool InsertRangeTest4()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                toInsert.Add(i);
            }

            try
            {
                list.Insert(-1, toInsert, toInsert.Count);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool InsertRangeTest5()
        {
            T list = CreateListDefault();
            T toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }
            for (int i = 100; i < 200; i++)
            {
                toInsert.Add(i);
            }

            list.Insert(100, toInsert, 50);

            for (int i = 0; i < 150; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 150;
        }

        #endregion

        #region Take First

        [RunTest(true)]
        public virtual bool TakeFirstTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (i != list.TakeFirst() || (i != (99 - list.Count)))
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool TakeFirstTest2()
        {
            T list = CreateListDefault();

            try
            {
                list.TakeFirst();
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool TakeFirstTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.TakeFirst();

            for (int i = 1; i < 10; i++)
            {
                if (list[i - 1] != i)
                {
                    return false;
                }
            }

            return list.Count == 9;
        }

        #endregion

        #region Take At

        [RunTest(true)]
        public virtual bool TakeAtTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            if (list.TakeAt(5) != 5)
            {
                UnityEngine.Debug.Log("TakeAtTest1 failed @ 3");
                UnityEngine.Debug.Log("TakeAtTest1: " + list);
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("TakeAtTest1 failed @ 1");
                    return false;
                }
            }

            for (int i = 5; i < 9; i++)
            {
                if (list[i] != (i + 1))
                {
                    UnityEngine.Debug.Log("TakeAtTest1 failed @ 2: " + list[i] + " != " + (i + 1) + " - " + list);
                    return false;
                }
            }

            return list.Count == 9;
        }

        [RunTest(true)]
        public virtual bool TakeAtTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            if (list.TakeAt(5) != 5 || list.Count != 9)
            {
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            for (int i = 5; i < 9; i++)
            {
                if (list[i] != (i + 1))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        private bool TakeAtTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            try
            {
                list.TakeAt(100);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        private bool TakeAtTest4()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            try
            {
                list.TakeAt(-1);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        private bool TakeAtTest5()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            for (int i = 8; i > 1; i--)
            {
                if (list.TakeAt(i) != i)
                {
                    return false;
                }
            }

            return list.Count == 3;
        }

        [RunTest(true)]
        private bool TakeAtTest6()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            if (list.TakeAt(8) != 8)
            {
                return false;
            }

            if (list.TakeAt(6) != 6)
            {
                return false;
            }

            if (list.TakeAt(4) != 4)
            {
                return false;
            }

            if (list.TakeAt(2) != 2)
            {
                return false;
            }

            if (list.TakeAt(0) != 0)
            {
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (list[i] != 1 + (i * 2))
                {
                    return false;
                }
            }

            return list.Count == 5;
        }

        [RunTest(true)]
        private bool TakeAtTest7()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            if (list.TakeAt(9) != 9)
            {
                return false;
            }

            if (list.TakeAt(7) != 7)
            {
                return false;
            }

            if (list.TakeAt(5) != 5)
            {
                return false;
            }

            if (list.TakeAt(3) != 3)
            {
                return false;
            }

            if (list.TakeAt(1) != 1)
            {
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (list[i] != (i * 2))
                {
                    return false;
                }
            }

            return list.Count == 5;
        }

        #endregion

        #region Take Last

        [RunTest(true)]
        public virtual bool TakeLastTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 99; i >= 0; i--)
            {
                int t = list.TakeLast();
                if (i != t)
                {
                    UnityEngine.Debug.Log("TakeLastTest1 failed @ 1: " + t + " != " + i);
                    return false;
                }
                else if (i != list.Count)
                {
                    UnityEngine.Debug.Log("TakeLastTest1 failed @ 2: Count: " + list.Count + " != " + i);
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool TaketLastTest2()
        {
            T list = CreateListDefault();

            try
            {
                list.TakeLast();
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool TakeLastTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.TakeLast();

            for (int i = 0; i < 9; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 9;
        }

        #endregion

        #region Remove First

        [RunTest(true)]
        public virtual bool RemoveFirstTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                list.RemoveFirst();
                if ((i < 99 && list[0] != (i + 1)) || (i != (99 - list.Count)))
                {
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool RemoveFirstTest2()
        {
            T list = CreateListDefault();

            try
            {
                list.RemoveFirst();
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool RemoveFirstTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.RemoveFirst();

            for (int i = 1; i < 10; i++)
            {
                if (list[i - 1] != i)
                {
                    return false;
                }
            }

            return list.Count == 9;
        }

        #endregion

        #region Remove Last

        [RunTest(true)]
        public virtual bool RemoveLastTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 99; i >= 0; i--)
            {
                list.RemoveLast();
                if ((i > 0 && list[i - 1] != (i - 1)))
                {
                    UnityEngine.Debug.Log("RemoveLastTest1 @ 1");
                    return false;
                }
                else if ((i != (list.Count)))
                {
                    UnityEngine.Debug.Log("RemoveLastTest1 @ 2");
                    return false;
                }
            }

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool RemoeFirstLastTest2()
        {
            T list = CreateListDefault();

            try
            {
                list.RemoveLast();
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        [RunTest(true)]
        public virtual bool RemoveLastTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.RemoveLast();

            for (int i = 0; i < 9; i++)
            {
                if (list[i] != i)
                {
                    return false;
                }
            }

            return list.Count == 9;
        }

        #endregion

        #region Remove Range

        [RunTest(true)]
        private bool RemoveRangeTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.RemoveRange(3, 4);

            for (int i = 0; i < 3; i++)
            {
                if (list[i] != i)
                {
                    // Debug.Log(list[i] + " != " + i);
                    return false;
                }
            }

            for (int i = 7; i < 10; i++)
            {
                if (list[i - 4] != i)
                {
                    // Debug.Log((i - 4) + " : " + list[i - 4] + " != " + i);
                    return false;
                }
            }

            return list.Count == 6;
        }

        [RunTest(true)]
        private bool RemoveRangeTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            try
            {
                list.RemoveRange(3, 10);
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }

        #endregion

        #region Remove All

        [RunTest(true)]
        private bool RemoveAllTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(5);
            }

            list.RemoveAll(5);

            return list.Count == 0;
        }

        [RunTest(true)]
        private bool RemoveAllTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    list.Add(5);
                }
                else
                {
                    list.Add(0);
                }
            }

            list.RemoveAll(5);

            return list.Count == 5;
        }

        [RunTest(true)]
        private bool RemoveAllTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    list.Add(5);
                }
                else
                {
                    list.Add(0);
                }
            }

            list.RemoveAll(7);

            return list.Count == 10;
        }

        #endregion

        #region Remove

        [RunTest(true)]
        public virtual bool RemoveTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            if (!list.Remove(5))
            {
                UnityEngine.Debug.Log("RemoveTest1 failed @ 1: No 5");
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log("RemoveTest1 failed @ 2");
                    UnityEngine.Debug.Log("RemoveTest1: " + list);
                    return false;
                }
            }
            for (int i = 6; i < 9; i++)
            {
                if (list[i - 1] != i)
                {
                    UnityEngine.Debug.Log("RemoveTest1 failed @ 3");
                    return false;
                }
            }

            return list.Count == 99;
        }

        [RunTest(true)]
        public virtual bool RemoveTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i % 2 == 0 ? 5 : 1);
            }

            for (int i = 0; i < 50; i++)
            {
                if (!list.Remove(5))
                {
                    return false;
                }
            }

            for (int i = 0; i < 50; i++)
            {
                if (list[i] != 1)
                {
                    return false;
                }
            }

            return list.Count == 50;
        }

        [RunTest(true)]
        public virtual bool RemoveTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i % 2 == 0 ? 5 : 1);
            }

            if (list.Remove(-1))
            {
                return false;
            }

            return list.Count == 100;
        }

        #endregion

        #region First Index Of

        [RunTest(true)]
        public virtual bool FirstIndexOfTest1()
        {
            T list = CreateListDefault();

            return list.FirstIndexOf(1) == -1;
        }

        [RunTest(true)]
        public virtual bool FirstIndexOfTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            return list.FirstIndexOf(1) == 1;
        }

        [RunTest(true)]
        public virtual bool FirstIndexOfTest3()
        {
            T list = CreateListDefault();

            for (int i = 9; i >= 0; i--)
            {
                list.Add(i);
            }

            return list.FirstIndexOf(1) == 8;
        }

        [RunTest(true)]
        public virtual bool FirstIndexOfTest4()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.FirstIndexOf(1) == 0;
        }

        [RunTest(true)]
        public virtual bool FirstIndexOfTest5()
        {
            T list = CreateListDefault();

            list.Add(-1);
            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.FirstIndexOf(1) == 1;
        }

        #endregion

        #region Last Index Of

        [RunTest(true)]
        public virtual bool LastIndexOfTest1()
        {
            T list = CreateListDefault();

            return list.LastIndexOf(1) == -1;
        }

        [RunTest(true)]
        public virtual bool LastIndexOfTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            return list.LastIndexOf(1) == 1;
        }

        [RunTest(true)]
        public virtual bool LastIndexOfTest3()
        {
            T list = CreateListDefault();

            for (int i = 9; i >= 0; i--)
            {
                list.Add(i);
            }

            return list.LastIndexOf(1) == 8;
        }

        [RunTest(true)]
        public virtual bool LastIndexOfTest4()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.LastIndexOf(1) == 9;
        }

        [RunTest(true)]
        public virtual bool LastIndexOfTest5()
        {
            T list = CreateListDefault();

            list.Add(-1);
            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.LastIndexOf(1) == 10;
        }

        #endregion

        #region To Array

        [RunTest(true)]
        public virtual bool ToArrayTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int[] arr = list.ToArray();

            for (int i = 0; i < 100; i++)
            {
                if (arr[i] != list[i])
                {
                    UnityEngine.Debug.Log("ToArrayTest1 failed @ 1: " + arr + " - " + list);
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool ToArrayTest2()
        {
            T list = CreateListDefault();

            System.Random rand = new System.Random();
            for (int i = 0; i < 100; i++)
            {
                list.Add(rand.Next());
            }

            int[] arr = list.ToArray();

            for (int i = 0; i < 100; i++)
            {
                if (arr[i] != list[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Enumerator

        [RunTest(true)]
        public virtual bool  EnumeratorTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int temp = 0;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp++;
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool EnumeratorTest2()
        {
            T list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
            {
                list.Add(i);
            }

            int temp = 99;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp--;
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool EnumeratorTest3()
        {
            T list = CreateListDefault();

            int temp = 99;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp--;
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool EnumeratorTest4()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            int temp = 0;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp++;
            }

            temp = 0;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp++;
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool EnumeratorTest5()
        {
            T list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
            {
                list.Add(i);
            }

            int temp = 99;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp--;
            }

            temp = 99;
            foreach (int val in list)
            {
                if (val != temp)
                {
                    return false;
                }
                temp--;
            }

            return true;
        }

        #endregion

        #region Contains

        [RunTest(true)]
        public virtual bool ContainsTest1()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (!list.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool ContainsTest2()
        {
            T list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (!list.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }

        [RunTest(true)]
        public virtual bool ContainsTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            return !list.Contains(-1);
        }

        [RunTest(true)]
        public virtual bool ContainsTest4()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            return !list.Contains(101);
        }

        [RunTest(true)]
        public virtual bool ContainsTest5()
        {
            T list = CreateListDefault();

            return !list.Contains(-1);
        }

        #endregion

        #region Clear

        [RunTest(true)]
        public virtual bool ClearTest1()
        {
            T list = CreateListDefault();

            list.Clear();

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool ClearTest2()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.Clear();

            return list.Count == 0;
        }

        [RunTest(true)]
        public virtual bool ClearTest3()
        {
            T list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.Clear();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            return list.Count == 10;
        }

        #endregion
    }
}
