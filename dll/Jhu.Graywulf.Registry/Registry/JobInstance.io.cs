using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.IO;

namespace Jhu.Graywulf.Registry
{
    public partial class JobInstance : Entity
    {
        #region Database IO Functions

        /// <summary>
        /// Saves the job instance to the database. Also saves job input parameters.
        /// </summary>
        protected override void Create()
        {
            base.Create();

            SaveParameters();
        }

        /// <summary>
        /// Modifies the existing job instance in the database. Also saves job input parameters.
        /// </summary>
        /// <param name="forceOverwrite"></param>
        protected override void Modify(bool forceOverwrite)
        {
            base.Modify(forceOverwrite);

            DeleteParameters();
            SaveParameters();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            LoadParameters();
        }

        /// <summary>
        /// Loads job instance input parameters.
        /// </summary>
        public void LoadParameters()
        {
            parameters.Clear();

            string sql = "spFindJobInstanceParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = Guid;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var par = new JobParameter()
                        {
                            Name = dr.GetString(1),
                            TypeName = dr.GetString(2),
                            Direction = (JobParameterDirection)dr.GetByte(3),
                            XmlValue = dr.IsDBNull(4) ? null : dr.GetString(4)
                        };

                        parameters.Add(par.Name, par);
                    }
                }
            }
        }

        /// <summary>
        /// Saves job instance parameters.
        /// </summary>
        protected void SaveParameters()
        {
            string sql = "spCreateJobInstanceParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = Guid;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
                cmd.Parameters.Add("@Direction", SqlDbType.TinyInt);
                cmd.Parameters.Add("@Value", SqlDbType.NVarChar);

                foreach (var par in parameters.Values)
                {
                    cmd.Parameters["@Name"].Value = par.Name;
                    cmd.Parameters["@Type"].Value = par.TypeName;
                    cmd.Parameters["@Direction"].Value = par.Direction;
                    cmd.Parameters["@Value"].Value = (object)par.XmlValue ?? DBNull.Value;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes job instance input parameters.
        /// </summary>
        protected void DeleteParameters()
        {
            string sql = "spDeleteJobInstanceParameter";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = Guid;

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
