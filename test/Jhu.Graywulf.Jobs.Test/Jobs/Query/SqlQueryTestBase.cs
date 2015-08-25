using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryTestBase : TestClassBase
    {
        protected virtual UserDatabaseFactory CreateUserDatabaseFactory(Context context)
        {
            return UserDatabaseFactory.Create(
                typeof(GraywulfUserDatabaseFactory).AssemblyQualifiedName,
                context.Federation);
        }

        protected virtual QueryFactory CreateQueryFactory(Context context)
        {
            var qf = QueryFactory.Create(typeof(SqlQueryFactory).AssemblyQualifiedName, context);
            return qf;
        }

        protected virtual SqlQuery CreateQuery(string query)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var qf = CreateQueryFactory(context);
                var q = CreateQuery(qf, query);

                return q;
            }
        }

        private SqlQuery CreateQuery(QueryFactory qf, string query)
        {
            var user = SignInTestUser(qf.Context);

            var udf = CreateUserDatabaseFactory(qf.Context);
            var mydb = udf.GetUserDatabase(user);
            var mysi = udf.GetUserDatabaseServerInstance(user);

            var q = qf.CreateQuery(query);
            qf.AppendUserDatabase(q, mydb, mysi);

            q.Destination = new Jhu.Graywulf.IO.Tasks.DestinationTable()
            {
                Dataset = mydb,
                DatabaseName = mydb.DatabaseName,
                SchemaName = mydb.DefaultSchemaName,
                TableNamePattern = "testtable",     // will be overwritten by INTO queries
                Options = TableInitializationOptions.Create | TableInitializationOptions.Drop
            };

            q.InitializeQueryObject(qf.Context);

            return q;
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType)
        {
            return ScheduleQueryJob(query, queueType, 0);
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType, int maxPartitions)
        {
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());  // *** TODO

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var qf = CreateQueryFactory(context);
                var q = CreateQuery(qf, query);

                q.MaxPartitions = maxPartitions;

                var ji = qf.ScheduleAsJob(null, q, queue, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }

        protected void RunQuery(string sql)
        {
            RunQuery(sql, 0, new TimeSpan(0, 2, 0));
        }

        protected void RunQuery(string sql, int maxPartitions)
        {
            RunQuery(sql, maxPartitions, new TimeSpan(0, 2, 0));
        }

        protected void RunQuery(string sql, int maxPartitions, TimeSpan timeout)
        {
            var testName = GetTestUniqueName();

            using (SchedulerTester.Instance.GetToken())
            {
                DropUserDatabaseTable(testName);

                SchedulerTester.Instance.EnsureRunning();
                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    sql = sql.Replace("[$into]", testName);

                    var guid = ScheduleQueryJob(sql, QueueType.Long, maxPartitions);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10), timeout);

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                }
            }
        }
    }
}
