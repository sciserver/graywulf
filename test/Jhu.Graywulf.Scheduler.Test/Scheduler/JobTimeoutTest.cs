using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    [TestClass]
    public class JobTimeoutTest : SchedulerTestBase
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

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AtomicJobTest()
        {
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Quick, new TimeSpan(0, 0, 15));

            WaitJobStarted(guid, TimeSpan.FromSeconds(3));

            WaitJobComplete(guid, TimeSpan.FromSeconds(3));

            var ji = LoadJob(guid);

            // TODO: It is not clear whether the job gets cancelled or timed out
            // Most of the time it gets cancelled before it could competed when
            // but this is a timing issue that needs further investigation.
            // Nevertheless, job status is accurate

            Assert.IsTrue(
                ji.JobExecutionStatus == JobExecutionState.Completed ||
                ji.JobExecutionStatus == JobExecutionState.TimedOut);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void CancelableTest()
        {
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 1, 0), JobType.CancelableDelay, QueueType.Quick, new TimeSpan(0, 0, 15));

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.TimedOut, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void QueryDelayTest()
        {
            // Job times out causing query to cancel
            SchedulerTester.Instance.EnsureRunning();

            // Delay time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 2, 0), JobType.QueryDelay, QueueType.Quick, new TimeSpan(0, 0, 30));

            WaitJobStarted(guid, TimeSpan.FromSeconds(10));
            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.TimedOut, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void QueryDelayRetryTest()
        {
            // Job times out causing query to cancel
            SchedulerTester.Instance.EnsureRunning();

            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 1, 0), JobType.QueryDelayRetry, QueueType.Quick, new TimeSpan(0, 0, 15));

            WaitJobStarted(guid, TimeSpan.FromSeconds(3));

            WaitJobComplete(guid, TimeSpan.FromSeconds(3));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.TimedOut, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void QueryTimeoutTest()
        {
            // Query times out causing job to cancel.
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 1, 0), JobType.QueryTimeout, QueueType.Quick, new TimeSpan(0, 0, 15));

            WaitJobStarted(guid, TimeSpan.FromSeconds(10));

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Failed, ji.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void QueryTimeoutRetryTest()
        {
            // Query times out causing job to cancel.
            SchedulerTester.Instance.EnsureRunning();
            
            // Time must be longer than the time-out of quick queue!
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 10), JobType.QueryTimeoutRetry, QueueType.Quick, new TimeSpan(0, 0, 50));

            WaitJobStarted(guid, TimeSpan.FromSeconds(3));
            WaitJobComplete(guid, TimeSpan.FromSeconds(3));

            var ji = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.TimedOut, ji.JobExecutionStatus);
        }
    }
}
