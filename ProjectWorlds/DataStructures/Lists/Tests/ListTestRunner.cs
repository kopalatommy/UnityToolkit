using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class ListTestRunner : MonoBehaviour
    {
        private ArrayListTester arrayListTester;
        private CircularListTester circularListTester;
        private DoubleLinkedListTester doubleLinkedListTester;
        private LinkedListTester linkedListTester;
        private SkipListTester skipListTester;
        private OrderedListTester orderedListTester;

        private void Start()
        {
            /*arrayListTester = new ArrayListTester();
            arrayListTester.RunTests(false, 60000);

            circularListTester = new CircularListTester();
            circularListTester.RunTests(false, 600000);

            doubleLinkedListTester = new DoubleLinkedListTester();
            doubleLinkedListTester.RunTests(false, 60000);

            linkedListTester = new LinkedListTester();
            linkedListTester.RunTests(false, 60000);*/

            /*skipListTester = new SkipListTester();
            skipListTester.RunTests(false, 60000);*/

            /*orderedListTester = new OrderedListTester();
            orderedListTester.RunTests(false, 60000);*/
        }
    }
}
