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
        public JobInstanceFactory(Context context)
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

#if false // TODO: delete
        public IEnumerable<JobInstance> FindJobInstances(Guid userGuid, Guid queueInstanceGuid, HashSet<Guid> jobDefinitionGuids, JobExecutionState jobExecutionStatus)
        {
            string sql = "spFindJobInstance_byDetails";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@From", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@Max", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@JobUserGuid", SqlDbType.UniqueIdentifier).Value = userGuid == Guid.Empty ? DBNull.Value : (object)userGuid;
                cmd.Parameters.Add("@QueueInstanceGuid", SqlDbType.UniqueIdentifier).Value = queueInstanceGuid == Guid.Empty ? DBNull.Value : (object)queueInstanceGuid;
                cmd.Parameters.Add("@JobDefinitionGuids", SqlDbType.Structured).Value = CreateGuidListTable(jobDefinitionGuids);
                cmd.Parameters.Add("@JobExecutionStatus", SqlDbType.Int).Value = jobExecutionStatus;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        JobInstance item = new JobInstance(Context);
                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }
#endif

        public int CountJobInstances()
        {
            string sql = "spFindJobInstance_byDetails";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
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

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                AppendCommandParameters(cmd, from, max);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        JobInstance job = new JobInstance(Context);
                        job.LoadFromDataReader(dr);

                        yield return job;
                    }
                    dr.Close();
                }
            }
        }

        private void AppendCommandParameters(SqlCommand cmd, int from, int max)
        {
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
            cmd.Parameters.Add("@From", SqlDbType.Int).Value = from == -1 ? DBNull.Value : (object)from;
            cmd.Parameters.Add("@Max", SqlDbType.Int).Value = max == -1 ? DBNull.Value : (object)max;
            cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;

            cmd.Parameters.Add("@JobUserGuid", SqlDbType.UniqueIdentifier).Value = userGuid == Guid.Empty ? DBNull.Value : (object)userGuid;
            cmd.Parameters.Add("@QueueInstanceGuids", SqlDbType.Structured).Value = CreateGuidListTable(queueInstanceGuids);
            cmd.Parameters.Add("@JobDefinitionGuids", SqlDbType.Structured).Value = CreateGuidListTable(jobDefinitionGuids);
            cmd.Parameters.Add("@JobExecutionStatus", SqlDbType.Int).Value = jobExecutionStatus;
        }

        #endregion
    }
}
