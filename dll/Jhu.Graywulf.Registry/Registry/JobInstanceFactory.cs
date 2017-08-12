using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Factory class for managing job instances.
    /// </summary>
    public class JobInstanceFactory : EntityFactory
    {
        #region Static member variables

        /// <summary>
        /// Rendom number generator used to gerenate unique job names.
        /// </summary>
        private static readonly Random random = new Random();

        #endregion
        #region Member variables

        private Guid userGuid;
        private HashSet<Guid> queueInstanceGuids;
        private HashSet<Guid> jobDefinitionGuids;
        private JobExecutionState jobExecutionStatus;

        #endregion
        #region Properties

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public HashSet<Guid> QueueInstanceGuids
        {
            get { return queueInstanceGuids; }
        }

        public HashSet<Guid> JobDefinitionGuids
        {
            get { return jobDefinitionGuids; }
        }

        public JobExecutionState JobExecutionStatus
        {
            get { return jobExecutionStatus; }
            set { jobExecutionStatus = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance with a context set.
        /// </summary>
        /// <param name="context">A valid context.</param>
        public JobInstanceFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.userGuid = Guid.Empty;
            this.queueInstanceGuids = new HashSet<Guid>();
            this.jobDefinitionGuids = new HashSet<Guid>();
            this.jobExecutionStatus = JobExecutionState.All;
        }

        #endregion
        #region Job Search Functions

        public int CountJobInstances()
        {
            string sql = "spFindJobInstance_byDetails";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendCommandParameters(cmd, -1, -1);

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["@RowCount"].Value;
            }
        }

        public IEnumerable<JobInstance> FindJobInstances()
        {
            return FindJobInstances(-1, -1);
        }

        public IEnumerable<JobInstance> FindJobInstances(int from, int max)
        {
            string sql = "spFindJobInstance_byDetails";

            var cmd = RegistryContext.CreateStoredProcedureCommand(sql);
            AppendCommandParameters(cmd, from, max);

            return new EntityCommandEnumerator<JobInstance>(RegistryContext, cmd, true);
        }

        private void AppendCommandParameters(SqlCommand cmd, int from, int max)
        {
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
            cmd.Parameters.Add("@From", SqlDbType.Int).Value = from == -1 ? DBNull.Value : (object)from;
            cmd.Parameters.Add("@Max", SqlDbType.Int).Value = max == -1 ? DBNull.Value : (object)max;
            cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;

            cmd.Parameters.Add("@JobUserGuid", SqlDbType.UniqueIdentifier).Value = userGuid == Guid.Empty ? DBNull.Value : (object)userGuid;
            cmd.Parameters.Add("@QueueInstanceGuids", SqlDbType.Structured).Value = CreateGuidListTable(queueInstanceGuids);
            cmd.Parameters.Add("@JobDefinitionGuids", SqlDbType.Structured).Value = CreateGuidListTable(jobDefinitionGuids);
            cmd.Parameters.Add("@JobExecutionStatus", SqlDbType.Int).Value = jobExecutionStatus;
        }

        /// <summary>
        /// Gets at set of waiting jobs from the queue
        /// </summary>
        /// <returns>The next available job or null if there are no queued jobs.</returns>
        /// <remarks>
        /// This function takes the user the last scheduled job was associated with to
        /// implement a round-robin scheduling.
        /// </remarks>
        public IEnumerable<JobInstance> FindAndLockNextJobInstances(Guid queueInstanceGuid, Guid lastUserGuid, int max)
        {
            string sql = "spFindJobInstance_Next";

            var cmd = RegistryContext.CreateStoredProcedureCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@LockOwner", SqlDbType.UniqueIdentifier).Value = RegistryContext.LockOwner;
            cmd.Parameters.Add("@QueueInstanceGuid", SqlDbType.UniqueIdentifier).Value = queueInstanceGuid;
            cmd.Parameters.Add("@LastUserGuid", SqlDbType.UniqueIdentifier).Value = lastUserGuid;
            cmd.Parameters.Add("@MaxJobs", SqlDbType.Int).Value = max;

            return new EntityCommandEnumerator<JobInstance>(RegistryContext, cmd, true);
        }

        public JobInstance FindAndLockJobInstance(Guid guid)
        {
            string sql = "spGetJobInstance";

            var cmd = RegistryContext.CreateStoredProcedureCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@LockOwner", SqlDbType.UniqueIdentifier).Value = RegistryContext.LockOwner;
            cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = guid;

            return new EntityCommandEnumerator<JobInstance>(RegistryContext, cmd, true).FirstOrDefault();
        }

        #endregion

        private static string GenerateUniqueJobID()
        {
            int rnd;

            lock (random)
            {
                rnd = random.Next(1000);
            }

            var now = DateTime.UtcNow;

            return String.Format("{0:yyMMddHHmmssff}{1:000}", now, rnd);
        }

        public static string GenerateUniqueJobID(RegistryContext context)
        {
            return String.Format("{0}_{1}", EntityFactory.GetName(context.User.Name), GenerateUniqueJobID());
        }

        public static string GenerateRecurringJobID(RegistryContext context, string oldName)
        {
            // TODO: This is an ad-hoc solution, make it more robust
            // Take old name, but remove date part
            int i = oldName.LastIndexOf('_');

            string newname;

            if (i < 0)
            {
                newname = oldName;
            }
            else if (i == 0)
            {
                newname = context.User.Name;
            }
            else
            {
                newname = oldName.Substring(0, i);
            }

            return String.Format("{0}_{1}", newname, GenerateUniqueJobID());
        }
    }
}
