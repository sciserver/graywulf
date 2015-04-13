using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Logging
{
    public class Event
    {
        private long eventId;
        private Guid userGuid;
        private Guid jobGuid;
        private Guid contextGuid;
        private EventSource eventSource;
        private EventSeverity eventSeverity;
        private DateTime eventDateTime;
        private long eventOrder;
        private ExecutionStatus executionStatus;
        private string operation;
        private Guid entityGuid;
        private Guid entityGuidFrom;
        private Guid entityGuidTo;
        private string exceptionType;
        private string site;
        private string message;
        private string stackTrace;

        private Dictionary<string, object> userData;
        private Exception exception;

        public long EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        public EventSource EventSource
        {
            get { return eventSource; }
            set { eventSource = value; }
        }

        public EventSeverity EventSeverity
        {
            get { return eventSeverity; }
            set { eventSeverity = value; }
        }

        public DateTime EventDateTime
        {
            get { return eventDateTime; }
            set { eventDateTime = value; }
        }

        public long EventOrder
        {
            get { return eventOrder; }
            set { eventOrder = value; }
        }

        public ExecutionStatus ExecutionStatus
        {
            get { return executionStatus; }
            set { executionStatus = value; }
        }

        public string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        public Guid EntityGuid
        {
            get { return entityGuid; }
            set { entityGuid = value; }
        }

        public Guid EntityGuidFrom
        {
            get { return entityGuidFrom; }
            set { entityGuidFrom = value; }
        }

        public Guid EntityGuidTo
        {
            get { return entityGuidTo; }
            set { entityGuidTo = value; }
        }

        public string ExceptionType
        {
            get { return exceptionType; }
            set { exceptionType = value; }
        }

        public string Site
        {
            get { return site; }
            set { site = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }

        public Dictionary<string, object> UserData
        {
            get { return userData; }
        }

        public Exception Exception
        {
            get { return exception; }
            set { SetException(value); }
        }

        public Event()
        {
            InitializeMembers();
        }

        public Event(Event old)
        {
            CopyMembers(old);
        }

        public Event(string operation, Guid entityGuid)
        {
            InitializeMembers();

            this.operation = operation;
            this.entityGuid = entityGuid;
        }

        public Event(string operation, Exception ex)
        {
            InitializeMembers();

            this.operation = operation;
            SetException(ex);
        }

        private void InitializeMembers()
        {
            this.eventId = 0;
            this.userGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.contextGuid = Guid.Empty;
            this.eventSource = EventSource.None;
            this.eventSeverity = EventSeverity.Status;
            this.eventDateTime = DateTime.Now;
            this.eventOrder = 0;
            this.executionStatus = ExecutionStatus.Executing;
            this.operation = string.Empty;
            this.entityGuid = Guid.Empty;
            this.entityGuidFrom = Guid.Empty;
            this.entityGuidTo = Guid.Empty;
            this.exceptionType = null;
            this.site = Environment.MachineName;
            this.message = null;
            this.stackTrace = null;

            this.userData = new Dictionary<string, object>();
            this.exception = null;
        }

        private void CopyMembers(Event old)
        {
            this.eventId = old.eventId;
            this.userGuid = old.userGuid;
            this.jobGuid = old.jobGuid;
            this.contextGuid = old.contextGuid;
            this.eventSource = old.eventSource;
            this.eventSeverity = old.eventSeverity;
            this.eventDateTime = old.eventDateTime;
            this.eventOrder = old.eventOrder;
            this.executionStatus = old.executionStatus;
            this.operation = old.operation;
            this.entityGuid = old.entityGuid;
            this.entityGuidFrom = old.entityGuidFrom;
            this.entityGuidTo = old.entityGuidTo;
            this.exceptionType = old.exceptionType;
            this.site = old.site;
            this.message = old.message;
            this.stackTrace = old.stackTrace;

            this.userData = new Dictionary<string, object>(old.userData);
            this.exception = old.exception;
        }

        public int LoadFromDataReader(SqlDataReader dr)
        {
            int o = -1;

            this.eventId = dr.GetInt64(++o);
            this.userGuid = dr.GetGuid(++o);
            this.jobGuid = dr.GetGuid(++o);
            this.contextGuid = dr.GetGuid(++o);
            this.eventSource = (EventSource)dr.GetInt32(++o);
            this.eventSeverity = (EventSeverity)dr.GetInt32(++o);
            this.eventDateTime = dr.GetDateTime(++o);
            this.eventOrder = dr.GetInt64(++o);
            this.executionStatus = (ExecutionStatus)dr.GetInt32(++o);
            this.operation = dr.GetString(++o);
            this.entityGuid = dr.GetGuid(++o);
            this.entityGuidFrom = dr.GetGuid(++o);
            this.entityGuidTo = dr.GetGuid(++o);
            this.exceptionType = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.message = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.site = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.stackTrace = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exception = null;

            return o;
        }

        private void SetException(Exception ex)
        {
            message = null;
            site = null;
            stackTrace = null;
            exceptionType = null;

            exception = ex;

            if (ex != null)
            {
                message = GetMessage(ex);
                stackTrace = GetStackTrace(ex);
                exceptionType = GetType(ex);
                site = GetSite(ex);
                eventSeverity = Logging.EventSeverity.Error;
            }
        }

        private string GetMessage(Exception ex)
        {
            if (ex is AggregateException)
            {
                return ex.InnerException.Message;
            }
            else
            {
                return ex.Message;
            }
        }

        private string GetType(Exception ex)
        {
            if (ex is AggregateException)
            {
                return ex.InnerException.GetType().FullName;
            }
            else
            {
                return ex.GetType().FullName;
            }
        }

        /// <summary>
        /// Returns the stack trace from the exception and all inner exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string GetStackTrace(Exception ex)
        {
            var sb = new StringBuilder();
            var e = ex;

            while (e != null)
            {
                sb.AppendFormat("[{0}]", e.GetType().FullName);
                sb.AppendLine();
                sb.AppendLine(e.Message);
                sb.AppendLine(e.StackTrace);
                sb.AppendLine();

                e = e.InnerException;
            }

            if (sb.Length == 0)
            {
                return null;
            }
            else
            {
                return sb.ToString();
            }
        }

        private string GetSite(Exception ex)
        {
            // Unwrap one level
            if (ex is AggregateException)
            {
                ex = ex.InnerException;
            }

            if (ex is SqlException)
            {
                var sqlex = (SqlException)ex;
                return sqlex.Server;
            }
            else if (ex is System.ServiceModel.EndpointNotFoundException)
            {
                var smex = (System.ServiceModel.EndpointNotFoundException)ex;
                return Environment.MachineName;

                // TODO: figure out how to get site from EndpointNotFoundException
            }
            else
            {
                return Environment.MachineName;
            }
        }
    }
}
