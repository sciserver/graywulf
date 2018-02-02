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
        public async Task RemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        await c.Value.ExecuteAsync();
                        Assert.IsFalse(await c.Value.IsCancellationRequestedAsync());
                    }
                }
            }
        }

        [TestMethod]
        public async Task MultipleRemoteExecuteTest()
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

                        Assert.IsFalse(await c.Value.IsCancellationRequestedAsync());
                    }
                }
            }
        }

        [TestMethod]
        public async Task CancelRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    var start = DateTime.Now;

                    new Task(() =>
                    {
                        System.Threading.Thread.Sleep(1000);
                        c.Value.CancelAsync();
                    }).Start();
                    
                    await c.Value.ExecuteAsync(10000, false);

                    Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                    Assert.IsTrue(await c.Value.IsCancelledAsync());
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>))]
        public async Task CancelFailingRemoteExecuteTest()
        {
            // TODO: If it is cancelled, it won't throw the exception

            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                        var start = DateTime.Now;

                        new Task(() =>
                        {
                            System.Threading.Thread.Sleep(1000);
                            c.Value.CancelAsync();
                        }).Start();

                        await c.Value.ExecuteAsync(10000, true);
                        
                        Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                        Assert.IsTrue(await c.Value.IsCancellationRequestedAsync());
                        Assert.IsTrue(await c.Value.IsCancelledAsync());
                    }
                }
            }
        }

        [TestMethod]
        public async Task CancelProcessInterleavedTest()
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
                        new Task(() =>
                        {
                            System.Threading.Thread.Sleep(1000);
                            c1.Value.CancelAsync();
                        }).Start();

                        await c1.Value.ExecuteAsync(10000, false);

                        using (var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(cancellationContext, Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                        {
                            new Task(() =>
                            {
                                System.Threading.Thread.Sleep(1000);
                                c2.Value.CancelAsync();
                            }).Start();

                            await c2.Value.ExecuteAsync(10000, false);
                            
                            Assert.IsTrue(await c1.Value.IsCancelledAsync());
                            Assert.IsTrue(await c2.Value.IsCancelledAsync());
                        }
                    }
                }
            }
        }
    }
}
