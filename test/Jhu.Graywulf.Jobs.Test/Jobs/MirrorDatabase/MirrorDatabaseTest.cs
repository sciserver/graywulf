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
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                SignInTestUser(context);

                var ef = new EntityFactory(context);
                var jd = ef.LoadEntity<JobDefinition>(Registry.AppSettings.ClusterName, Registry.Constants.SystemDomainName, Registry.Constants.SystemFederationName, typeof(MirrorDatabaseJob).Name);

                var ji = jd.CreateJobInstance(queue, ScheduleType.Queued);

                ji.Parameters["DatabaseVersionName"].Value = "DatabaseVersion:Graywulf.VOServices.SkyQuery.Galex.STAT";

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
