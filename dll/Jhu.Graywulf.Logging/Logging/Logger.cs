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

        private List<LogWriterBase> writers;

        #endregion
        #region Properties

        #endregion
        #region Constructors and initializers

        public Logger()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.writers = new List<LogWriterBase>();
        }

        #endregion

        public void Start(bool attachConsole)
        {
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
            foreach (var writer in writers)
            {
                writer.Stop();
            }

            writers.Clear();
        }


        public Event LogDebug(string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Debug, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogInfo(string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Info, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogStatus(string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Status, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogWarning(string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Warning, message, operation, null, data);
            RecordEvent(e);
            return e;
        }

        public Event LogError(Exception ex, string message = null, string operation = null, Dictionary<string, object> data = null)
        {
            var e = CreateEvent(EventSeverity.Error, message, operation, ex, data);
            RecordEvent(e);
            return e;
        }

        public Event CreateEvent(EventSeverity severity, string message, string operation, Exception ex, Dictionary<string, object> data)
        {
            var e = new Event();

            // TODO: allow overwriting operation?
            if (ex == null)
            {
                e.Severity = severity;
                e.Message = message;
                e.Operation = operation;
            }
            else
            {
                ex = UnwrapException(ex);

                e.Severity = Logging.EventSeverity.Error;
                e.Message = GetExceptionMessage(ex);
                e.Operation = GetExceptionOperation(ex);
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

        protected virtual string UnwindStack()
        {
            var stack = new StackFrame();
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
            MethodBase site = ex.TargetSite;
            string methodName = site == null ? null : site.Name;
            return methodName;
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
