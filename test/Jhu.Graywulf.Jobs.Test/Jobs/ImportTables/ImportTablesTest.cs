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
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            InitializeJobTests();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            CleanupJobTests();
        }

        protected Guid ScheduleImportTableJob(string path, string tableName, QueueType queueType)
        {
            var queue = GetQueueName(queueType);

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
                    TableNamePattern = tableName,
                    Options = TableInitializationOptions.Drop | TableInitializationOptions.Create
                };

                var itf = ImportTablesJobFactory.Create(context.Federation);
                
                var parameters = itf.CreateParameters(
                    context.Federation,
                    Util.UriConverter.FromFilePath(path),
                    null,
                    null,
                    destination);

                var ji = itf.ScheduleAsJob(parameters, queue, TimeSpan.Zero, "comments");

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

                ds.Tables.LoadAll(true);
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
                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var path = String.Format(@"\\{0}\{1}\files\csv_numbers.csv",
                        Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);

                    var guid = ScheduleImportTableJob(path, "SimpleImportJobTest_" + IO.Constants.ResultsetNameToken, QueueType.Long);

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
            DropUserDatabaseTable("ImportArchiveJobTest");

            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var path = String.Format(@"\\{0}\{1}\files\archive.zip", Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);

                    var guid = ScheduleImportTableJob(path, "ImportArchiveJobTest_" + IO.Constants.ResultsetNameToken, QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                }
            }
        }
    }
}
