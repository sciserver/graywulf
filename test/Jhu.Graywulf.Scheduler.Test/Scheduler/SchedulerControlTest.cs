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
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ClassInitialize();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            ClassCleanUp();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestInitialize(false, 1);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        private ISchedulerControl GetControl()
        {
            return ServiceHelper.CreateChannel<ISchedulerControl>(DnsHelper.Localhost, "Control", Scheduler.Configuration.Endpoint);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallHelloTest()
        {
            var control = GetControl();
            control.Hello();
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallWhoAmITest()
        {
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

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallWhoAreYouTest()
        {
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

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void GetQueuesTest()
        {
            var control = GetControl();
            var queues = control.GetQueues();
            Assert.IsTrue(queues.Length > 0);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void GetJobsTest()
        {
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(5));

            var control = GetControl();
            var queues = control.GetQueues();
            int cnt = 0;

            foreach (var q in queues)
            {
                cnt += control.GetJobs(q.Guid).Length;
            }

            Assert.IsTrue(cnt > 0);

            var job = control.GetJob(guid);
            Assert.AreEqual(JobStatus.Executing, job.Status);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void StartAndCancelJobTest()
        {
            var control = GetControl();

            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            control.StartJob(guid);

            var job = control.GetJob(guid);
            Assert.AreEqual(JobStatus.Executing, job.Status);

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));

            control.CancelJob(guid);
            WaitJobStarted(guid, TimeSpan.FromSeconds(5));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
        }
    }
}
