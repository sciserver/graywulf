using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    [TestClass]
    public class InterruptSchedulerTest : SchedulerTestBase
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
            TestInitialize(false, 2);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void CancelAtomicJobTest()
        {
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(10));
            CancelJob(guid);
            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void CancelCancelableJobTest()
        {
            // Make delay long enough but cancelable
            var guid = ScheduleTestJob(new TimeSpan(0, 10, 0), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(10));
            CancelJob(guid);
            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void KillSchedulerTest()
        {
            // Make delay long enough but cancelable
            var guid = ScheduleTestJob(new TimeSpan(0, 2, 0), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(10));
            SchedulerTester.Instance.Kill();
            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            var ji = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AbortJobsTest()
        {
            // Long job with atomic delay, jobs must be aborted for scheduler to quit
            var guid = ScheduleTestJob(new TimeSpan(0, 10, 0), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 10, 0));
            WaitJobStarted(guid, TimeSpan.FromSeconds(10));
            SchedulerTester.Instance.Stop();
            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            var ji = LoadJob(guid);
            
            Assert.AreEqual(JobExecutionState.Failed, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void PersistAndResumeJobTest()
        {
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.MultipleDelay, QueueType.Long, new TimeSpan(0, 2, 0));

            WaitJobStarted(guid, TimeSpan.FromSeconds(10));

            // Suspend workflows
            SchedulerTester.Instance.Stop();

            // Leave enough time to suspend
            Thread.Sleep(new TimeSpan(0, 0, 30));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Persisted, ji.JobExecutionStatus);

            SchedulerTester.Instance.EnsureRunning();

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
            Assert.AreEqual("OK", (string)ji.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void PersistAndCancelJobTest()
        {
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.MultipleDelay, QueueType.Long, new TimeSpan(0, 2, 0));

            WaitJobStarted(guid, TimeSpan.FromSeconds(10));

            // Suspend workflows
            SchedulerTester.Instance.Stop();

            // Leave enough time to suspend
            Thread.Sleep(new TimeSpan(0, 0, 30));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Persisted, ji.JobExecutionStatus);

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                ji.RegistryContext = context;
                ji.Cancel();
            }

            ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Persisted | JobExecutionState.CancelRequested, ji.JobExecutionStatus);

            SchedulerTester.Instance.EnsureRunning();

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            ji = LoadJob(guid);
            Assert.IsTrue(
                ji.JobExecutionStatus == JobExecutionState.Cancelled ||
                ji.JobExecutionStatus == JobExecutionState.Completed);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionWithStopTest()
        {
            // TODO: this workflow fails too quickly to get persisted.
            // needs more testing but it's not a major issue
            var guid = ScheduleTestJob(JobType.AsyncExceptionWithRetry, QueueType.Long);
            WaitJobStarted(guid, TimeSpan.FromSeconds(10));

            // Leave enough time to run
            Thread.Sleep(new TimeSpan(0, 0, 10));

            SchedulerTester.Instance.Stop();

            var job = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Persisted, job.JobExecutionStatus);

            SchedulerTester.Instance.EnsureRunning();

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));
            job = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
        }
    }
}
