﻿using System;
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
            var queue = String.Format("Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);

                var ef = new EntityFactory(context);
                var f = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);

                var format = FileFormatFactory.Create(null).GetFileFormatDescription(typeof(Jhu.Graywulf.Format.DelimitedTextDataFile).FullName);

                var source = new Jhu.Graywulf.Schema.Table()
                {
                    Dataset = user.GetUserDatabaseInstance(f.MyDBDatabaseVersion).GetDataset(),  // TODO: fix this
                    SchemaName = schemaName,
                    TableName = tableName
                };

                var etf = new ExportTablesFactory(context);
                var ji = etf.ScheduleAsJob(
                    new TableOrView[] {source },
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
            var t = typeof(Jhu.Graywulf.Jobs.ExportTables.ExportTables);

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

                    var path = String.Format(@"\\{0}\{1}", Jhu.Graywulf.Test.Constants.Localhost, Jhu.Graywulf.Test.Constants.TestDirectory);
                    var guid = ScheduleExportTableJob("dbo", "testtable", path, QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);

                    var uri = ((ExportTables)ji.Parameters["Parameters"].GetValue()).Uri;

                    path = Util.UriConverter.ToFilePath(uri);

                    Assert.IsTrue(File.Exists(path));
                    File.Delete(path);
                }
            }
        }
    }
}
