using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Jobs.Query;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class SqlQueryTestBase : TestClassBase
    {
        protected virtual UserDatabaseFactory CreateUserDatabaseFactory(FederationContext context)
        {
            return UserDatabaseFactory.Create(context);
        }

        protected virtual QueryFactory CreateQueryFactory(RegistryContext context)
        {
            var qf = QueryFactory.Create(typeof(SqlQueryFactory).AssemblyQualifiedName, context);
            return qf;
        }

        protected SqlQueryPartition CreatePartition()
        {
            return CreatePartition(false, false);
        }

        protected SqlQueryPartition CreatePartition(bool partitioningKeyMin, bool partitioningKeyMax)
        {
            return new SqlQueryPartition()
            {
                CodeDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                {
                    Name = Jhu.Graywulf.Registry.Constants.CodeDbName,
                    ConnectionString = "data source=localhost;initial catalog=SkyQuery_Code",
                },
                TemporaryDataset = new Graywulf.Sql.Schema.SqlServer.SqlServerDataset()
                {
                    Name = Jhu.Graywulf.Registry.Constants.TempDbName,
                    ConnectionString = "data source=localhost;initial catalog=Graywulf_Temp",
                },
                PartitioningKeyMin = partitioningKeyMin ? (IComparable)(1.0) : null,
                PartitioningKeyMax = partitioningKeyMax ? (IComparable)(1.0) : null,
            };
        }

        protected virtual SqlQueryRewriter CreateQueryRewriter(bool partitioningKeyMin, bool partitioningKeyMax)
        {
            return new SqlQueryRewriter(CreatePartition(partitioningKeyMin, partitioningKeyMax));
        }

        protected virtual SqlQueryCodeGenerator CreateCodeGenerator(bool partitioningKeyMin, bool partitioningKeyMax)
        {
            var cg = new SqlQueryCodeGenerator(CreatePartition(partitioningKeyMin, partitioningKeyMax))
            {
                TableNameRendering = CodeGeneration.NameRendering.FullyQualified,
                ColumnNameRendering = CodeGeneration.NameRendering.FullyQualified,
                DataTypeNameRendering = CodeGeneration.NameRendering.FullyQualified,
                FunctionNameRendering = CodeGeneration.NameRendering.FullyQualified,
                VariableRendering = CodeGeneration.VariableRendering.Substitute,
            };

            cg.AddSystemVariableMappings();

            return cg;
        }

        protected void RewriteQueryHelper(string sql, string gt)
        {
            var qrw = CreateQueryRewriter(false, false);
            var cg = CreateCodeGenerator(false, false);
            var parsingTree = Parse(sql);

            qrw.Execute(parsingTree);
            var res = cg.Execute(parsingTree);

            Assert.AreEqual(gt, res);
        }

        protected void RewriteQueryHelper(string sql, string gt, bool partitioningKeyMin, bool partitioningKeyMax)
        {
            var qrw = CreateQueryRewriter(partitioningKeyMin, partitioningKeyMax);
            var cg = CreateCodeGenerator(partitioningKeyMin, partitioningKeyMax);
            var parsingTree = Parse(sql);

            qrw.Execute(parsingTree);
            var res = cg.Execute(parsingTree);

            Assert.AreEqual(gt, res);
        }

        protected virtual SqlQuery CreateQuery(string query)
        {
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                var qf = CreateQueryFactory(context);
                var q = CreateQuery(qf, query);

                return q;
            }
        }

        private SqlQuery CreateQuery(QueryFactory qf, string query)
        {
            var user = SignInTestUser(qf.RegistryContext);

            var udf = CreateUserDatabaseFactory(new FederationContext(qf.RegistryContext, user));
            var mydb = udf.GetUserDatabases(user)[Registry.Constants.UserDbName];
            var mysi = udf.GetUserDatabaseServerInstances(user)[Registry.Constants.UserDbName];

            var q = qf.CreateQuery(query);
            qf.AppendUserDatabase(q, mydb, mysi);

            q.Parameters.Destination = new Jhu.Graywulf.IO.Tasks.DestinationTable()
            {
                Dataset = mydb,
                DatabaseName = mydb.DatabaseName,
                SchemaName = mydb.DefaultSchemaName,
                TableNamePattern = "testtable",     // will be overwritten by INTO queries
                Options = TableInitializationOptions.Create | TableInitializationOptions.Drop
            };

            q.InitializeQueryObject(null, qf.RegistryContext);

            return q;
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType)
        {
            return ScheduleQueryJob(query, queueType, 0, false);
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType, int maxPartitions, bool dumpsql)
        {
            var queue = GetQueueName(queueType);

            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                var qf = CreateQueryFactory(context);
                var q = CreateQuery(qf, query);

                q.Parameters.MaxPartitions = maxPartitions;
                q.Parameters.DumpSql = dumpsql;

                var ji = qf.ScheduleAsJob(null, q, queue, TimeSpan.Zero, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }
        
        protected void RunQuery(string sql)
        {
            var timeout = new TimeSpan(0, 5, 0);
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
                        using (var cn = new SqlConnection(Logging.SqlLogWriter.Configuration.ConnectionString))
                        {
                            cn.Open();

                            var lsql =
@"SELECT TOP 1 EventException.Type, StackTrace, Server
FROM Event 
INNER JOIN EventException ON EventID = ID
WHERE JobGuid = @jobGuid
ORDER BY ID DESC";

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

                    if (ji.JobExecutionStatus == JobExecutionState.Failed)
                    {
                        throw new Exception(ji.ExceptionMessage);
                    }
                    else
                    {
                        Assert.AreEqual(expectedOutcome, ji.JobExecutionStatus);
                    }
                }
            }
        }
    }
}
