using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class JobDefinition : Entity
    {
        #region Database IO Functions

        /// <summary>
        /// Saves a new <b>Job Definition</b> to the database.
        /// </summary>
        /// <remarks>
        /// This function also queries the workflow for input parameters
        /// </remarks>
        protected override void Create()
        {
            // TODO: consider removing the discover functions and require calling them from the caller code
            // instead of doing it here.

            base.Create();

            DiscoverWorkflowParameters();
            SaveParameters();
        }

        /// <summary>
        /// Modifies and existiong <b>Job Definition</b> in the database.
        /// </summary>
        /// <param name="forceOverwrite">If true, record in the database is overwritten despite of any exceptions.</param>
        protected override void Modify(bool forceOverwrite)
        {
            base.Modify(forceOverwrite);

            DeleteParameters();
            DiscoverWorkflowParameters();
            SaveParameters();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            LoadParameters();
        }

        /// <summary>
        /// Loads job workflow input parameters from the database.
        /// </summary>
        protected void LoadParameters()
        {
            parameters.Clear();

            string sql = "spFindJobDefinitionParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobDefinitionGuid", SqlDbType.UniqueIdentifier).Value = Guid;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var par = new JobParameter()
                        {
                            Name = dr.GetString(1),
                            TypeName = dr.GetString(2),
                            Direction = (JobParameterDirection)dr.GetByte(3)
                        };

                        parameters.Add(par.Name, par);
                    }
                }
            }
        }

        /// <summary>
        /// Saves job workflow input parameters to the database.
        /// </summary>
        protected void SaveParameters()
        {
            string sql = "spCreateJobDefinitionParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobDefinitionGuid", SqlDbType.UniqueIdentifier).Value = Guid;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
                cmd.Parameters.Add("@Direction", SqlDbType.TinyInt);

                foreach (var par in parameters.Values)
                {
                    cmd.Parameters["@Name"].Value = par.Name;
                    cmd.Parameters["@Type"].Value = par.TypeName;
                    cmd.Parameters["@Direction"].Value = (byte)par.Direction;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes job workflow input parameters from the database.
        /// </summary>
        protected void DeleteParameters()
        {
            string sql = "spDeleteJobDefinitionParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobDefinitionGuid", SqlDbType.UniqueIdentifier).Value = Guid;

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
