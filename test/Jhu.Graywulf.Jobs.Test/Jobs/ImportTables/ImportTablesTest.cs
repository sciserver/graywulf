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
                //var user = SignInTestUser(context);
                //var mydbds = context.Federation.MyDBDatabaseVersion.GetUserDatabaseInstance(user).GetDataset();



                var destination = new DestinationTable()
                {
                    Dataset = IOTestDataset,
                    DatabaseName = IOTestDataset.DatabaseName,
                    SchemaName = IOTestDataset.DefaultSchemaName,
                    TableNamePattern = tableName
                };

                var parameters = new ImportTablesParameters()
                {
                    Uri = Util.UriConverter.FromFilePath(path),
                    Destinations = new DestinationTable[] { destination },
                    FileFormatFactoryType = context.Federation.FileFormatFactory,
                    StreamFactoryType = context.Federation.StreamFactory,
                };

                var etf = ImportTablesJobFactory.Create(context.Federation);
                var ji = etf.ScheduleAsJob(parameters, queue, "comments");

                ji.Save();

                return ji.Guid;
            }
        }

        [TestMethod]
        public void ImportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Jobs.ImportTables.ImportTablesParameters);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ImportTablesXmlTest()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var path = String.Format(@"\\{0}\{1}.csv.zip", Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);
                var guid = ScheduleImportTableJob("SampleData", path, QueueType.Long);

                var ji = LoadJob(guid);

                var xml = ji.Parameters["Parameters"].XmlValue;
            }
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

                    var path = String.Format(@"\\{0}\{1}\SimpleExportTest.csv.zip",
                        Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);
                    var guid = ScheduleImportTableJob(path, "SampleData", QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    var uri = ((ExportTablesParameters)ji.Parameters["Parameters"].Value).Uri;

                    
                }
            }
        }
    }
}
