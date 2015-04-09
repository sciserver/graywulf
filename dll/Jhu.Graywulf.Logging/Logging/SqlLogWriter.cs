using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Logging
{
    public class SqlLogWriter : LogWriter
    {
        #region Private member variables

        private bool skipExceptions;
        private string connectionString;

        /// <summary>
        /// Holds an object pool for database commands to create an event
        /// </summary>
        private Components.ObjectPool<SqlCommand> createEventCommandPool;

        /// <summary>
        /// Holds an object pool for database commands to create event data
        /// </summary>
        private Components.ObjectPool<SqlCommand> createEventDataCommandPool;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets whether database server exceptions are skipped.
        /// </summary>
        public bool SkipExceptions
        {
            get { return skipExceptions; }
            set { skipExceptions = value; }
        }

        /// <summary>
        /// Gets or set the database connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        #endregion
        #region Constructors and initializers

        public SqlLogWriter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.skipExceptions = true;
            this.connectionString = AppSettings.ConnectionString;
            createEventCommandPool = new Components.ObjectPool<SqlCommand>(CreateCreateEventCommand);
            createEventDataCommandPool = new Components.ObjectPool<SqlCommand>(CreateCreateEventDataCommand);
        }

        #endregion

        /// <summary>
        /// Creates a command that records an event in the database.
        /// </summary>
        /// <returns></returns>
        private SqlCommand CreateCreateEventCommand()
        {
            var sql = "spCreateEvent";

            var cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30; // TODO take from config?
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@JobGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ContextGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@EventSource", SqlDbType.Int);
            cmd.Parameters.Add("@EventSeverity", SqlDbType.Int);
            cmd.Parameters.Add("@EventDateTime", SqlDbType.DateTime);
            cmd.Parameters.Add("@EventOrder", SqlDbType.BigInt);
            cmd.Parameters.Add("@ExecutionStatus", SqlDbType.Int);
            cmd.Parameters.Add("@Operation", SqlDbType.NVarChar, 255);
            cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@EntityGuidFrom", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@EntityGuidTo", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ExceptionType", SqlDbType.NVarChar, 255);
            cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 255);
            cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@StackTrace", SqlDbType.NVarChar);

            return cmd;
        }

        /// <summary>
        /// Creates a command that records event data in the database.
        /// </summary>
        /// <returns></returns>
        private SqlCommand CreateCreateEventDataCommand()
        {
            var sql = "spCreateEventData";

            var cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;     // *** TODO: take from config
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@EventId", SqlDbType.BigInt);
            cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Data", SqlDbType.Variant);

            return cmd;
        }

        private void SetCreateEventCommandValues(SqlCommand cmd, Event e)
        {
            cmd.Parameters["@UserGuid"].Value = e.UserGuid;
            cmd.Parameters["@JobGuid"].Value = e.JobGuid;
            cmd.Parameters["@ContextGuid"].Value = e.ContextGuid;
            cmd.Parameters["@EventSource"].Value = e.EventSource;
            cmd.Parameters["@EventSeverity"].Value = e.EventSeverity;
            cmd.Parameters["@EventDateTime"].Value = e.EventDateTime;
            cmd.Parameters["@EventOrder"].Value = e.EventOrder;
            cmd.Parameters["@ExecutionStatus"].Value = e.ExecutionStatus;
            cmd.Parameters["@Operation"].Value = e.Operation;
            cmd.Parameters["@EntityGuid"].Value = e.EntityGuid;
            cmd.Parameters["@EntityGuidFrom"].Value = e.EntityGuidFrom;
            cmd.Parameters["@EntityGuidTo"].Value = e.EntityGuidTo;
            cmd.Parameters["@ExceptionType"].Value = e.ExceptionType == null ? (object)DBNull.Value : (object)e.ExceptionType;
            cmd.Parameters["@Site"].Value = e.Site == null ? (object)DBNull.Value : (object)e.Site;
            cmd.Parameters["@Message"].Value = e.Message == null ? (object)DBNull.Value : (object)e.Message;
            cmd.Parameters["@StackTrace"].Value = e.StackTrace == null ? (object)DBNull.Value : (object)e.StackTrace;
        }

        private void SetCreateEventDataCommandValues(SqlCommand cmd, long eventId, string key, object data)
        {
            cmd.Parameters["@EventId"].Value = eventId;
            cmd.Parameters["@Key"].Value = key;
            cmd.Parameters["@Data"].Value = data;
        }

        public override void WriteEvent(Event e)
        {
            //try
            //{
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlTransaction tn = cn.BeginTransaction())
                {
                    // --- write event
                    using (var cmd = createEventCommandPool.Take())
                    {
                        SetCreateEventCommandValues(cmd.Value, e);

                        cmd.Value.Connection = cn;
                        cmd.Value.Transaction = tn;

                        cmd.Value.ExecuteNonQuery();
                        e.EventId = Convert.ToInt64(cmd.Value.Parameters["@EventId"].Value);
                    }

                    // --- write data
                    if (e.UserData.Count > 0)
                    {
                        using (var cmd = createEventDataCommandPool.Take())
                        {
                            cmd.Value.Connection = cn;
                            cmd.Value.Transaction = tn;

                            foreach (string key in e.UserData.Keys)
                            {
                                SetCreateEventDataCommandValues(cmd.Value, e.EventId, key, e.UserData[key]);
                                cmd.Value.ExecuteNonQuery();
                            }
                        }
                    }

                    tn.Commit();
                }
            }
            //}
            //catch (SqlException)
            //{
            //    if (!skipExceptions)
            //    {
            //        throw;
            //    }
            //}
        }
    }
}
