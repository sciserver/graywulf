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

        private ServiceProxy<ISchedulerControl> GetControl()
        {
            return ServiceHelper.CreateChannel<ISchedulerControl>(DnsHelper.Localhost, "Control", Scheduler.Configuration.Endpoint, TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallHelloTest()
        {
            using (var control = GetControl())
            {
                control.Value.Hello();
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Fast")]
        public void CallWhoAmITest()
        {
            using (var control = GetControl())
            {

                string name, authtype;
                bool isauth;

                control.Value.WhoAmI(out name, out isauth, out authtype);

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
            using (var control = GetControl())
            {
                string name, authtype;
                bool isauth;

                control.Value.WhoAreYou(out name, out isauth, out authtype);

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
        public void GetQueuesTest()
        {
            using (var control = GetControl())
            {
                var queues = control.Value.GetQueues();
                Assert.IsTrue(queues.Length > 0);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void GetJobsTest()
        {
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(5));

            using (var control = GetControl())
            {
                var queues = control.Value.GetQueues();
                int cnt = 0;

                foreach (var q in queues)
                {
                    cnt += control.Value.GetJobs(q.Guid).Length;
                }

                Assert.IsTrue(cnt > 0);

                var job = control.Value.GetJob(guid);
                Assert.AreEqual(JobStatus.Executing, job.Status);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        [TestCategory("Slow")]
        public void StartAndCancelJobTest()
        {
            // TODO: this test fails because the scheduler takes quite a bit of time
            // to start. Add delay or logic in TestInitialize to wait for the control service
            // to start.

            using (var control = GetControl())
            {
                var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));
                control.Value.StartJob(guid);

                var job = control.Value.GetJob(guid);
                Assert.AreEqual(JobStatus.Executing, job.Status);

                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));

                control.Value.CancelJob(guid);
                WaitJobStarted(guid, TimeSpan.FromSeconds(5));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }
    }
}
