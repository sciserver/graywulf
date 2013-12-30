using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Test
{
    public abstract class TestClassBase
    {

        #region Scheduler functions

        protected enum QueueType
        {
            Maintenance,
            Long,
            Quick,
        }

        protected enum JobType
        {
            AtomicDelay,
            CancelableDelay,
            MultipleDelay,
        }

        protected Task scheduler;

        protected User SignInTestUser(Context context)
        {
            // TODO: throw exception on logon failure
            var ef = new EntityFactory(context);
            
            //var c = ef.LoadEntity<Cluster>(Cluster.AppSettings.ClusterName);
            var d = ef.LoadEntity<Domain>(Domain.AppSettings.DomainName);

            var uu = new UserFactory(context);
            return uu.LoginUser(d, "test", "alma");
        }

        protected void PurgeTestJobs()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var jd = ef.LoadEntity<JobDefinition>(Cluster.AppSettings.ClusterName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);

                var jf = new JobInstanceFactory(context);
                foreach (var job in jf.FindJobInstances(Guid.Empty, Guid.Empty, new HashSet<Guid>() { jd.Guid }, JobExecutionState.Scheduled))
                {
                    job.Cancel();
                }
            }
        }

        protected Guid ScheduleTestJob(TimeSpan delayPeriod, JobType jobType, QueueType queueType)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                SignInTestUser(context);

                var ef = new EntityFactory(context);
                var jd = ef.LoadEntity<JobDefinition>(Cluster.AppSettings.ClusterName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);

                var queue = String.Format("Graywulf.Controller.Controller.{0}", queueType.ToString());

                JobInstance job = jd.CreateJobInstance(queue, Jhu.Graywulf.Registry.ScheduleType.Queued);

                job.Parameters["DelayPeriod"].SetValue((int)delayPeriod.TotalMilliseconds);
                job.Parameters["TestMethod"].SetValue(jobType.ToString());

                job.Name = String.Format("{0}_{1}", "test", DateTime.Now.ToString("yyMMddHHmmssff"));

                job.Save();

                return job.Guid;
            }
        }

        protected JobInstance LoadJob(Guid guid)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.DirtyRead))
            {
                var job = new JobInstance(context);
                job.Guid = guid;
                job.Load();
                job.LoadParameters();

                return job;
            }
        }

        protected void CancelJob(Guid guid)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var job = new JobInstance(context);
                job.Guid = guid;
                job.Load();
                job.LoadParameters();

                job.Cancel();
            }
        }

        protected void WaitJobComplete(Guid guid, TimeSpan pollingInterval)
        {
            while (true)
            {
                Thread.Sleep(pollingInterval);

                var ji = LoadJob(guid);

                if ((ji.JobExecutionStatus == JobExecutionState.Scheduled))
                {
                    throw new InvalidOperationException();
                }

                if ((ji.JobExecutionStatus &
                    (JobExecutionState.Cancelled | JobExecutionState.Completed | JobExecutionState.Failed |
                     JobExecutionState.Persisted | JobExecutionState.TimedOut)) != 0)
                {
                    break;
                }
            }
        }

        protected void WaitJobStarted(Guid guid, TimeSpan pollingInterval)
        {
            while (true)
            {
                Thread.Sleep(pollingInterval);

                var ji = LoadJob(guid);

                if (ji.JobExecutionStatus != JobExecutionState.Scheduled && ji.JobExecutionStatus != JobExecutionState.Starting)
                {
                    break;
                }
            }
        }

        #endregion

        protected bool IsTableExisting(Table table)
        {
            return IsTableExisting(table.Dataset.ConnectionString, table.SchemaName, table.TableName);
        }

        protected bool IsTableExisting(string connectionString, string schemaName, string tableName)
        {
            var sql = String.Format("IF OBJECT_ID('{0}.{1}','U') IS NOT NULL SELECT 1 ELSE SELECT 0", schemaName, tableName);

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
        }

        protected void DropMyDBTable(string schemaName, string tableName)
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var federation = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);
                var user = SignInTestUser(context);
                var di = user.GetUserDatabaseInstance(federation.MyDBDatabaseVersion);

                DropTable(di.GetConnectionString().ConnectionString, "dbo", "SqlQueryTest_SimpleQueryTest");
            }
        }

        protected void DropTable(Table table)
        {
            DropTable(table.Dataset.ConnectionString, table.SchemaName, table.TableName);
        }

        protected void DropTable(string connectionString, string schemaName, string tableName)
        {
            var sql = String.Format("IF OBJECT_ID('{0}.{1}','U') IS NOT NULL DROP TABLE {0}.{1}", schemaName, tableName);

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
