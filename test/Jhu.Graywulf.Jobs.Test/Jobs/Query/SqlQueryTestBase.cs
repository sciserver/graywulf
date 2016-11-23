using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
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
                //typeof(GraywulfUserDatabaseFactory).AssemblyQualifiedName,
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
            var mydb = udf.GetUserDatabases(user)[Registry.Constants.UserDbName];
            var mysi = udf.GetUserDatabaseServerInstances(user)[Registry.Constants.UserDbName];

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
            return ScheduleQueryJob(query, queueType, 0, false);
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType, int maxPartitions, bool dumpsql)
        {
            var queue = GetQueueName(queueType);

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var qf = CreateQueryFactory(context);
                var q = CreateQuery(qf, query);

                q.MaxPartitions = maxPartitions;
                q.DumpSql = dumpsql;

                var ji = qf.ScheduleAsJob(null, q, queue, TimeSpan.Zero, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }

        protected void RunQuery(string sql)
        {
            var timeout = new TimeSpan(0, 2, 0);
            var partitions = 1;
            var dumpsql = false;

            RunQuery(sql, partitions, timeout, dumpsql);
        }

        protected void RunQuery(string sql, int maxPartitions, bool dumpsql)
        {
            RunQuery(sql, maxPartitions, new TimeSpan(0, 2, 0), dumpsql);
        }

        protected void RunQuery(string sql, int maxPartitions, TimeSpan timeout, bool dumpsql)
        {
            RunQuery(sql, QueueType.Long, JobExecutionState.Completed, maxPartitions, timeout, dumpsql);
        }

        protected void RunQuery(string sql, QueueType queue, JobExecutionState expectedOutcome, int maxPartitions, TimeSpan timeout, bool dumpsql)
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

                    var guid = ScheduleQueryJob(sql, queue, maxPartitions, dumpsql);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10), timeout);

                    var ji = LoadJob(guid);

                    if (ji.JobExecutionStatus == JobExecutionState.Failed)
                    {
                        // Load stack trace from
                        using (var cn = new SqlConnection(Jhu.Graywulf.Logging.AppSettings.ConnectionString))
                        {
                            cn.Open();

                            var lsql =
@"SELECT TOP 1 ExceptionType, StackTrace, Site FROM Event 
WHERE JobGuid = @jobGuid AND
      ExceptionType IS NOT NULL
ORDER BY EventID DESC";

                            using (var cmd = new SqlCommand(lsql, cn))
                            {
                                cmd.Parameters.Add("@jobGuid", SqlDbType.UniqueIdentifier).Value = ji.Guid;

                                using (var dr = cmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        var exceptionType = dr.GetString(0);
                                        var stackTrace = dr.GetString(1);
                                        var site = dr.GetString(2);

                                        throw new SqlQueryTestException(ji.ExceptionMessage, site, stackTrace);
                                    }
                                }
                            }

                        }
                    }

                    Assert.AreEqual(expectedOutcome, ji.JobExecutionStatus);
                }
            }
        }
    }
}
