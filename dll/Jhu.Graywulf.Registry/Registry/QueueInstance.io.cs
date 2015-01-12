using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class QueueInstance : Entity
    {
        #region Database IO Functions

        #endregion
        #region Navigation Functions

        /// <summary>
        /// Gets the next available job from the queue.
        /// </summary>
        /// <returns>The next available job or null if there are no queued jobs.</returns>
        /// <remarks>
        /// This function takes the user the last scheduled job was associated with to
        /// implement a round-robin scheduling.
        /// </remarks>
        public JobInstance GetNextJobInstance(Guid lastUserGuid)
        {
            string sql = "spFindJobInstance_Next";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@QueueInstanceGuid", SqlDbType.UniqueIdentifier).Value = this.Guid;
                cmd.Parameters.Add("@LastUserGuid", SqlDbType.UniqueIdentifier).Value = lastUserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@JobInstanceGuid"].Value != DBNull.Value)
                {
                    JobInstance j = new JobInstance(Context);
                    j.Guid = (Guid)cmd.Parameters["@JobInstanceGuid"].Value;
                    j.Load();

                    return j;
                }
            }

            return null;
        }

        public void LoadJobInstances(bool forceReload)
        {
            LoadChildren<JobInstance>(forceReload);
        }

        #endregion
    }
}
