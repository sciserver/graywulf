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

        private List<LogWriterBase> writers;

        public List<LogWriterBase> Writers
        {
            get { return writers; }
        }

        public Logger()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.writers = new List<LogWriterBase>();
        }

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

        public void LogEvent(Event e)
        {
            lock (this)
            {
                foreach (LogWriterBase writer in writers)
                {
                    if ((writer.SourceMask & e.EventSource) > 0)
                    {
                        writer.WriteEvent(e);
                    }
                }
            }
        }

        public Event LogException(string operation, EventSource source, Guid userGuid, Guid contextGuid, Exception ex)
        {
            var errorEvent = new Logging.Event(operation, Guid.Empty)
            {
                Exception = ex,
                UserGuid = userGuid,
                ContextGuid = contextGuid,
                EventSource = source,
            };

            Logging.Logger.Instance.LogEvent(errorEvent);

            return errorEvent;
        }
    }
}
