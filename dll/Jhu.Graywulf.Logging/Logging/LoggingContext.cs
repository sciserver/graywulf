using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Activities;
using System.Activities.Tracking;

namespace Jhu.Graywulf.Logging
{
    [Serializable]
    public class LoggingContext : IDisposable
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

        [ThreadStatic]
        private static LoggingContext context;

        public static LoggingContext Current
        {
            get
            {
                if (context == null)
                {
                    context = new LoggingContext();
                }

                return context;
            }
            set
            {
                context = value;
            }
        }

        #endregion

        private LoggingContext outerContext;
        private bool isValid;
        private bool isAsync;
        private EventSource defaultEventSource;
        private int eventOrder;
        private List<Event> asyncEvents;
        private CodeActivityContext activityContext;

        private Guid contextGuid;
        private Guid jobGuid;
        private string jobName;
        private Guid userGuid;
        private string userName;
        private string taskName;

        #region Properties

        /// <summary>
        /// Gets the validity of the context.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
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

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
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
            : this(null, false)
        {
        }

        public LoggingContext(bool isAsync)
            : this(null, isAsync)
        {
        }

        public LoggingContext(LoggingContext outerContext)
            : this(null, outerContext.isAsync)
        {
        }

        public LoggingContext(LoggingContext outerContext, bool isAsync)
        {
            if (outerContext != null)
            {
                CopyMembers(outerContext);
            }
            else
            {
                InitializeMembers(new StreamingContext());
            }

            if (isAsync)
            {
                InitializeAsyncMode();
            }
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.outerContext = null;
            this.isValid = true;
            this.isAsync = false;
            this.defaultEventSource = EventSource.None;
            this.eventOrder = 0;
            this.asyncEvents = null;
            this.activityContext = null;

            this.contextGuid = Guid.NewGuid();
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

        private void CopyMembers(LoggingContext outerContext)
        {
            this.outerContext = outerContext;
            this.isValid = true;
            this.isAsync = outerContext.isAsync;
            this.defaultEventSource = outerContext.defaultEventSource;
            this.eventOrder = 0;
            this.asyncEvents = null;
            this.activityContext = outerContext.activityContext;

            this.contextGuid = Guid.NewGuid();
            this.jobGuid = outerContext.jobGuid;
            this.jobName = outerContext.jobName;
            this.userGuid = outerContext.userGuid;
            this.userName = outerContext.userName;
            this.taskName = outerContext.taskName;
        }

        public virtual void Dispose()
        {
            isValid = false;
        }

        #endregion

        public void Push()
        {
            this.outerContext = LoggingContext.Current;
            LoggingContext.Current = this;
        }

        public void Pop()
        {
            LoggingContext.Current = this.outerContext;
            this.outerContext = null;
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

            LoggingContext.Current.UpdateEvent(e);

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
            if (this.contextGuid != Guid.Empty) e.ContextGuid = this.contextGuid;

            e.Principal = System.Threading.Thread.CurrentPrincipal;
        }

        /// <summary>
        /// Route the event through the pipeline, eventually generating a custom tracking record
        /// </summary>
        /// <param name="e"></param>
        public void RecordEvent(Event e)
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
                Logger.WriteEvent(e);
            }
        }

        public void WriteEvent(Event e)
        {
            Logger.WriteEvent(e);
        }

        public virtual void FlushEvents()
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
                    Logger.WriteEvent(e);
                }
            }

            asyncEvents.Clear();
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
