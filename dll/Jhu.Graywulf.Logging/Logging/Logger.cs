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
        }

        private void InitializeMembers()
        {
            this.writers = new List<LogWriter>();
        }

        public void LogEvent(Event e)
        {
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
