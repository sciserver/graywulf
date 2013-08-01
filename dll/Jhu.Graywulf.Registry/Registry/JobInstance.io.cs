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

#if false
        /// <summary>
        /// Loads the entity from a <b>SqlDataReader</b> object.
        /// </summary>
        /// <param name="dr">The data reader to read from.</param>
        /// <returns>Returns the number of columns read.</returns>
        /// <remarks>
        /// Always reads at the current cursor position, doesn't calls the <b>Read</b> function
        /// on the <b>SqlDataReader</b> object. Reads data columns by their ordinal position in
        /// the query and not by their names.
        /// </remarks>
        internal override int LoadFromDataReader(SqlDataReader dr)
        {
            int o = base.LoadFromDataReader(dr);

            ++o;    // skip guid
            this.workflowTypeName = dr.GetString(++o);
            this.dateStarted = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            this.dateFinished = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            this.jobExecutionStatus = (JobExecutionState)dr.GetInt32(++o);
            this.suspendTimeout = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            this.scheduleType = (ScheduleType)dr.GetInt32(++o);
            this.scheduleTime = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            this.recurringPeriod = (RecurringPeriod)dr.GetInt32(++o);
            this.recurringInterval = dr.GetInt32(++o);
            this.recurringMask = dr.GetInt64(++o);
            this.workflowInstanceId = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.adminRequestTime = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            if (!dr.IsDBNull(++o))
            {
                XmlSerializer ser = new XmlSerializer(typeof(JobAdminRequestData));
                StringReader sr = new StringReader(dr.GetString(o));
                this.adminRequestData = (JobAdminRequestData)ser.Deserialize(sr);
            }
            else
            {
                this.adminRequestData = null;
            }
            this.adminRequestResult = dr.GetInt32(++o);
            this.exceptionMessage = dr.IsDBNull(++o) ? null : dr.GetString(o);

            return o;
        }
#endif

        /*
        /// <summary>
        /// Appends required parameters to create/modify stored procedure commands
        /// </summary>
        /// <param name="cmd">The <b>SqlCommand</b> to append the parameters to.</param>
        protected override void AppendCreateModifyParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@WorkflowTypeName", SqlDbType.NVarChar, 1000).Value = workflowTypeName;
            cmd.Parameters.Add("@DateStarted", SqlDbType.DateTime).Value = dateStarted == DateTime.MinValue ? (object)DBNull.Value : (object)dateStarted;
            cmd.Parameters.Add("@DateFinished", SqlDbType.DateTime).Value = dateFinished == DateTime.MinValue ? (object)DBNull.Value : (object)dateFinished;
            cmd.Parameters.Add("@JobExecutionStatus", SqlDbType.Int).Value = jobExecutionStatus;
            cmd.Parameters.Add("@SuspendTimeout", SqlDbType.DateTime).Value = suspendTimeout == DateTime.MinValue ? (object)DBNull.Value : (object)suspendTimeout;
            cmd.Parameters.Add("@ScheduleType", SqlDbType.Int).Value = scheduleType;
            cmd.Parameters.Add("@ScheduleTime", SqlDbType.DateTime).Value = scheduleTime == DateTime.MinValue ? (object)DBNull.Value : (object)scheduleTime;
            cmd.Parameters.Add("@RecurringPeriod", SqlDbType.Int).Value = recurringPeriod;
            cmd.Parameters.Add("@RecurringInterval", SqlDbType.Int).Value = recurringInterval;
            cmd.Parameters.Add("@RecurringMask", SqlDbType.BigInt).Value = recurringMask;
            cmd.Parameters.Add("@WorkflowInstanceId", SqlDbType.UniqueIdentifier).Value = workflowInstanceId == Guid.Empty ? (object)DBNull.Value : (object)workflowInstanceId;
            cmd.Parameters.Add("@AdminRequestTime", SqlDbType.DateTime).Value = adminRequestTime == DateTime.MinValue ? (object)DBNull.Value : (object)adminRequestTime;
            
            cmd.Parameters.Add("@AdminRequestData", SqlDbType.NVarChar);
            if (adminRequestData != null)
            {
                XmlSerializer ser = new XmlSerializer(typeof(JobAdminRequestData));
                StringWriter sw = new StringWriter();
                ser.Serialize(sw, adminRequestData);
                cmd.Parameters["@AdminRequestData"].Value = sw.ToString();
            }
            else
            {
                cmd.Parameters["@AdminRequestData"].Value = DBNull.Value;
            }

            cmd.Parameters.Add("@AdminRequestResult", SqlDbType.Int).Value = adminRequestResult;
            cmd.Parameters.Add("@ExceptionMessage", SqlDbType.NVarChar).Value = exceptionMessage == null ? (object)DBNull.Value : (object)exceptionMessage;
        }
        */

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

        /// <summary>
        /// Loads the job instance from the database. Also loads job input parameters.
        /// </summary>
        public override void Load()
        {
            base.Load();

            LoadParameters();
            LoadCheckpoints();
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

        /// <summary>
        /// Loads job instance checkpoints.
        /// </summary>
        protected void LoadCheckpoints()
        {
            checkpoints.Clear();

            string sql = "spFindJobInstanceCheckpoint";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = Guid;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        JobCheckpoint cp = new JobCheckpoint();

                        // 0 index is the job instance guid
                        int o = -1;
                        cp.SequenceNumber = dr.GetInt32(++o);
                        cp.Name = dr.GetString(++o);
                        cp.ExecutionStatus = (JobExecutionState)dr.GetInt32(++o);

                        checkpoints.Add(cp);
                    }
                }
            }
        }

        /// <summary>
        /// Sets a single job instance checkpoint's identified by its name.
        /// </summary>
        /// <param name="name">Name of the checkpoint.</param>
        /// <param name="executionStatus">Execution status of the checkpoint.</param>
        /// <remarks>
        /// This function is called by the <b>Jhu.Graywulf.Workflow.Activities.CheckpointActivity</b>
        /// to record the sequential provenance information of the executing job.
        /// </remarks>
        public void SetCheckpoint(string name, JobExecutionState executionStatus)
        {
            string sql = "spSetJobInstanceCheckpoint";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@JobInstanceGuid", SqlDbType.UniqueIdentifier).Value = Guid;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128).Value = name;
                cmd.Parameters.Add("@ExecutionStatus", SqlDbType.Int).Value = executionStatus;

                cmd.ExecuteNonQuery();
            }

            // Write log entry
            Jhu.Graywulf.Logging.Event e = new Jhu.Graywulf.Logging.Event("Jhu.Graywulf.Registry.JobInstance.SetCheckpoint", this.Guid);
            e.UserData.Add("Name", name);
            e.UserData.Add("ExecutionStatus", executionStatus.ToString());
            Context.LogEvent(e);
        }

        #endregion
    }
}
