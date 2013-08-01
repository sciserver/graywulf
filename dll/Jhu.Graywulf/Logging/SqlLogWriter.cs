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
        private object syncRoot = new object();
        private string connectionString;
        private SqlCommand createEventCommand;
        private SqlCommand createEventDataCommand;

        public SqlLogWriter()
            : base()
        {
            CreateCommands();
            connectionString = AppSettings.ConnectionString;
        }

        private void CreateCommands()
        {
            string sql;
            
            // --- Create Event Command
            sql = "spCreateEvent";

            createEventCommand = new SqlCommand();
            createEventCommand.CommandText = sql;
            createEventCommand.CommandTimeout = 30; // TODO take from config?
            createEventCommand.CommandType = CommandType.StoredProcedure;
    
            createEventCommand.Parameters.Add("@EventId", SqlDbType.BigInt).Direction = ParameterDirection.Output;
            createEventCommand.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@JobGuid", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@ContextGuid", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@EventSource", SqlDbType.Int);
            createEventCommand.Parameters.Add("@EventSeverity", SqlDbType.Int);
            createEventCommand.Parameters.Add("@EventDateTime", SqlDbType.DateTime);
            createEventCommand.Parameters.Add("@EventOrder", SqlDbType.BigInt);
            createEventCommand.Parameters.Add("@ExecutionStatus", SqlDbType.Int);
            createEventCommand.Parameters.Add("@Operation", SqlDbType.NVarChar, 255);
            createEventCommand.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@EntityGuidFrom", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@EntityGuidTo", SqlDbType.UniqueIdentifier);
            createEventCommand.Parameters.Add("@ExceptionType", SqlDbType.NVarChar, 255);
            createEventCommand.Parameters.Add("@Message", SqlDbType.NVarChar, 1024);
            createEventCommand.Parameters.Add("@StackTrace", SqlDbType.NVarChar);

            // --- Create Event Data command
            sql = "spCreateEventData";

            createEventDataCommand = new SqlCommand();
            createEventDataCommand.CommandText = sql;
            createEventDataCommand.CommandTimeout = 30;     // *** TODO: take from config
            createEventDataCommand.CommandType = CommandType.StoredProcedure;

            createEventDataCommand.Parameters.Add("@EventId", SqlDbType.BigInt);
            createEventDataCommand.Parameters.Add("@Key", SqlDbType.NVarChar, 50);
            createEventDataCommand.Parameters.Add("@Data", SqlDbType.Variant);
        }

        private void SetEventCommandValues(Event e)
        {
            createEventCommand.Parameters["@UserGuid"].Value = e.UserGuid;
            createEventCommand.Parameters["@JobGuid"].Value = e.JobGuid;
            createEventCommand.Parameters["@ContextGuid"].Value = e.ContextGuid;
            createEventCommand.Parameters["@EventSource"].Value = e.EventSource; 
            createEventCommand.Parameters["@EventSeverity"].Value = e.EventSeverity;
            createEventCommand.Parameters["@EventDateTime"].Value = e.EventDateTime;
            createEventCommand.Parameters["@EventOrder"].Value = e.EventOrder;
            createEventCommand.Parameters["@ExecutionStatus"].Value =  e.ExecutionStatus;
            createEventCommand.Parameters["@Operation"].Value = e.Operation;
            createEventCommand.Parameters["@EntityGuid"].Value = e.EntityGuid;
            createEventCommand.Parameters["@EntityGuidFrom"].Value = e.EntityGuidFrom;
            createEventCommand.Parameters["@EntityGuidTo"].Value = e.EntityGuidTo;
            createEventCommand.Parameters["@ExceptionType"].Value = e.ExceptionType == null ? (object)DBNull.Value : (object)e.ExceptionType;
            createEventCommand.Parameters["@Message"].Value = e.Message == null ? (object)DBNull.Value : (object)e.Message;
            createEventCommand.Parameters["@StackTrace"].Value = e.StackTrace == null ? (object)DBNull.Value : (object)e.StackTrace;
        }

        private void SetEventDataValues(long eventId, string key, object data)
        {
            createEventDataCommand.Parameters["@EventId"].Value = eventId;
            createEventDataCommand.Parameters["@Key"].Value = key;
            createEventDataCommand.Parameters["@Data"].Value = data;
        }

        public override void WriteEvent(Event e)
        {
            lock (syncRoot)
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    using (SqlTransaction tn = cn.BeginTransaction())
                    {
                        // --- write event
                        SetEventCommandValues(e);

                        createEventCommand.Connection = cn;
                        createEventCommand.Transaction = tn;

                        createEventCommand.ExecuteNonQuery();
                        e.EventId = Convert.ToInt64(createEventCommand.Parameters["@EventId"].Value);

                        // --- write data
                        createEventDataCommand.Connection = cn;
                        createEventDataCommand.Transaction = tn;

                        foreach (string key in e.UserData.Keys)
                        {
                            SetEventDataValues(e.EventId, key, e.UserData[key]);
                            createEventDataCommand.ExecuteNonQuery();
                        }

                        tn.Commit();
                    }
                }
            }
        }
    }
}
