using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void CallHelloTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var control = GetControl();

                control.Hello();
            }
        }
    }
}
