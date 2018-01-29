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
using Jhu.Graywulf.IO.Jobs.ExportTables;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.IO.Jobs.CopyTables
{
    [TestClass]
    public class CopyTablesTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            StopLogger();
        }

        protected Guid ScheduleCopyTablesJob(string sourceTableName, string destinationTableName, bool dropSourceTable, QueueType queueType)
        {
            var queue = GetQueueName(queueType);

            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                var user = SignInTestUser(context);

                var ef = new EntityFactory(context);
                var udf = UserDatabaseFactory.Create(new FederationContext(context, user));
                var mydbds = udf.GetUserDatabases(user)[Registry.Constants.UserDbName];

                var sourcetable = new Table()
                {
                    Dataset = IOTestDataset,
                    DatabaseName = IOTestDataset.DatabaseName,
                    SchemaName = IOTestDataset.DefaultSchemaName,
                    TableName = sourceTableName
                };

                var source = SourceTable.Create(sourcetable);

                var destinationtable = new Table()
                {
                    Dataset = mydbds,
                    DatabaseName = mydbds.DatabaseName,
                    SchemaName = mydbds.DefaultSchemaName,
                    TableName = destinationTableName,
                };

                var destination = new DestinationTable(destinationtable, TableInitializationOptions.Create | TableInitializationOptions.CreatePrimaryKey);

                var parameters = new CopyTablesParameters()
                {
                    Items = new CopyTablesItem[]
                    {
                        new CopyTablesItem()
                        {
                            Source = source,
                            Destination = destination,
                            DropSourceTable = dropSourceTable
                        }
                    }
                };

                var ctf = CopyTablesJobFactory.Create(context);
                var ji = ctf.ScheduleAsJob(parameters, queue, TimeSpan.Zero, "");

                ji.Save();

                return ji.Guid;
            }
        }

        [TestMethod]
        public void CopyTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Jobs.CopyTables.CopyTablesParameters);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void CopyTablesXmlTest()
        {
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                var name = GetTestUniqueName();
                var guid = ScheduleCopyTablesJob("SampleData", name, false, QueueType.Long);
                var ji = LoadJob(guid);
                var xml = ji.Parameters["Parameters"].XmlValue;
            }
        }

        /// <summary>
        /// This tests attempts to export the table 'testtable' from the myDB of user 'test'.
        /// Create table manually if test fails.
        /// </summary>
        [TestMethod]
        public void SimpleCopyTableTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var name = GetTestUniqueName();
                    DropUserDatabaseTable(name);

                    var guid = ScheduleCopyTablesJob(
                        "SampleData_PrimaryKey",
                        name,
                        false,
                        QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                }
            }
        }

    }
}
