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
    public class SchedulerTest : SchedulerTestBase
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

        [TestMethod]
        [TestCategory("Scheduler")]
        public void StartStopTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();
                Thread.Sleep(new TimeSpan(0, 0, 20));
                SchedulerTester.Instance.DrainStop();
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void SimpleJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(new TimeSpan(0, 0, 10), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
                Assert.AreEqual("OK", (string)job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void ManySimpleJobsTest()
        {
            PurgeTestJobs();

            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                Parallel.For(0, 20, i =>
                {
                    var guid = ScheduleTestJob(new TimeSpan(0, 0, 1), JobType.AtomicDelay, QueueType.Quick, new TimeSpan(0, 2, 0));

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var job = LoadJob(guid);

                    Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
                    Assert.AreEqual("OK", (string)job.Parameters["Result"].Value);
                });
            }
        }


        [TestMethod]
        [TestCategory("Scheduler")]
        public void CancelAtomicJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(new TimeSpan(0, 0, 30), JobType.AtomicDelay, QueueType.Long, new TimeSpan(0, 2, 0));

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                CancelJob(guid);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void CancelCancelableJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Make delay long enough but cancelable
                var guid = ScheduleTestJob(new TimeSpan(0, 10, 0), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                CancelJob(guid);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void KillSchedulerTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                // Make delay long enough but cancelable
                var guid = ScheduleTestJob(new TimeSpan(0, 2, 0), JobType.CancelableDelay, QueueType.Long, new TimeSpan(0, 2, 0));

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                SchedulerTester.Instance.Kill();

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                // This will complete because cancellation is not possible
                Assert.AreEqual(JobExecutionState.Cancelled, ji.JobExecutionStatus);
            }
        }


        [TestMethod]
        [TestCategory("Scheduler")]
        public void PersistAndResumeJobTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

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
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void PersistAndCancelJobTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

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
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void ExceptionTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.Exception, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
                Assert.AreEqual(null, job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.AsyncException, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
                Assert.AreEqual(null, job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionWithRetryTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.AsyncExceptionWithRetry, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
                Assert.AreEqual(null, job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void RetryWithFaultInFinallyTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.RetryWithFaultInFinally, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
                Assert.AreEqual(null, job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void RetryWithFaultInCancelTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.RetryWithFaultInCancel, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
                Assert.AreEqual(null, job.Parameters["Result"].Value);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncExceptionWithStopTest()
        {
            // TODO: this workflow fails too quickly to get persisted.
            // needs more testing but it's not a major issue

            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleTestJob(JobType.AsyncExceptionWithRetry, QueueType.Long);

                WaitJobStarted(guid, TimeSpan.FromSeconds(10));

                SchedulerTester.Instance.Stop();

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Persisted, job.JobExecutionStatus);

                SchedulerTester.Instance.EnsureRunning();

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Failed, job.JobExecutionStatus);
            }
        }

        [TestMethod]
        [TestCategory("Scheduler")]
        public void AsyncTrackingRecordTest()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                SchedulerTester.Instance.EnsureRunning(false);

                var guid = ScheduleTestJob(JobType.AsyncTrackingRecord, QueueType.Long);

                WaitJobComplete(guid, TimeSpan.FromSeconds(30));

                var job = LoadJob(guid);

                Assert.AreEqual(JobExecutionState.Completed, job.JobExecutionStatus);
            }
        }
    }
}
