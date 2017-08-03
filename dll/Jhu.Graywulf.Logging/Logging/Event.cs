using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Principal;

namespace Jhu.Graywulf.Logging
{
    public class Event
    {
        #region Private member variables

        private long id;
        private Guid userGuid;
        private string userName;
        private string taskName;
        private Guid jobGuid;
        private string jobName;
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
        private string request;
        private string message;
        private string exceptionType;
        private string exceptionStackTrace;
        private Guid bookmarkGuid;

        private Dictionary<string, object> userData;

        private Exception exception;
        private IPrincipal principal;

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

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public string JobName
        {
            get { return jobName; }
            set { jobName = value; }
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

        public string Request
        {
            get { return request; }
            set { request = value; }
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

        public Guid BookmarkGuid
        {
            get { return bookmarkGuid; }
            set { bookmarkGuid = value; }
        }

        public Dictionary<string, object> UserData
        {
            get { return userData; }
        }

        public Exception Exception
        {
            get { return exception; }
            internal set { exception = value; }
        }

        public IPrincipal Principal
        {
            get { return principal; }
            set { principal = value; }
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
            this.userName = null;
            this.taskName = null;
            this.jobGuid = Guid.Empty;
            this.jobName = null;
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
            this.request = null;
            this.exceptionType = null;
            this.exceptionStackTrace = null;
            this.bookmarkGuid = Guid.Empty;

            this.userData = new Dictionary<string, object>();

            this.exception = null;
            this.principal = null;
        }

        private void CopyMembers(Event old)
        {
            this.id = old.id;
            this.userGuid = old.userGuid;
            this.userName = old.userName;
            this.taskName = old.taskName;
            this.jobGuid = old.jobGuid;
            this.jobName = old.jobName;
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
            this.request = old.request;
            this.message = old.message;
            this.exceptionType = old.exceptionType;
            this.exceptionStackTrace = old.exceptionStackTrace;
            this.bookmarkGuid = old.bookmarkGuid;

            this.userData = new Dictionary<string, object>(old.userData);

            this.exception = old.exception;
            this.principal = old.principal;
        }

        public int LoadFromDataReader(SqlDataReader dr)
        {
            int o = -1;

            this.id = dr.GetInt64(++o);
            this.userGuid = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.userName = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.taskName = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.jobGuid = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.jobName = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.sessionGuid = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.contextGuid = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.source = (EventSource)dr.GetInt32(++o);
            this.severity = (EventSeverity)dr.GetByte(++o);
            this.dateTime = dr.GetDateTime(++o);
            this.order = dr.GetInt64(++o);
            this.executionStatus = (ExecutionStatus)dr.GetByte(++o);
            this.operation = dr.GetString(++o);
            this.server = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.client = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.request = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.message = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exceptionType = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.exceptionStackTrace = dr.IsDBNull(++o) ? null : dr.GetString(o);
            this.bookmarkGuid = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);

            this.exception = null;
            this.principal = null;

            return o;
        }

        #endregion

        internal void Validate()
        {
            if (this.operation == null)
            {
                throw Error.OperationNull();
            }
        }
    }
}
