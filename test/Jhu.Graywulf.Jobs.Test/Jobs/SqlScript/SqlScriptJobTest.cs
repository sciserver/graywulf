using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    [TestClass]
    public class SqlScriptJobTest : TestClassBase
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

        private Guid ScheduleSqlScriptJob(DatasetBase[] datasets, string sql, QueueType queueType)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);

                var jf = SqlScriptJobFactory.Create(context);
                var parameters = jf.CreateParameters(datasets, sql);
                var queue = GetQueueName(queueType);
                var ji = jf.ScheduleAsJob(parameters, queue, TimeSpan.Zero, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }

        private void RunScript(string sql)
        {
            DatasetBase mydb;

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);
                var udf = UserDatabaseFactory.Create(new FederationContext(context, user));
                mydb = udf.GetUserDatabases(user)[Registry.Constants.UserDbName];
            }

            RunScript(new [] { mydb }, sql, QueueType.Long, JobExecutionState.Completed, new TimeSpan(0, 1, 0));
        }

        private void RunScript(DatasetBase[] datasets, string sql, QueueType queue, JobExecutionState expectedOutcome, TimeSpan timeout)
        {
            var testname = GetTestUniqueName();

            using (SchedulerTester.Instance.GetToken())
            {
                SchedulerTester.Instance.EnsureRunning();

                var guid = ScheduleSqlScriptJob(datasets, sql, queue);

                WaitJobComplete(guid, TimeSpan.FromSeconds(10), timeout);
                var ji = LoadJob(guid);
                Assert.AreEqual(expectedOutcome, ji.JobExecutionStatus);
            }
        }

        [TestMethod]
        public void DiscoverParametersTest()
        {
            var jd = new JobDefinition();
            jd.WorkflowTypeName = Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(typeof(Jhu.Graywulf.Jobs.SqlScript.SqlScriptJob));
            jd.DiscoverWorkflowParameters();
        }

        [TestMethod]
        public void SqlScriptParametersSerializableTest()
        {
            var t = typeof(SqlScriptParameters);

            var sc = new SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void MyDbSqlScriptTest()
        {
            var sql = "SELECT COUNT(*) FROM sys.tables";

            RunScript(sql);
        }

        [TestMethod]
        public void MultistepScriptTest()
        {
            var sql = @"
SELECT COUNT(*) FROM sys.tables
GO
SELECT COUNT(*) FROM sys.tables
                ";

            RunScript(sql);
        }

        [TestMethod]
        public void CreatePrimaryKeyTest()
        {

        }

        [TestMethod]
        public void DropPrimaryKeyTest()
        {
        }
    }
}
