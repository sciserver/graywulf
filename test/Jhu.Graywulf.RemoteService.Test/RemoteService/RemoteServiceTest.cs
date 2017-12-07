using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
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

                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    c.Value.Execute();
                    Assert.IsFalse(c.Value.IsCanceled);
                }
            }
        }

        [TestMethod]
        public void CancelRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    c.Value.Period = 10000;

                    var start = DateTime.Now;
                    c.Value.BeginExecute();

                    System.Threading.Thread.Sleep(1000);
                    c.Value.Cancel();

                    c.Value.EndExecute();
                    Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                    Assert.IsTrue(c.Value.IsCanceled);
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

                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    c.Value.Period = 1000;
                    c.Value.ThrowException = true;

                    var start = DateTime.Now;
                    c.Value.BeginExecute();

                    System.Threading.Thread.Sleep(2000);
                    c.Value.Cancel();

                    c.Value.EndExecute();
                }
            }
        }

        [TestMethod]
        public void CancelFailedRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                    {
                    c.Value.Period = 1000;
                    c.Value.ThrowException = true;

                    c.Value.BeginExecute();
                    System.Threading.Thread.Sleep(2000);

                    try
                    {
                        c.Value.EndExecute();
                        Assert.Fail();
                    }
                    catch (Exception ex)
                    {
                    }

                    System.Threading.Thread.Sleep(2000);
                    c.Value.Cancel();
                }
            }
        }

        [TestMethod]
        public void SessionTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
                {
                    using (var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost, allowInProc))
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
