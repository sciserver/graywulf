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
        protected Guid ScheduleMirroDatabaseJob(QueueType queueType)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                SignInTestUser(context);

                // TODO: create test database just for this
                var ef = new EntityFactory(context);
                var dv = (DatabaseVersion)ef.LoadEntity("DatabaseVersion:Graywulf\\SciServer\\SkyQuery\\TEST\\HOT");

                var jf = MirrorDatabaseJobFactory.Create(context.Federation);
                var ji = jf.ScheduleAsJob(dv, true);
                
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

                WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                var ji = LoadJob(guid);
                Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
            }
        }
    }
}
