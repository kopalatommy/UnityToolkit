using ProjectWorlds.Testing;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class CircularListTester : ListTesterBase<CircularList<int>>
    {
        public CircularListTester() : base("Circular List Tester")
        {
            testType = GetType();
        }

        protected override CircularList<int> CreateListDefault()
        {
            return new CircularList<int>(200);
        }

        #region Add

        [RunTest(true)]
        public bool OverAddTest()
        {
            CircularList<int> list = new CircularList<int>(10);

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < 10; i++)
            {
                if (list[i] != (90 + i))
                {
                    return false;
                }
            }

            return list.Count == 10;
        }

        #endregion
    }
}
