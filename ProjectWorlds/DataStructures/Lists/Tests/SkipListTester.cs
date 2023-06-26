using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class SkipListTester : ListTesterBase<SkipList<int>>
    {
        public SkipListTester() : base("Skip List Tester")
        {
            testType = GetType();
        }

        public override bool GetTest2()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
            {
                list.Add(i);
            }

            for (int i = 0; i < 100; i++)
            {
                if (list[i] != i)
                {
                    UnityEngine.Debug.Log(list[i] + " != " + i + " - " + list);
                    return false;
                }
            }

            return true;
        }

        public override bool SetTest1()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            // Replace the 5th item with -1
            list.Set(5, -1);

            return list.Count == 101;
        }

        // Test for the proper exception
        public override bool SetTest2()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            try
            {
                list.Set(11, -1);
                return list.Count == 11;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        // Test for the proper exception
        public override bool SetTest3()
        {
            SkipList<int> list = CreateListDefault();

            try
            {
                list.Set(11, -1);
                return list.Count == 1;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        // Test for the proper exception
        public override bool SetTest4()
        {
            SkipList<int> list = CreateListDefault();

            try
            {
                list.Set(-1, -1);
                return list.Count == 1;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        // Test setting each index
        public override bool SetTest5()
        {
            SkipList<int> list = CreateListDefault();

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

        public override bool InsertTest3()
        {
            SkipList<int> list = CreateListDefault();

            try
            {
                list.Insert(10, 10);
                return list.Count == 1;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public override bool InsertTest4()
        {
            SkipList<int> list = CreateListDefault();

            try
            {
                list.Insert(-1, 10);
                return list.Count == 1;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public override bool InsertRangeTest2B()
        {
            SkipList<int> list = CreateListDefault();
            SkipList<int>  toInsert = CreateListDefault();

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
/*                foreach (int i in toInsert)
                {
                    UnityEngine.Debug.Log(i);
                }*/
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

        public override bool InsertRangeTest3()
        {
            SkipList<int> list = CreateListDefault();
            SkipList<int> toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                toInsert.Add(i);
            }

            try
            {
                list.Insert(1, toInsert, toInsert.Count);
                return list.Count == toInsert.Count;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public override bool InsertRangeTest4()
        {
            SkipList<int> list = CreateListDefault();
            SkipList<int> toInsert = CreateListDefault();

            for (int i = 0; i < 100; i++)
            {
                toInsert.Add(i);
            }

            try
            {
                list.Insert(1, toInsert, toInsert.Count);
                return list.Count == toInsert.Count;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public override bool TakeFirstTest3()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }

            list.TakeFirst();

            for (int i = 1; i < 10; i++)
            {
                if (list[i - 1] != i)
                {
                    UnityEngine.Debug.Log("TakeFirstTest3 failed @ 1: " + list[i - 1] + " != " + i);
                    return false;
                }
            }

            return list.Count == 9;
        }

        public override bool FirstIndexOfTest3()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 9; i >= 0; i--)
            {
                list.Add(i);
            }

            return list.FirstIndexOf(1) == 1;
        }

        public override bool LastIndexOfTest3()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 9; i >= 0; i--)
            {
                list.Add(i);
            }

            return list.LastIndexOf(1) == 1;
        }

        public override bool LastIndexOfTest4()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.LastIndexOf(1) == 9;
        }

        public override bool LastIndexOfTest5()
        {
            SkipList<int> list = CreateListDefault();

            list.Add(-1);
            for (int i = 0; i < 10; i++)
            {
                list.Add(1);
            }

            return list.LastIndexOf(1) == 10;
        }

        public override bool EnumeratorTest2()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
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

        public override bool EnumeratorTest5()
        {
            SkipList<int> list = CreateListDefault();

            for (int i = 99; i >= 0; i--)
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
    }
}
