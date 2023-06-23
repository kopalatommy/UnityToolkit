using System;

namespace ProjectWorlds.Testing
{
    public class RunTestAttribute : Attribute
    {
        public bool RunTest { get { return runTest; } }
        public object[] TestArgs { get { return testArgs; } }

        private bool runTest = false;
        private object[] testArgs = null;

        public RunTestAttribute(bool runTest, object[] testArgs = null)
        {
            this.runTest = runTest;
            if (testArgs == null)
            {
                this.testArgs = new object[] { };
            }
            else
            {
                this.testArgs = testArgs;
            }
        }
    }
}
