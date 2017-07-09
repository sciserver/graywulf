using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    public abstract class LogWriterBase : IDisposable
    {
        private EventSource sourceMask;
        private EventSeverity severityMask;
        private ExecutionStatus statusMask;

        public EventSource SourceMask
        {
            get { return sourceMask; }
            set { sourceMask = value; }
        }

        public EventSeverity SeverityMask
        {
            get { return severityMask; }
            set { severityMask = value; }
        }

        public ExecutionStatus StatusMask
        {
            get { return statusMask; }
            set { statusMask = value; }
        }

        public LogWriterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            sourceMask = EventSource.All;
            severityMask = EventSeverity.All;
            statusMask = ExecutionStatus.All;
        }

        public virtual void Dispose()
        {
        }

        public abstract void Start();

        public abstract void Stop();

        public void WriteEvent(Event e)
        {
            // Enforce mask

            if ((e.Source & this.SourceMask) != 0 &&
                (e.Severity & this.SeverityMask) != 0 &&
                (e.ExecutionStatus & this.StatusMask) != 0)
            {
                OnWriteEvent(e);
            }
        }

        protected virtual void OnWriteEvent(Event e)
        {
        }
    }
}
