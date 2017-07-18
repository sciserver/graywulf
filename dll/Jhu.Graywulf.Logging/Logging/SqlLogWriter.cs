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

        private SqlConnection connection;
        private SqlTransaction transaction;
        private SqlCommand createEventCommand;
        private SqlCommand createEventDataCommand;

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
            set
            {
                SetConnectionString(value);
            }
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
            this.connectionString = null;
            this.connection = null;
            this.transaction = null;
            this.createEventCommand = null;
            this.createEventDataCommand = null;
        }

        #endregion

        protected override void OnStart()
        {
            createEventCommand = CreateCreateEventCommand();
            createEventDataCommand = CreateCreateEventDataCommand();
        }

        protected override void OnBatchStart()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        protected override void OnBatchEnd()
        {
            transaction.Commit();
            transaction.Dispose();
            transaction = null;

            connection.Dispose();
            connection = null;
        }

        protected override void OnStop()
        {
            createEventCommand.Dispose();
            createEventDataCommand.Dispose();
        }

        protected override void OnWriteEvent(Event e)
        {
            try
            {
                // --- write event
                SetCreateEventCommandValues(e);
                e.ID = ExecuteCreateEventCommand();

                // --- write data
                foreach (string key in e.UserData.Keys)
                {
                    SetCreateEventDataCommandValues(e.ID, key, e.UserData[key]);
                    ExecuteCreateEventDataCommand();
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

        protected override void OnUnhandledExpcetion(Exception ex)
        {
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }

            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }

        private void SetConnectionString(string connectionString)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            csb.Enlist = false;
            this.connectionString = csb.ConnectionString;
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

        private void SetCreateEventCommandValues(Event e)
        {
            createEventCommand.Parameters["@UserGuid"].Value = e.UserGuid == Guid.Empty ? (object)DBNull.Value : e.UserGuid;
            createEventCommand.Parameters["@UserName"].Value = e.UserName == null ? (object)DBNull.Value : (object)e.UserName;
            createEventCommand.Parameters["@TaskName"].Value = e.TaskName == null ? (object)DBNull.Value : (object)e.TaskName;
            createEventCommand.Parameters["@JobGuid"].Value = e.JobGuid == Guid.Empty ? (object)DBNull.Value : e.JobGuid;
            createEventCommand.Parameters["@JobName"].Value = e.JobName == null ? (object)DBNull.Value : (object)e.JobName;
            createEventCommand.Parameters["@SessionGuid"].Value = e.SessionGuid == Guid.Empty ? (object)DBNull.Value : e.SessionGuid;
            createEventCommand.Parameters["@ContextGuid"].Value = e.ContextGuid == Guid.Empty ? (object)DBNull.Value : e.ContextGuid;
            createEventCommand.Parameters["@Source"].Value = e.Source;
            createEventCommand.Parameters["@Severity"].Value = e.Severity;
            createEventCommand.Parameters["@DateTime"].Value = e.DateTime;
            createEventCommand.Parameters["@Order"].Value = e.Order;
            createEventCommand.Parameters["@ExecutionStatus"].Value = e.ExecutionStatus;
            createEventCommand.Parameters["@Operation"].Value = e.Operation;
            createEventCommand.Parameters["@Server"].Value = e.Server == null ? (object)DBNull.Value : (object)e.Server;
            createEventCommand.Parameters["@Client"].Value = e.Client == null ? (object)DBNull.Value : (object)e.Client;
            createEventCommand.Parameters["@Request"].Value = e.Request == null ? (object)DBNull.Value : (object)e.Request;
            createEventCommand.Parameters["@Message"].Value = e.Message == null ? (object)DBNull.Value : (object)e.Message;
            createEventCommand.Parameters["@ExceptionType"].Value = e.ExceptionType == null ? (object)DBNull.Value : (object)e.ExceptionType;
            createEventCommand.Parameters["@ExceptionStackTrace"].Value = e.ExceptionStackTrace == null ? (object)DBNull.Value : (object)e.ExceptionStackTrace;
            createEventCommand.Parameters["@BookmarkGuid"].Value = e.BookmarkGuid == Guid.Empty ? (object)DBNull.Value : e.BookmarkGuid;

            createEventCommand.Connection = connection;
            createEventCommand.Transaction = transaction;
        }

        private long ExecuteCreateEventCommand()
        {
            createEventCommand.ExecuteNonQuery();
            return Convert.ToInt64(createEventCommand.Parameters["@EventId"].Value);
        }

        private void SetCreateEventDataCommandValues(long eventId, string key, object data)
        {
            createEventDataCommand.Parameters["@EventId"].Value = eventId;
            createEventDataCommand.Parameters["@Key"].Value = key;
            createEventDataCommand.Parameters["@Data"].Value = data;

            createEventDataCommand.Connection = connection;
            createEventDataCommand.Transaction = transaction;
        }

        private void ExecuteCreateEventDataCommand()
        {
            createEventDataCommand.ExecuteNonQuery();
        }
    }
}
