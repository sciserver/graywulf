using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class Logger
    {
        public static readonly Logger Instance = new Logger();

        private List<LogWriter> writers;

        public List<LogWriter> Writers
        {
            get { return writers; }
        }

        public Logger()
        {
            InitializeMembers();
            InitializeWriters();
        }

        private void InitializeMembers()
        {
            this.writers = new List<LogWriter>();
        }

        private void InitializeWriters()
        {
            if (AppSettings.ConnectionString != null)
            {
                SqlLogWriter sqlwriter = new SqlLogWriter();
                writers.Add(sqlwriter);
            }
        }

        public void LogEvent(Event e)
        {
            // Unwrap aggregate exceptions
            if (e.Exception != null && e.Exception is AggregateException)
            {
                Exception ex = e.Exception;
                while (ex != null && ex is AggregateException)
                {
                    ex = ex.InnerException;
                }

                e.Exception = ex;
                e.ExceptionType = ex.GetType().ToString();
            }

            lock (this)
            {
                foreach (LogWriter writer in writers)
                {
                    if ((writer.EventSourceMask & e.EventSource) > 0)
                    {
                        writer.WriteEvent(e);
                    }
                }
            }
        }
    }
}
