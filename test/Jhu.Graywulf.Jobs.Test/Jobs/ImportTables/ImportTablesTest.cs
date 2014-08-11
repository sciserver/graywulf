using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    [TestClass]
    public class ImportTablesTest : TestClassBase
    {
        protected Guid ScheduleImportTableJob(string path, string tableName, QueueType queueType)
        {
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);
                var federationContext = new FederationContext(context, user);

                var ds = federationContext.MyDBDataset;

                var destination = new DestinationTable()
                {
                    Dataset = ds,
                    DatabaseName = ds.DatabaseName,
                    SchemaName = ds.DefaultSchemaName,
                    TableNamePattern = tableName
                };

                var itf = ImportTablesJobFactory.Create(context.Federation);
                
                var parameters = itf.CreateParameters(
                    context.Federation,
                    Util.UriConverter.FromFilePath(path),
                    destination);

                var ji = itf.ScheduleAsJob(parameters, queue, "comments");

                ji.Save();

                return ji.Guid;
            }
        }

        private void DropTestTables(string name)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);
                var federationContext = new FederationContext(context, user);

                var ds = federationContext.MyDBDataset;

                ds.Tables.LoadAll();
                foreach (var t in ds.Tables.Values)
                {
                    if (t.ObjectName.StartsWith(name))
                    {
                        t.Drop();
                    }
                }
            }
        }

        [TestMethod]
        public void ImportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Jobs.ImportTables.ImportTablesParameters);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        /// <summary>
        /// This tests attempts to export the table 'testtable' from the myDB of user 'test'.
        /// Create table manually if test fails.
        /// </summary>
        [TestMethod]
        public void SimpleImportJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var path = @"..\..\..\graywulf\test\files\csv_numbers.csv";

                    var guid = ScheduleImportTableJob(path, "SimpleImportJobTest_[$BatchName]_[$ResultsetName]", QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    DropTestTables("SimpleImportJobTest");
                }
            }
        }

        [TestMethod]
        public void ImportArchiveJobTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var path = @"..\..\..\graywulf\test\files\archive.zip";

                    var guid = ScheduleImportTableJob(path, "ImportArchiveJobTest_[$BatchName]_[$ResultsetName]", QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    DropTestTables("ImportArchiveJobTest");
                }
            }
        }
    }
}
