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

        private static string GenerateUniqueJobID()
        {
            int rnd;

            lock (random)
            {
                rnd = random.Next(1000);
            }

            var now = DateTime.Now;

            return String.Format("{0:yyMMddHHmmssff}{1:000}", now, rnd);
        }

        public static string GenerateUniqueJobID(Context context)
        {
            return String.Format("{0}_{1}", EntityFactory.GetName(context.UserName), GenerateUniqueJobID());
        }

        public static string GenerateRecurringJobID(Context context, string oldName)
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
                newname = context.UserName;
            }
            else
            {
                newname = oldName.Substring(0, i);
            }

            return String.Format("{0}_{1}", newname, GenerateUniqueJobID());
        }
    }
}
