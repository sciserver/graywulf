using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.MirrorDatabase;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    [TestClass]
    public class MirrorDatabaseTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                PurgeTestJobs();
            }
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                if (SchedulerTester.Instance.IsRunning)
                {
                    SchedulerTester.Instance.DrainStop();
                }

                PurgeTestJobs();
            }
        }

        protected Guid ScheduleMirroDatabaseJob(QueueType queueType)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                SignInTestUser(context);

                // TODO: create test database just for this
                var ef = new EntityFactory(context);
                var databaseVersion = (DatabaseVersion)ef.LoadEntity("DatabaseVersion:Graywulf\\SciServer\\SkyQuery\\TEST\\PROD");

                var jf = MirrorDatabaseJobFactory.Create(context);
                var parameters = jf.CreateParameters(databaseVersion);
                var ji = jf.ScheduleAsJob(parameters, jf.GetDefaultMaintenanceQueue(), TimeSpan.Zero, "test job");
                
                ji.Save();

                return ji.Guid;
            }
        }

        [TestMethod]
        public void MirrorDatabaseSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Jobs.MirrorDatabase.MirrorDatabaseJob);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void SimpleMirrorTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleMirroDatabaseJob(QueueType.Maintenance);

                WaitJobComplete(guid, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
            }
        }
    }
}
