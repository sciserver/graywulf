using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Registry;


namespace Jhu.Graywulf.Scheduler
{
    [TestClass]
    public class SchedulerControlTest : SchedulerTestBase
    {
        private ISchedulerControl GetControl()
        {
            return ServiceHelper.CreateChannel<ISchedulerControl>(DnsHelper.Localhost, "Control", Scheduler.Configuration.Endpoint);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallHelloTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var control = GetControl();

                control.Hello();
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallWhoAmITest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var control = GetControl();

                string name, authtype;
                bool isauth;

                control.WhoAmI(out name, out isauth, out authtype);

                // get current identity
                var id = WindowsIdentity.GetCurrent();

                Assert.AreEqual(id.Name, name);
                Assert.AreEqual(id.IsAuthenticated, isauth);
                Assert.AreEqual(id.AuthenticationType, authtype);

            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallWhoAreYouTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var control = GetControl();

                string name, authtype;
                bool isauth;

                control.WhoAreYou(out name, out isauth, out authtype);

                // get current identity
                var id = WindowsIdentity.GetCurrent();

                Assert.AreEqual(id.Name, name);
                Assert.AreEqual(id.IsAuthenticated, isauth);
                Assert.AreEqual(id.AuthenticationType, authtype);

            }
        }
    }
}
