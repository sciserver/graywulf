using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Factory class for managing queue instances.
    /// </summary>
    public class QueueInstanceFactory : EntityFactory
    {
        #region Constructors

        /// <summary>
        /// Creates an instance with a context set.
        /// </summary>
        /// <param name="context">A valid context.</param>
        public QueueInstanceFactory(Context context)
            : base(context)
        {
        }

        #endregion
        #region Queue Search Functions

        /// <summary>
        /// Finds all queue instances with scheduled jobs.
        /// </summary>
        /// <returns>An IEnumerable interface to the queue instance objects.</returns>
        /// <remarks>
        /// This function is called by the scheduler's polling function to look
        /// up all queues with scheduled jobs so new jobs can be picked up
        /// easily.
        /// </remarks>
#if false
        TODO: delete, not needed anymore
        public IEnumerable<QueueInstance> FindNonEmptyQueues()
        {
            List<QueueInstance> res = new List<QueueInstance>();

            try
            {
                string sql = "spFindNonEmptyQueueInstance";

                using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
                {
                    cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                    cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                    cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            QueueInstance item = new QueueInstance(Context);
                            item.LoadFromDataReader(dr);
                            res.Add(item);
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception)
            {
            }

            return res;
        }
#endif

        #endregion
    }
}
