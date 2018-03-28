using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Activities;
using System.Activities.Tracking;
using System.Threading;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Logging
{
    /// <summary>
    /// Implements an abmient context to provide logging functionality.
    /// </summary>
    /// <remarks>
    /// When running inside a workflow, logging event are routed through
    /// the workflow foundation event system. For this reason, log messages
    /// from async activities are collected and processed when the async
    /// operation has finished.
    /// </remarks>
    public class LoggingContext : AmbientContextBase
    {
        #region Singletons

        private static Logger logger;

        private static Logger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = new Logger();
                }

                return logger;
            }
        }

        #endregion

        private LoggingContext parent;

        private bool isAsync;
        private EventSource defaultEventSource;
        private int eventOrder;
        private List<Event> asyncEvents;
        private CodeActivityContext activityContext;

        private Guid jobGuid;
        private string jobName;
        private Guid userGuid;
        private string userName;
        private string taskName;

        #region Properties

        public static LoggingContext Current
        {
            get
            {
                var context = Get<LoggingContext>();

                if (context == null)
                {
                    throw Error.LoggingContextNotInitialized();
                }

                return context;
            }
            set { Set(value); }
        }

        /// <summary>
        /// Gets if the logging context is inside an async activity
        /// </summary>
        public bool IsAsync
        {
            get { return isAsync; }
        }

        public EventSource DefaultEventSource
        {
            get { return defaultEventSource; }
            set { defaultEventSource = value; }
        }

        protected List<Event> AsyncEvents
        {
            get { return asyncEvents; }
        }

        public CodeActivityContext ActivityContext
        {
            get { return activityContext; }
            set { activityContext = value; }
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

        #endregion

        #region Constructors and initializers

        public LoggingContext()
            : this(false, AmbientContextStoreLocation.Default)
        {
            // override
        }

        public LoggingContext(bool isAsync)
            : this(isAsync, AmbientContextStoreLocation.Default)
        {
            // override
        }

        public LoggingContext(bool isAsync, AmbientContextStoreLocation supportedLocation)
            : base(supportedLocation)
        {
            InitializeMembers();

            if (isAsync)
            {
                InitializeAsyncMode();
            }
        }

        public LoggingContext(LoggingContext parent)
            : base(parent)
        {
            CopyMembers(parent);
        }

        private void InitializeMembers()
        {
            this.parent = null;

            this.isAsync = false;
            this.defaultEventSource = EventSource.None;
            this.eventOrder = 0;
            this.asyncEvents = null;
            this.activityContext = null;

            this.jobGuid = Guid.Empty;
            this.jobName = null;
            this.userGuid = Guid.Empty;
            this.userName = null;
            this.taskName = null;
        }

        private void InitializeAsyncMode()
        {
            this.isAsync = true;
            this.asyncEvents = new List<Event>();
        }

        private void CopyMembers(LoggingContext parent)
        {
            this.parent = parent;

            this.isAsync = parent.isAsync;
            this.defaultEventSource = parent.defaultEventSource;
            this.eventOrder = parent.eventOrder;
            this.asyncEvents = null;
            this.activityContext = parent.activityContext;

            this.jobGuid = parent.jobGuid;
            this.jobName = parent.jobName;
            this.userGuid = parent.userGuid;
            this.userName = parent.userName;
            this.taskName = parent.taskName;
        }

        #endregion
        #region Control functions

        /// <summary>
        /// When executed on a manually created thread, this method
        /// prevents flowing the async execution context.
        /// </summary>
        public static void SupressFlow()
        {
            DetachAsyncLocal<LoggingContext>();
        }

        public void StartLogger(EventSource defaultEventSource, bool attachConsole)
        {
            this.defaultEventSource = defaultEventSource;
            Logger.Start(attachConsole);
        }

        public void StopLogger()
        {
            Logger.Stop();
        }

        public Logger GetLogger()
        {
            return LoggingContext.logger;
        }

        public void SetLogger(Logger logger)
        {
            LoggingContext.logger = logger;
        }

        #endregion

        public Event LogDebug(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = RecordEventWithUnwindStack(EventSeverity.Debug, source, message, operation, null, data);
            return e;
        }

        public Event LogOperation(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = RecordEventWithUnwindStack(EventSeverity.Operation, source, message, operation, null, data);
            return e;
        }

        public Event LogStatus(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = RecordEventWithUnwindStack(EventSeverity.Status, source, message, operation, null, data);
            return e;
        }

        public Event LogWarning(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = RecordEventWithUnwindStack(EventSeverity.Warning, source, message, operation, null, data);
            return e;
        }

        public Event LogError(EventSource source, Exception ex, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = RecordEventWithUnwindStack(EventSeverity.Error, source, message, operation, ex, data);
            return e;
        }

        private Event RecordEventWithUnwindStack(EventSeverity severity, EventSource source, string message, string operation, Exception ex, Dictionary<string, object> data)
        {
            if (operation == null)
            {
                var method = UnwindStack(2);
                operation = method.DeclaringType.FullName + "." + method.Name;
            }

            var e = CreateEvent(severity, source, message, operation, ex, data);
            RecordEvent(e);
            return e;
        }

        public Event CreateEvent(EventSeverity severity, EventSource source, string message, string operation, Exception ex, Dictionary<string, object> data)
        {
            var e = new Event();

            if (ex == null)
            {
                e.Severity = severity;
                e.Source |= source;
                e.Message = message;
                e.Operation = operation;
            }
            else
            {
                ex = UnwrapException(ex);

                e.Severity = Logging.EventSeverity.Error;
                e.Source |= source;
                e.Message = message ?? GetExceptionMessage(ex);
                e.Operation = operation ?? GetExceptionOperation(ex);
                e.Exception = ex;
                e.ExceptionType = GetExceptionType(ex);
                e.ExceptionStackTrace = GetExceptionStackTrace(ex);
                e.Server = GetExceptionSite(ex);

                e.ExecutionStatus = ExecutionStatus.Faulted;
            }

            if (data != null && data.Count > 0)
            {
                foreach (var key in data.Keys)
                {
                    e.UserData[key] = data[key];
                }
            }

            UpdateEvent(e);

            return e;
        }

        public virtual void UpdateEvent(Event e)
        {
            e.Source |= DefaultEventSource;

            if (this.userGuid != Guid.Empty) e.UserGuid = this.userGuid;
            if (this.userName != null) e.UserName = this.userName;
            if (this.taskName != null) e.TaskName = this.taskName;
            if (this.jobGuid != Guid.Empty) e.JobGuid = this.jobGuid;
            if (this.jobName != null) e.JobName = this.jobName;
            if (this.ContextGuid != Guid.Empty) e.ContextGuid = this.ContextGuid;

            e.Principal = System.Threading.Thread.CurrentPrincipal;
        }

        /// <summary>
        /// Route the event through the pipeline, eventually generating a custom tracking record
        /// </summary>
        /// <param name="e"></param>
        public void RecordEvent(Event e)
        {
            if (parent != null)
            {
                parent.RecordEvent(e);
            }
            else
            {
                e.Order = ++eventOrder;

                if (activityContext != null && !IsAsync)
                {
                    // This is a synchronous event called from a simple CodeActivity
                    // Route event through the workflow tracking infrastructure
                    var ctr = new CustomTrackingRecord("Graywulf log event");
                    ctr.Data.Add("Event", e);
                    activityContext.Track(ctr);
                }
                else if (isAsync)
                {
                    asyncEvents.Add(e);
                }
                else
                {
                    WriteEvent(e);
                }
            }
        }

        public void WriteEvent(Event e)
        {
            TidyUpEvent(e);
            Logger.WriteEvent(e);
        }

        public virtual void FlushEvents()
        {
            if (parent != null)
            {
                parent.FlushEvents();
            }
            else
            {
                if (isAsync && asyncEvents != null)
                {
                    foreach (var e in asyncEvents)
                    {
                        var record = new CustomTrackingRecord("asyncEvent");
                        record.Data[Constants.ActivityRecordDataItemEvent] = e;

                        activityContext.Track(record);
                    }
                }
                else
                {
                    foreach (var e in asyncEvents)
                    {
                        WriteEvent(e);
                    }
                }

                asyncEvents.Clear();
            }
        }

        private void TidyUpEvent(Event e)
        {
            // ServiceModel.ExceptionDetail is non-serializable, this is a bug

            var ex = e.Exception as System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>;
            if (ex != null)
            {
                e.Message = ex.Detail.Message;
                e.ExceptionStackTrace = ex.Detail.StackTrace;
                e.ExceptionType = ex.Detail.Type;
                e.Exception = null;
            }
        }

        public MethodBase UnwindStack(int skip)
        {
            return UnwindStack(skip + 1, typeof(LoggingContext));
        }

        public MethodBase UnwindStack(int skip, Type skipType)
        {
            var stack = new StackTrace(skip, true);

            for (int i = 0; i < stack.FrameCount; i++)
            {
                var frame = stack.GetFrame(i);
                var method = frame.GetMethod();

                if (!skipType.IsAssignableFrom(method.DeclaringType))
                {
                    return method;
                }
            }

            return null;
        }

        protected virtual Exception UnwrapException(Exception ex)
        {
            while (ex is AggregateException)
            {
                ex = ex.InnerException;
            }

            return ex;
        }

        protected virtual string GetExceptionMessage(Exception ex)
        {
            return ex.Message;
        }

        protected virtual string GetExceptionOperation(Exception ex)
        {
            return ex.TargetSite == null ? "(unknown)" : ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
        }

        protected virtual string GetExceptionType(Exception ex)
        {
            return ex.GetType().FullName;
        }

        /// <summary>
        /// Returns the stack trace from the exception and all inner exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual string GetExceptionStackTrace(Exception ex)
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

        protected virtual string GetExceptionSite(Exception ex)
        {
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

        public IEnumerable<Check.CheckRoutineBase> GetCheckRoutines()
        {
            return logger.GetCheckRoutines();
        }
    }
}
