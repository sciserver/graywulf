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
        #region Constructors

        /// <summary>
        /// Creates a new instance with a context set.
        /// </summary>
        /// <param name="context">A valid context.</param>
        public JobInstanceFactory(Context context)
            : base(context)
        {
        }

        #endregion
        #region Job Search Functions

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

        #endregion
    }
}
