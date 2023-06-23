using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ProjectWorlds.Testing
{
    public class TesterBase
    {
        protected Dictionary<int, Thread> activeThreads = new Dictionary<int, Thread>();
        protected Dictionary<int, Timer> activeTimers = new Dictionary<int, Timer>();
        protected Dictionary<int, string> activeNames = new Dictionary<int, string>();

        protected int numberOfTests = 0;
        protected int successes = 0;

        protected string testName = "Base Test";

        protected bool verbose = false;

        protected Type testType = null;

        protected string logFilePath;

        public TesterBase(string testName)
        {
            this.testName = testName;

            // Format the test name for the log file path
            testName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(testName);
            testName = testName.Replace(" ", "");

            logFilePath = "TestLogs/" + testName + "Log.txt";
        }

        ~TesterBase()
        {
            foreach (int threadID in activeThreads.Keys)
            {
                activeThreads[threadID].Abort();
                activeTimers[threadID].Dispose();
            }
            activeTimers.Clear();
            activeThreads.Clear();
            activeNames.Clear();
        }

        protected void TimerCallBack(object threadIdArg)
        {
            int threadId = (int)threadIdArg;
            if (activeThreads.ContainsKey(threadId))
            {
                Log(testName + ": " + activeNames[threadId] + " failed to finish on time");
                activeThreads[threadId].Abort();
                KillTimer(threadId, false);
            }
            else
            {
                CheckIfFinished();
            }
        }

        /// <summary>
        /// Called by a test when finishing
        /// </summary>
        /// <param name="threadIdArg"></param>
        protected void KillTimer(int threadIdArg, bool success)
        {
            lock (activeThreads)
            {
                activeThreads.Remove(threadIdArg);
                activeTimers[threadIdArg].Dispose();
                activeTimers.Remove(threadIdArg);

                if (success)
                {
                    successes++;
                    if (verbose)
                    {
                        Log(activeNames[threadIdArg] + " succeeded");
                    }
                }
                else
                {
                    Log(activeNames[threadIdArg] + " failed for " + testName);
                }

                CheckIfFinished();
            }
        }

        protected void CheckIfFinished()
        {
            if (activeTimers.Count == 0)
            {
                Log(testName + " passed " + successes + " / " + numberOfTests + " tests");
            }
            else if (verbose)
            {
                Log("Remaining tests: " + activeTimers.Count);
            }
        }

        public virtual void RunTests(bool verbose, int timeoutTime = 2500)
        {
            this.verbose = verbose;

            if (verbose)
            {
                Log("Running tests for " + testName);
            }

            // Clear any previous test results
            foreach (int threadID in activeThreads.Keys)
            {
                activeThreads[threadID].Abort();
                activeTimers[threadID].Dispose();
            }
            activeTimers.Clear();
            activeThreads.Clear();
            activeNames.Clear();

            // Search through all methods of the test type and get any with the run test attribute and runTest = true
            foreach (MethodInfo method in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                // Check if the method has the RunTestAttribute
                foreach (Attribute a in method.GetCustomAttributes())
                {
                    if (a is RunTestAttribute)
                    {
                        // Check if the test should be run
                        if (((RunTestAttribute)a).RunTest == true)
                        {
                            if (verbose)
                            {
                                Log("Running test: " + method.Name);
                            }

                            numberOfTests++;

                            TestWorker worker = new TestWorker(method, this, ((RunTestAttribute)a).TestArgs);
                            worker.FinishedCallBack = new KillTimerDelegate(KillTimer);
                            Thread thread = new Thread(worker.DoWork);
                            activeThreads.Add(thread.ManagedThreadId, thread);
                            thread.IsBackground = true;
                            Timer timer = new Timer(TimerCallBack, thread.ManagedThreadId, timeoutTime, timeoutTime);
                            activeTimers.Add(thread.ManagedThreadId, timer);
                            activeNames.Add(thread.ManagedThreadId, method.Name);
                        }
                        else if (verbose)
                        {
                            Log("Skipping test: " + method.Name);
                        }

                        break;
                    }
                }
            }
            if (verbose)
            {
                Log("Finished building tests: " + activeThreads.Count);
            }

            // Get a list of all keys before starting the tests because some of the
            // tests may exit quickly and be removed from the dictionary before all
            // are started
            var keys = activeThreads.Keys;
            List<int> ks = new List<int>();
            foreach (int t in keys)
            {
                ks.Add(t);
            }
            foreach (int t in ks)
            {
                activeThreads[t].Start();
            }
            if (verbose)
            {
                Log("Finished starting all tests");
            }

            //Timer end = new Timer(TimerCallBack, 0, 5000, 5000);
        }

        protected void Log(string log)
        {
            UnityEngine.Debug.Log(log);

            lock (logFilePath)
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(log);
                    if (!log.EndsWith("\r") && !log.EndsWith("\n"))
                    {
                        sw.Write("\r\n");
                    }
                }
            }
        }
    }
}
