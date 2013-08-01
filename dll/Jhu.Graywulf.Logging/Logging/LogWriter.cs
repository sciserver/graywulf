using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Logging
{
    public class LogWriter
    {
        private readonly object syncRoot = new object();
        private EventSource sourceMask;

        public EventSource EventSourceMask
        {
            get { return sourceMask; }
            set { sourceMask = value; }
        }

        public LogWriter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            sourceMask = EventSource.All;
        }

        public virtual void WriteEvent(Event e)
        {
        }
    }
}
