using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

namespace Jhu.Graywulf.Logging
{
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
        private List<Event> asyncEvents;

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

        #region Constructors and initializers

        protected LoggingContext()
            : this(null, false)
        {
        }

        protected LoggingContext(bool isAsync)
            :this(null, isAsync)
        {
        }

        protected LoggingContext(LoggingContext outerContext)
            : this(null, outerContext.isAsync)
        {
        }

        protected LoggingContext(LoggingContext outerContext, bool isAsync)
        {
            if (outerContext != null)
            {
                CopyMembers(outerContext);
            }
            else
            {
                InitializeMembers();
            }

            if (isAsync)
            {
                InitializeAsyncMode();
            }
        }

        private void InitializeMembers()
        {
            this.outerContext = null;
            this.isValid = true;
            this.isAsync = false;
            this.defaultEventSource = EventSource.None;
            this.asyncEvents = null;
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
            this.asyncEvents = null;
        }

        public virtual void Dispose()
        {
            isValid = false;
        }

        #endregion

        public void Push()
        {
            LoggingContext.Current = this;
        }

        public void Pop()
        {
            LoggingContext.Current = this.outerContext;
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
            var e = CreateEvent(EventSeverity.Debug, source, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogOperation(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Operation, source, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogStatus(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Status, source, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogWarning(EventSource source, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Warning, source, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogError(EventSource source, Exception ex, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Error, source, message, operation, ex, data);
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

                if (operation == null)
                {
                    var method = UnwindStack(2);
                    e.Operation = method.DeclaringType.FullName + "." + method.Name;
                }
                else
                {
                    e.Operation = operation;
                }
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
        }

        /// <summary>
        /// Route the event through the pipeline, eventually generating a custom tracking record
        /// </summary>
        /// <param name="e"></param>
        public virtual void RecordEvent(Event e)
        {
            if (isAsync)
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
            foreach (var e in asyncEvents)
            {
                Logger.WriteEvent(e);
            }

            asyncEvents.Clear();
        }

        public MethodBase UnwindStack(int skip)
        {
            var stack = new StackTrace(skip, true);

            for (int i = 0; i < stack.FrameCount; i++)
            {
                var frame = stack.GetFrame(i);
                var method = frame.GetMethod();

                if (!typeof(LoggingContext).IsAssignableFrom(method.DeclaringType))
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
    }
}
