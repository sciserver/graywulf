using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;

namespace Jhu.Graywulf.Logging
{
    public class Logger
    {
        #region Singleton

        public static readonly Logger Instance = new Logger();

        #endregion
        #region Private member variables

        private LoggerStatus status;
        private EventSource eventSource;
        private List<LogWriterBase> writers;

        #endregion
        #region Properties

        public LoggerStatus Status
        {
            get { return status; }
        }

        public EventSource EventSource
        {
            get { return eventSource; }
            set { eventSource = value; }
        }

        #endregion
        #region Constructors and initializers

        public Logger()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.eventSource = EventSource.None;
            this.writers = new List<LogWriterBase>();
        }

        #endregion

        public void Start(EventSource eventSource, bool attachConsole)
        {
            if (this.status != LoggerStatus.Stopped)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            this.status = LoggerStatus.Started;
            this.eventSource = eventSource;

            var f = new LogWriterFactory();

            foreach (var writer in f.GetLogWriters())
            {
                if (attachConsole && writer is ConsoleLogWriter ||
                    !(writer is ConsoleLogWriter))
                {
                    writers.Add(writer);
                }
            }

            foreach (var writer in writers)
            {
                writer.Start();
            }
        }

        public void Stop()
        {
            if (this.status != LoggerStatus.Started)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            foreach (var writer in writers)
            {
                writer.Stop();
            }

            writers.Clear();

            this.status = LoggerStatus.Stopped;
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

        /// <summary>
        /// Route a single event through the pipeline
        /// </summary>
        /// <param name="e"></param>
        public void RecordEvent(Event e)
        {
            var context = LoggingContext.Current;

            if (context == null)
            {
                throw new InvalidOperationException("No logging context available on the current thread."); // *** TODO
            }

            context.RecordEvent(e);
        }

        /// <summary>
        /// Write a single event directly to the writers
        /// </summary>
        /// <param name="e"></param>
        public void WriteEvent(Event e)
        {
            foreach (var writer in writers)
            {
                writer.WriteEvent(e);
            }
        }

        public MethodBase UnwindStack(int skip)
        {
            var stack = new StackTrace(skip, true);

            for (int i = 0; i < stack.FrameCount; i++)
            {
                var frame = stack.GetFrame(i);
                var method = frame.GetMethod();

                if (method.DeclaringType != typeof(Logger))
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
