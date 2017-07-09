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
        #region Private member variables

        private long id;
        private Guid userGuid;
        private Guid jobGuid;
        private Guid sessionGuid;
        private Guid contextGuid;
        private EventSource source;
        private EventSeverity severity;
        private DateTime dateTime;
        private long order;
        private ExecutionStatus executionStatus;
        private string operation;
        private string server;
        private string client;
        private string message;
        private string exceptionType;
        private string exceptionStackTrace;

        private Dictionary<string, object> userData;
        private Exception exception;

        #endregion
        #region Properties

        public long ID
        {
            get { return id; }
            set { id = value; }
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

        public Guid SessionGuid
        {
            get { return sessionGuid; }
            set { sessionGuid = value; }
        }

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        public EventSource Source
        {
            get { return source; }
            set { source = value; }
        }

        public EventSeverity Severity
        {
            get { return severity; }
            set { severity = value; }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public long Order
        {
            get { return order; }
            set { order = value; }
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

        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        public string Client
        {
            get { return client; }
            set { client = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string ExceptionType
        {
            get { return exceptionType; }
            set { exceptionType = value; }
        }

        public string ExceptionStackTrace
        {
            get { return exceptionStackTrace; }
            set { exceptionStackTrace = value; }
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

        #endregion
        #region Constructors and initializers

        public Event()
        {
            InitializeMembers();
        }

        public Event(Event old)
        {
            CopyMembers(old);
        }

        // TODO ****
        public Event(string operation, Guid entityGuid)
        {
            InitializeMembers();

            this.operation = operation;

            userData[Constants.UserDataEntityGuid] = entityGuid;
        }

        // TODO ***
        public Event(string operation, Exception ex)
        {
            InitializeMembers();

            this.operation = operation;
            SetException(ex);
        }

        private void InitializeMembers()
        {
            this.id = 0;
            this.userGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.sessionGuid = Guid.Empty;
            this.contextGuid = Guid.Empty;
            this.source = EventSource.None;
            this.severity = EventSeverity.Info;
            this.dateTime = DateTime.Now;
            this.order = 0;
            this.executionStatus = ExecutionStatus.Executing;
            this.operation = string.Empty;
            this.server = Environment.MachineName;
            this.client = null;
            this.message = null;
            this.exceptionType = null;
            this.exceptionStackTrace = null;

            this.userData = new Dictionary<string, object>();
            this.exception = null;
        }

        private void CopyMembers(Event old)
        {
            this.id = old.id;
            this.userGuid = old.userGuid;
            this.jobGuid = old.jobGuid;
            this.sessionGuid = old.sessionGuid;
            this.contextGuid = old.contextGuid;
            this.source = old.source;
            this.severity = old.severity;
            this.dateTime = old.dateTime;
            this.order = old.order;
            this.executionStatus = old.executionStatus;
            this.operation = old.operation;
            this.server = old.server;
            this.client = old.client;
            this.message = old.message;
            this.exceptionType = old.exceptionType;
            this.exceptionStackTrace = old.exceptionStackTrace;

            this.userData = new Dictionary<string, object>(old.userData);
            this.exception = old.exception;
        }

        public int LoadFromDataReader(SqlDataReader dr)
        {
            int o = -1;

            this.id = dr.GetInt64(++o);
            this.userGuid = dr.GetGuid(++o);
            this.jobGuid = dr.GetGuid(++o);
            this.sessionGuid = dr.GetGuid(++o);
            this.contextGuid = dr.GetGuid(++o);
            this.source = (EventSource)dr.GetInt32(++o);
            this.severity = (EventSeverity)dr.GetByte(++o);
            this.dateTime = dr.GetDateTime(++o);
            this.order = dr.GetInt64(++o);
            this.executionStatus = (ExecutionStatus)dr.GetByte(++o);
            this.operation = dr.GetString(++o);
            this.server = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.client = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.message = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exceptionType = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exceptionStackTrace = dr.IsDBNull(++o) ? null : dr.GetString(o);

            this.exception = null;

            return o;
        }

        #endregion

        public static Event CreateWebServiceOperationEvent(string operation)
        {
            var e = new Event()
            {
            };

            e.SetWebContext();
            e.SetWcfContext();
            e.SetWcfWebContext();

            return e;
        }

        public static Event CreateWebServiceExceptionEvent(string operation, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void SetWebContext()
        {
            var context = System.Web.HttpContext.Current;

            if (context != null)
            {

            }
        }

        private void SetWcfContext()
        {
            var context = System.ServiceModel.OperationContext.Current;
            
            if (context != null)
            {
                //context.IncomingMessageProperties
            }
        }

        private void SetWcfWebContext()
        {
            var context = System.ServiceModel.Web.WebOperationContext.Current;

            if (context != null)
            {

            }
        }

        private void SetException(Exception ex)
        {
            message = null;
            server = null;
            exceptionStackTrace = null;
            exceptionType = null;

            exception = ex;

            if (ex != null)
            {
                message = GetExceptionMessage(ex);
                exceptionStackTrace = GetExceptionStackTrace(ex);
                exceptionType = GetExceptionType(ex);
                server = GetExceptionSite(ex);
                severity = Logging.EventSeverity.Error;
            }
        }

        private string GetExceptionMessage(Exception ex)
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

        private string GetExceptionType(Exception ex)
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
        private string GetExceptionStackTrace(Exception ex)
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

        private string GetExceptionSite(Exception ex)
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
