using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Scheduler;

namespace Jhu.Graywulf.Test
{
    public abstract class TestClassBase : LoggingTestClassBase
    {
        protected const string TestUser = "test";
        protected const string OtherUser = "testother";
        protected const string TestGroup = "test";
        protected const string GroupAdminUser = "testadmin";
        protected const string GroupWriterUser = "testwriter";
        protected const string GroupReaderUser = "testreader";

        private Random rnd = new Random();
        private SqlServerDataset ioTestDataset;
        private User testUser;

        protected SqlServerDataset IOTestDataset
        {
            get
            {
                if (ioTestDataset == null)
                {
                    ioTestDataset = CreateIOTestDataset();
                }

                return ioTestDataset;
            }
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
            Exception,
            AsyncException,
            AsyncExceptionWithRetry,
            RetryWithFaultInFinally,
            RetryWithFaultInCancel,
            QueryDelay,
            QueryTimeout,
            QueryDelayRetry,
            QueryTimeoutRetry,
            AsyncTrackingRecord,
        }

        protected virtual SqlServerDataset CreateIOTestDataset()
        {
            return new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString);
        }

        protected User SignInTestUser(RegistryContext context)
        {
            var ip = IdentityProvider.Create(context.Domain);
            ip.VerifyPassword(new AuthenticationRequest("test", "almafa"));

            testUser = ip.GetUserByUserName("test");

            context.UserReference.Value = testUser;
            return testUser;
        }

        protected static void InitializeJobTests()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                PurgeTestJobs();
            }
        }

        protected static void CleanupJobTests()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                if (SchedulerTester.Instance.IsRunning)
                {
                    SchedulerTester.Instance.DrainStop();
                }

                PurgeTestJobs();
            }
        }

        public static void PurgeTestJobs()
        {
            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                var sql = @"UPDATE Entity
SET RunningState = 64
FROM Entity
INNER JOIN JobInstance ON EntityGuid = Guid
WHERE DateFinished IS NULL";

                var cmd = context.CreateTextCommand(sql);
                cmd.ExecuteNonQuery();
            }

            /*using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var jd = ef.LoadEntity<JobDefinition>(Registry.ContextManager.Configuration.ClusterName, Registry.Constants.SystemDomainName, Registry.Constants.SystemFederationName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);

                var jf = new JobInstanceFactory(context);
                jf.JobDefinitionGuids.Add(jd.Guid);
                jf.JobExecutionStatus = JobExecutionState.Scheduled | JobExecutionState.Executing;

                foreach (var job in jf.FindJobInstances())
                {
                    if ((job.JobExecutionStatus & JobExecutionState.CancelRequested) == 0)
                    {
                        job.Cancel();
                    }
                }
            }*/
        }

        protected SqlServerDataset GetTestUserMyDB()
        {
            // Get mydb default schema
            using (var context = ContextManager.Instance.CreateContext(TransactionMode.DirtyRead))
            {
                var user = SignInTestUser(context);
                var fc = new FederationContext(context, user);

                return (SqlServerDataset)fc.SchemaManager.Datasets[Registry.Constants.UserDbName];
            }
        }

        protected string GetQueueName(QueueType queueType)
        {
            return String.Format(@"QueueInstance:Graywulf\Controller\{0}", queueType.ToString());
        }

        protected Guid ScheduleTestJob(JobType jobType, QueueType queueType)
        {
            return ScheduleTestJob(TimeSpan.Zero, jobType, queueType, TimeSpan.Zero);
        }

        protected Guid ScheduleTestJob(JobType jobType, QueueType queueType, TimeSpan timeout)
        {
            return ScheduleTestJob(TimeSpan.Zero, jobType, queueType, timeout);
        }

        protected Guid ScheduleTestJob(TimeSpan delayPeriod, JobType jobType, QueueType queueType, TimeSpan timeout)
        {
            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                SignInTestUser(context);

                var queue = GetQueueName(queueType);
                var ef = new EntityFactory(context);
                var qi = ef.LoadEntity<QueueInstance>(queue);
                var jd = ef.LoadEntity<JobDefinition>(Registry.ContextManager.Configuration.ClusterName, Registry.Constants.SystemDomainName, Registry.Constants.SystemFederationName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name);
                
                JobInstance job = jd.CreateJobInstance(queue, ScheduleType.Queued, timeout);

                job.Parameters["DelayPeriod"].Value = (int)delayPeriod.TotalMilliseconds;
                job.Parameters["TestMethod"].Value = jobType.ToString();

                lock (rnd)
                {
                    job.Name = String.Format("{0}_{1}_{2}", "test", DateTime.UtcNow.ToString("yyMMddHHmmssff"), rnd.Next(1000));
                }

                job.Save();

                return job.Guid;
            }
        }

        protected JobInstance LoadJob(Guid guid)
        {
            using (var context = ContextManager.Instance.CreateContext(TransactionMode.DirtyRead))
            {
                var ef = new EntityFactory(context);
                var job = ef.LoadEntity<JobInstance>(guid);
                return job;
            }
        }

        protected void CancelJob(Guid guid)
        {
            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                var ef = new EntityFactory(context);
                var job = ef.LoadEntity<JobInstance>(guid);
                job.Cancel();
            }
        }

        protected void WaitJobComplete(Guid guid, TimeSpan pollingInterval)
        {
            WaitJobComplete(guid, pollingInterval, new TimeSpan(0, 2, 0));
        }

        protected void WaitJobComplete(Guid guid, TimeSpan pollingInterval, TimeSpan timeout)
        {
            var start = DateTime.Now;

            // Wait for job until timeout or indefinitely when debugging
            while ((DateTime.Now - start) < timeout ||
                System.Diagnostics.Debugger.IsAttached)
            {
                Thread.Sleep(pollingInterval);

                var ji = LoadJob(guid);

                /* 
                // This can be used for debugging but otherwise not necessary anymore
                if ((ji.JobExecutionStatus == JobExecutionState.Scheduled))
                {
                    throw new Exception("Unexpected job outcome");
                }*/

                if ((ji.JobExecutionStatus &
                    (JobExecutionState.Cancelled | JobExecutionState.Completed | JobExecutionState.Failed |
                     JobExecutionState.Persisted | JobExecutionState.TimedOut)) != 0)
                {
                    return;
                }
            }

            throw new Exception("Test job has not finished in a reasonable time.");
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

        protected void DropUserDatabaseTable(string tableName)
        {
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                var user = SignInTestUser(context);
                var udf = UserDatabaseFactory.Create(new FederationContext(context, user));
                var userdb = udf.GetUserDatabases(user)[Registry.Constants.UserDbName];

                DropTable(userdb.ConnectionString, userdb.DefaultSchemaName, tableName);
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

        /// <summary>
        /// Find the outmost directory with a solution file
        /// </summary>
        /// <returns></returns>
        protected static string GetSolutionDir()
        {
            var dir = Environment.CurrentDirectory;
            string best = null;

            while (dir != null)
            {
                var files = Directory.GetFiles(dir, "*.sln");

                if (files != null && files.Length > 0)
                {
                    best = dir;
                }

                dir = Directory.GetParent(dir)?.FullName;
            }

            return best;
        }

        protected static string GetTestFilePath(string filename)
        {
            var sln = GetSolutionDir();
            return Path.Combine(sln, filename);
        }

        protected string GetTestFilePath(params string[] filename)
        {
            var sln = GetSolutionDir();
            return Path.Combine(sln, Path.Combine(filename));
        }

        protected Uri GetAbsoluteTestUniqueFileUri(string prefix, string extension)
        {
            var name = GetTestUniqueName();
            var path = GetTestFilePath(prefix, name + extension);
            var uri = Util.UriConverter.FromFilePath(path);

            return uri;
        }

        protected Uri GetTestUniqueFileUri(string extension)
        {
            var fn = GetTestUniqueName() + extension;
            return new Uri(fn, UriKind.Relative);
        }

        protected string GetTestUniqueName()
        {
            return GetTestClassName() + "_" + GetTestMethodName();
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

            throw new Exception("Not called from a test method");
        }

        protected object CallMethod(object obj, string name, params object[] pars)
        {
            var tt = new Type[pars.Length];

            for (int i = 0; i < pars.Length; i++)
            {
                tt[i] = pars[i].GetType();
            }

            var t = obj.GetType();
            var f = t.GetMethod(
                name,
                BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                null, CallingConventions.Any, tt, null);

            try
            {
                return f.Invoke(obj, pars);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
