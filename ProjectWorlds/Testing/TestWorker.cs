using System.Reflection;
using System.Threading;

namespace ProjectWorlds.Testing
{
    public delegate void KillTimerDelegate(int arg, bool wasSuccess);

    public class TestWorker
    {
        public KillTimerDelegate FinishedCallBack { get; set; }

        public bool Finished { get { return finished; } }
        public bool Success { get { return success; } }

        public object[] Args { get { return args; } }

        private bool finished = false;
        private bool success = false;

        private MethodInfo testFunct = null;
        private object tester = null;
        private object[] args = null;

        public TestWorker(MethodInfo testFunct, object tester, object[] args)
        {
            this.testFunct = testFunct;
            this.tester = tester;
            this.args = args;
        }

        public void DoWork()
        {
            object ret = testFunct.Invoke(tester, args);

            if (ret != null)
            {
                success = (bool)ret;
            }
            else
            {
                success = false;
            }
            FinishedCallBack(Thread.CurrentThread.ManagedThreadId, success);
            finished = true;
        }
    }
}
