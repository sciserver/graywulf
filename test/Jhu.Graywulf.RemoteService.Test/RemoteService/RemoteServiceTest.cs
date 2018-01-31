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
    public class RemoteServiceTest : Jhu.Graywulf.Test.LoggingTestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();

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

            StopLogger();
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
                    using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        c.Value.ExecuteAsync().Wait();

                        Assert.IsFalse(c.Value.IsCancellationRequested);
                    }
                }
            }
        }

        [TestMethod]
        public void MultipleRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        c.Value.ExecuteAsync().Wait();
                        c.Value.ExecuteAsync().Wait();
                        c.Value.ExecuteAsync().Wait();

                        Assert.IsFalse(c.Value.IsCancellationRequested);
                    }
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
                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    c.Value.Period = 10000;

                    var start = DateTime.Now;
                    var task = c.Value.ExecuteAsync();

                    System.Threading.Thread.Sleep(1000);
                    c.Value.Cancel();

                    task.Wait();
                    Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                    Assert.IsTrue(c.Value.IsCancelled);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>))]
        public void CancelFailingRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        c.Value.Period = 10000;

                        var start = DateTime.Now;
                        var task = c.Value.ExecuteAsync();

                        Thread.Sleep(1000);
                        c.Value.Cancel();

                        Util.TaskHelper.Wait(task);

                        Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                        Assert.IsTrue(c.Value.IsCancellationRequested);
                        Assert.IsTrue(c.Value.IsCancelled);
                    }
                }
            }
        }

        [TestMethod]
        public void CancelProcessInterleavedTest()
        {
            // Here we test that the two remote calls are directed
            // to separate service instances
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        c1.Value.Period = 10000;
                        var task1 = c1.Value.ExecuteAsync();

                        Thread.Sleep(1000);

                        using (var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                        {
                            c2.Value.Period = 10000;
                            var task2 = c2.Value.ExecuteAsync();

                            c1.Value.Cancel();

                            Thread.Sleep(1000);

                            c2.Value.Cancel();

                            Util.TaskHelper.Wait(task1);
                            Util.TaskHelper.Wait(task2);

                            Assert.IsTrue(c1.Value.IsCancelled);
                            Assert.IsTrue(c2.Value.IsCancelled);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CancelFailedRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        using (var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                        {
                            c2.Value.Period = 2;
                            c1.Value.Period = 1;

                            Assert.AreEqual(1, c1.Value.Period);
                            Assert.AreEqual(2, c2.Value.Period);
                        }
                    }
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
                    using (var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        using (var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                        {
                            c2.Value.Period = 2;
                            c1.Value.Period = 1;

                            Assert.AreEqual(1, c1.Value.Period);
                            Assert.AreEqual(2, c2.Value.Period);
                        }
                    }
                }
            }
        }
    }
}
