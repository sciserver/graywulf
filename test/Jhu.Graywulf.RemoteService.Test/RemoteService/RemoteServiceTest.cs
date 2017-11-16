using System;
using System.Threading;
using System.Threading.Tasks;
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
        [ExpectedException(typeof(TaskCanceledException))]
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

                    Util.TaskHelper.Wait(task);

                    Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                    Assert.IsTrue(c.IsCancellationRequested);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>))]
        public void CancelProcessInterleavedTest()
        {
            // Here we test that the two remote calls are directed
            // to separate service instances
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);
                    c1.Period = 10000;
                    var task1 = c1.ExecuteAsync();

                    Thread.Sleep(1000);

                    var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc);
                    c2.Period = 10000;
                    var task2 = c2.ExecuteAsync();

                    c1.Cancel();

                    Thread.Sleep(1000);

                    c2.Cancel();

                    Util.TaskHelper.Wait(task1);
                    Util.TaskHelper.Wait(task2);
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
