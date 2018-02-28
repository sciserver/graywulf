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
    public class SimpleSchedulerTest : SchedulerTestBase
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
        public void StartStopTest()
        {
            Thread.Sleep(new TimeSpan(0, 0, 20));
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void SimpleJobTest()
        {
            var guid = ScheduleTestJob(new TimeSpan(0, 0, 10), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
            Assert.AreEqual("OK", (string)job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void ExceptionTest()
        {
            var guid = ScheduleTestJob(JobType.Exception, QueueType.Long);

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            Assert.AreEqual(null, job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionTest()
        {
            var guid = ScheduleTestJob(JobType.AsyncException, QueueType.Long);

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            Assert.AreEqual(null, job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionWithRetryTest()
        {
            var guid = ScheduleTestJob(JobType.AsyncExceptionWithRetry, QueueType.Long);

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            Assert.AreEqual(null, job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void RetryWithFaultInFinallyTest()
        {
            var guid = ScheduleTestJob(JobType.RetryWithFaultInFinally, QueueType.Long);

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            Assert.AreEqual(null, job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void RetryWithFaultInCancelTest()
        {
            SchedulerTester.Instance.EnsureRunning();

            var guid = ScheduleTestJob(JobType.RetryWithFaultInCancel, QueueType.Long);

            WaitJobComplete(guid, TimeSpan.FromSeconds(10));

            var job = LoadJob(guid);

            Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            Assert.AreEqual(null, job.Parameters["Result"].Value);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncTrackingRecordTest()
        {
            var guid = ScheduleTestJob(JobType.AsyncTrackingRecord, QueueType.Long);
            WaitJobComplete(guid, TimeSpan.FromSeconds(30));
            var job = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void ParallelQueryDelayTest()
        {
            var guid = ScheduleTestJob(JobType.ParallelQueryDelay, QueueType.Long);
            WaitJobComplete(guid, TimeSpan.FromSeconds(30));
            var job = LoadJob(guid);
            Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
        }
    }
}
