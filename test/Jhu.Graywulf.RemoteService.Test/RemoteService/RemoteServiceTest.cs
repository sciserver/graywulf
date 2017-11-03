using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.RemoteService.Server;

namespace Jhu.Graywulf.RemoteService
{
    [TestClass]
    public class RemoteServiceTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();
            }
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.Stop();
            }
        }

        // TODO: these tests now run in in-proc mode because authentication against localhost is problematic

        private const bool allowInProc = false;

        [TestMethod]
        public void StartStopTest()
        {
            using (RemoteServiceTester.Instance.GetExclusiveToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();
                RemoteServiceTester.Instance.Stop();
            }
        }

        [TestMethod]
        public void RemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);
                    c.ExecuteAsync().Wait();

                    Assert.IsFalse(c.IsCancellationRequested);
                }
            }
        }
        
        [TestMethod]
        public void CancelRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);
                    c.Period = 10000;

                    var start = DateTime.Now;
                    var task = c.ExecuteAsync();

                    Thread.Sleep(1000);
                    c.Cancel();

                    task.Wait();

                    Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                    Assert.IsTrue(c.IsCancellationRequested);
                }
            }
        }

        [TestMethod]
        public void SessionTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);
                    var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);

                    c2.Period = 2;
                    c1.Period = 1;

                    Assert.AreEqual(1, c1.Period);
                    Assert.AreEqual(2, c2.Period);
                }
            }
        }
    }
}
