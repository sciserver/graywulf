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
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    [TestClass]
    public class ExportTablesTest : TestClassBase
    {
        protected Guid ScheduleExportTableJob(string schemaName, string tableName, string path, QueueType queueType)
        {
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);

                var ef = new EntityFactory(context);
                var federation = ef.LoadEntity<Federation>(Registry.AppSettings.FederationName);

                // TODO: delete after testing
                //var mydbds = federation.UserDatabaseVersion.GetUserDatabaseInstance(user).GetDataset();

                var udf = UserDatabaseFactory.Create(federation);
                var mydbds = udf.GetUserDatabase(user);

                var source = new Jhu.Graywulf.Schema.Table()
                {
                    Dataset = IOTestDataset,
                    DatabaseName = IOTestDataset.DatabaseName,
                    SchemaName = schemaName,
                    TableName = tableName
                };


                // Set the file name (within the archive)
                var destination = new Jhu.Graywulf.Format.DelimitedTextDataFile();
                destination.Uri = Util.UriConverter.FromFilePath(tableName + destination.Description.Extension);

                var parameters = new ExportTablesParameters()
                {
                    Sources = new TableOrView[] { source },
                    Destinations = new DataFileBase[] { destination },
                    Uri = Util.UriConverter.FromFilePath(path),
                };

                var etf = ExportTablesJobFactory.Create(context.Federation);
                var ji = etf.ScheduleAsJob(parameters, queue, "");

                ji.Save();

                return ji.Guid;
            }
        }

        [TestMethod]
        public void ExportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Jobs.ExportTables.ExportTablesParameters);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ExportTablesXmlTest()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var path = String.Format(@"\\{0}\{1}.csv.zip", Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);
                var guid = ScheduleExportTableJob("dbo", "SampleData", path, QueueType.Long);

                var ji = LoadJob(guid);

                var xml = ji.Parameters["Parameters"].XmlValue;
            }
        }

        /// <summary>
        /// This tests attempts to export the table 'testtable' from the myDB of user 'test'.
        /// Create table manually if test fails.
        /// </summary>
        [TestMethod]
        public void SimpleExportTest()
        {
            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var path = String.Format(@"\\{0}\{1}\SimpleExportTest.csv.zip",
                        Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);
                    var guid = ScheduleExportTableJob("dbo", "SampleData", path, QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    var uri = ((ExportTablesParameters)ji.Parameters["Parameters"].Value).Uri;

                    path = Util.UriConverter.ToFilePath(uri);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }
    }
}
