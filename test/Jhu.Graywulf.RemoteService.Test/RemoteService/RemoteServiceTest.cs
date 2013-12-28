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

                var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost);
                c.Execute();

                Assert.IsFalse(c.IsCanceled);

            }
        }

        [TestMethod]
        public void CancelRemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var c = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost);
                c.Period = 10000;

                var start = DateTime.Now;
                c.BeginExecute();

                System.Threading.Thread.Sleep(1000);
                c.Cancel();

                c.EndExecute();
                Assert.IsTrue((DateTime.Now - start).TotalMilliseconds < 5000);
                Assert.IsTrue(c.IsCanceled);
            }
        }

        [TestMethod]
        public void SessionTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var c1 = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost);
                var c2 = RemoteServiceHelper.CreateObject<ICancelableDelay>(Jhu.Graywulf.Test.Constants.Localhost);

                c2.Period = 2;
                c1.Period = 1;

                Assert.AreEqual(1, c1.Period);
                Assert.AreEqual(2, c2.Period);
            }
        }
    }
}
