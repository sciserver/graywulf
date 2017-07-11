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
        private Exception exception;
        private string exceptionType;
        private string exceptionStackTrace;

        private Dictionary<string, object> userData;

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

        public Exception Exception
        {
            get { return exception; }
            internal set { exception = value; }
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

        private void InitializeMembers()
        {
            this.id = 0;
            this.userGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.sessionGuid = Guid.Empty;
            this.contextGuid = Guid.Empty;
            this.source = EventSource.None;
            this.severity = EventSeverity.None;
            this.dateTime = DateTime.Now;
            this.order = 0;
            this.executionStatus = ExecutionStatus.Executing;
            this.operation = string.Empty;
            this.server = Environment.MachineName;
            this.client = null;
            this.message = null;
            this.exception = null;
            this.exceptionType = null;
            this.exceptionStackTrace = null;

            this.userData = new Dictionary<string, object>();
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
            this.exception = old.exception;
            this.exceptionType = old.exceptionType;
            this.exceptionStackTrace = old.exceptionStackTrace;

            this.userData = new Dictionary<string, object>(old.userData);
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
            this.exception = null;
            this.exceptionType = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exceptionStackTrace = dr.IsDBNull(++o) ? null : dr.GetString(o);

            return o;
        }

        #endregion

        /* TODO: delete
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
        */
    }
}
