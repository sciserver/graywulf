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
    class Logger
    {
        #region Private member variables

        private LoggerStatus status;
        private List<LogWriterBase> writers;

        #endregion
        #region Properties

        public LoggerStatus Status
        {
            get { return status; }
        }

        #endregion
        #region Constructors and initializers

        internal Logger()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.status = LoggerStatus.Stopped;
            this.writers = new List<LogWriterBase>();
        }

        #endregion

        public void Start(bool attachConsole)
        {
            if (this.status != LoggerStatus.Started)
            {
                this.status = LoggerStatus.Started;

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
        }

        public void Stop()
        {
            if (this.status != LoggerStatus.Stopped)
            {
                foreach (var writer in writers)
                {
                    writer.Stop();
                }

                writers.Clear();

                this.status = LoggerStatus.Stopped;
            }
        }

        /// <summary>
        /// Write a single event directly to the writers
        /// </summary>
        /// <param name="e"></param>
        public void WriteEvent(Event e)
        {
            if (this.status != LoggerStatus.Started)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            foreach (var writer in writers)
            {
                writer.WriteEvent(e);
            }
        }
    }
}
