using ProjectWorlds.Testing;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class ArrayListTester : ListTesterBase<ArrayList<int>>
    {
        public ArrayListTester() : base("Array List Tester")
        {
            testType = GetType();
        }
    }
}
