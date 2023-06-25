using ProjectWorlds.Testing;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class LinkedListTester : ListTesterBase<LinkedList<int>>
    {
        public LinkedListTester() : base("Linked List Tester")
        {
            testType = GetType();
        }
    }
}
