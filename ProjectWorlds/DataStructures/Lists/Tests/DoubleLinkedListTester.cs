using ProjectWorlds.Testing;
using ProjectWorlds.DataStructures.Lists;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class DoubleLinkedListTester : ListTesterBase<DoubleLinkedList<int>>
    {
        public DoubleLinkedListTester() : base("Double Linked List Tester")
        {
            testType = GetType();
        }
    }
}
