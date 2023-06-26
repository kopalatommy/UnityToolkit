using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class ListTestRunner : MonoBehaviour
    {
        ArrayListTester arrayListTester;
        CircularListTester circularListTester;
        DoubleLinkedListTester doubleLinkedListTester;
        LinkedListTester linkedListTester;
        SkipListTester skipListTester;
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

            skipListTester = new SkipListTester();
            skipListTester.RunTests(false, 60000);
        }
    }
}
