using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Test
{
    public abstract class TestClassBase
    {
        private SqlServerDataset ioTestDataset = new SqlServerDataset("Test", Jhu.Graywulf.Test.AppSettings.IOTestConnectionString);

        protected SqlServerDataset IOTestDataset
        {
            get { return ioTestDataset; }
        }

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
            var d = ef.LoadEntity<Domain>(Registry.AppSettings.DomainName);

            var uu = new UserFactory(context);
            return uu.LoginUser(d, "test", "alma");
        }

        protected void PurgeTestJobs()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var jd = ef.LoadEntity<JobDefinition>(Registry.AppSettings.ClusterName, Registry.Constants.SharedDomainName, Registry.Constants.SharedFederationName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);

                var jf = new JobInstanceFactory(context);
                jf.UserGuid = Guid.Empty;
                jf.JobDefinitionGuids.Add(jd.Guid);
                jf.JobExecutionStatus = JobExecutionState.Scheduled;
                foreach (var job in jf.FindJobInstances())
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
                var jd = ef.LoadEntity<JobDefinition>(Registry.AppSettings.ClusterName, Registry.Constants.SharedDomainName, Registry.Constants.SharedFederationName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);

                var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());

                JobInstance job = jd.CreateJobInstance(queue, Jhu.Graywulf.Registry.ScheduleType.Queued);

                job.Parameters["DelayPeriod"].Value = (int)delayPeriod.TotalMilliseconds;
                job.Parameters["TestMethod"].Value = jobType.ToString();

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
                var federation = ef.LoadEntity<Federation>(Registry.AppSettings.FederationName);
                var user = SignInTestUser(context);
                var di = federation.MyDBDatabaseVersion.GetUserDatabaseInstance(user);

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

        protected Uri GetTestFilename(string extension)
        {
            var fn = GetTestClassName() + "_" + GetTestMethodName() + extension;
            return new Uri(fn, UriKind.Relative);
        }

        protected string GetTestClassName()
        {
            return GetType().Name;
        }

        protected string GetTestMethodName()
        {
            var stackTrace = new System.Diagnostics.StackTrace();

            foreach (var stackFrame in stackTrace.GetFrames())
            {
                var methodBase = stackFrame.GetMethod();
                var attributes = methodBase.GetCustomAttributes(typeof(TestMethodAttribute), false);

                if (attributes.Length >= 1)
                {
                    return methodBase.Name;
                }
            }

            return "Not called from a test method";
        }
    }
}
