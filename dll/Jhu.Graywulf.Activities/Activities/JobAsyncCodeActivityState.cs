using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    class JobAsyncCodeActivityState : IDisposable
    {
        private object syncRoot;
        private CancellationContext cancellationContext;
        private EventQueue eventQueue;

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public CancellationContext CancellationContext
        {
            get { return cancellationContext; }
        }

        public EventQueue EventQueue
        {
            get { return eventQueue; }
        }

        public JobAsyncCodeActivityState()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            syncRoot = new object();
            cancellationContext = new CancellationContext();
            eventQueue = new EventQueue();
        }

        public void Dispose()
        {
            cancellationContext.Dispose();
        }
    }
}
