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
using Jhu.Graywulf.Jobs.ExportTable;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Jobs.ExportTable
{
    [TestClass]
    public class ExportTableTest : TestClassBase
    {
        protected Guid ScheduleExportTableJob(string schemaName, string tableName, string path, QueueType queueType)
        {
            var queue = String.Format("Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);

                var ef = new EntityFactory(context);
                var f = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);

                var format = new FileFormatFactory().GetFileFormatDescription("Jhu.Graywulf.Format.CsvFile");

                var source = new Jhu.Graywulf.Schema.Table()
                {
                    Dataset = user.GetUserDatabaseInstance(f.MyDBDatabaseVersion).GetDataset(),  // TODO: fix this
                    SchemaName = schemaName,
                    TableName = tableName
                };

                var etf = new ExportTableFactory(context);
                var ji = etf.ScheduleAsJob(
                    source,
                    path,
                    format,
                    queue,
                    "");

                ji.Save();

                return ji.Guid;
            }
        }

        [TestMethod]
        public void ExportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Jobs.ExportTable.ExportTable);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
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

                    var path = String.Format(@"\\{0}\{1}", Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.GWCode);
                    var guid = ScheduleExportTableJob("dbo", "testtable", path, QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    var file = ((ExportTable)ji.Parameters["Parameters"].GetValue()).Destination.Path;

                    Assert.IsTrue(File.Exists(file));

                    File.Delete(file);
                }
            }
        }
    }
}
