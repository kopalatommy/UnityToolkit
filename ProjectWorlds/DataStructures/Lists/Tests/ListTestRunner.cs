using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.DataStructures.Lists.Tests
{
    public class ListTestRunner : MonoBehaviour
    {
        ArrayListTester arrayListTester;
        CircularListTester circularListTester;

        private void Start()
        {
            arrayListTester = new ArrayListTester();
            arrayListTester.RunTests(false, 60000);

            circularListTester = new CircularListTester();
            circularListTester.RunTests(false, 600000);

        }
    }
}
