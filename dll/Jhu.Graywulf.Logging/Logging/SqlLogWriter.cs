using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class SqlLogWriter : LogWriterBase
    {
        public static SqlLogWriterConfiguration Configuration
        {
            get
            {
                return (SqlLogWriterConfiguration)ConfigurationManager.GetSection("jhu.graywulf/logging/sql");
            }
        }

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

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

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
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@TaskName", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@JobGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@JobName", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@SessionGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ContextGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@Source", SqlDbType.Int);
            cmd.Parameters.Add("@Severity", SqlDbType.TinyInt);
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime);
            cmd.Parameters.Add("@Order", SqlDbType.BigInt);
            cmd.Parameters.Add("@ExecutionStatus", SqlDbType.TinyInt);
            cmd.Parameters.Add("@Operation", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@Server", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@Client", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@Request", SqlDbType.VarChar, 1024);
            cmd.Parameters.Add("@Message", SqlDbType.VarChar, 1024);
            cmd.Parameters.Add("@ExceptionType", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@ExceptionStackTrace", SqlDbType.VarChar);
            cmd.Parameters.Add("@BookmarkGuid", SqlDbType.UniqueIdentifier);

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
            cmd.Parameters.Add("@Key", SqlDbType.VarChar, 50);
            cmd.Parameters.Add("@Data", SqlDbType.Variant);

            return cmd;
        }

        private void SetCreateEventCommandValues(SqlCommand cmd, Event e)
        {
            cmd.Parameters["@UserGuid"].Value = e.UserGuid == Guid.Empty ? (object)DBNull.Value : e.UserGuid;
            cmd.Parameters["@UserName"].Value = e.UserName == null ? (object)DBNull.Value : (object)e.UserName;
            cmd.Parameters["@TaskName"].Value = e.TaskName == null ? (object)DBNull.Value : (object)e.TaskName;
            cmd.Parameters["@JobGuid"].Value = e.JobGuid == Guid.Empty ? (object)DBNull.Value : e.JobGuid;
            cmd.Parameters["@JobName"].Value = e.JobName == null ? (object)DBNull.Value : (object)e.JobName;
            cmd.Parameters["@SessionGuid"].Value = e.SessionGuid == Guid.Empty ? (object)DBNull.Value : e.SessionGuid;
            cmd.Parameters["@ContextGuid"].Value = e.ContextGuid == Guid.Empty ? (object)DBNull.Value : e.ContextGuid;
            cmd.Parameters["@Source"].Value = e.Source;
            cmd.Parameters["@Severity"].Value = e.Severity;
            cmd.Parameters["@DateTime"].Value = e.DateTime;
            cmd.Parameters["@Order"].Value = e.Order;
            cmd.Parameters["@ExecutionStatus"].Value = e.ExecutionStatus;
            cmd.Parameters["@Operation"].Value = e.Operation;
            cmd.Parameters["@Server"].Value = e.Server == null ? (object)DBNull.Value : (object)e.Server;
            cmd.Parameters["@Client"].Value = e.Client == null ? (object)DBNull.Value : (object)e.Client;
            cmd.Parameters["@Request"].Value = e.Request == null ? (object)DBNull.Value : (object)e.Request;
            cmd.Parameters["@Message"].Value = e.Message == null ? (object)DBNull.Value : (object)e.Message;
            cmd.Parameters["@ExceptionType"].Value = e.ExceptionType == null ? (object)DBNull.Value : (object)e.ExceptionType;
            cmd.Parameters["@ExceptionStackTrace"].Value = e.ExceptionStackTrace == null ? (object)DBNull.Value : (object)e.ExceptionStackTrace;
            cmd.Parameters["@BookmarkGuid"].Value = e.BookmarkGuid == Guid.Empty ? (object)DBNull.Value : e.BookmarkGuid;
        }

        private void SetCreateEventDataCommandValues(SqlCommand cmd, long eventId, string key, object data)
        {
            cmd.Parameters["@EventId"].Value = eventId;
            cmd.Parameters["@Key"].Value = key;
            cmd.Parameters["@Data"].Value = data;
        }

        protected override void OnWriteEvent(Event e)
        {
            try
            {
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
                            e.ID = Convert.ToInt64(cmd.Value.Parameters["@EventId"].Value);
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
                                    SetCreateEventDataCommandValues(cmd.Value, e.ID, key, e.UserData[key]);
                                    cmd.Value.ExecuteNonQuery();
                                }
                            }
                        }

                        tn.Commit();
                    }
                }
            }
            catch (SqlException)
            {
                if (!skipExceptions)
                {
                    throw;
                }
            }
        }
    }
}
