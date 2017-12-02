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

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public CancellationContext CancellationContext
        {
            get { return cancellationContext; }
        }

        public JobAsyncCodeActivityState()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            syncRoot = new object();
            cancellationContext = new CancellationContext();
        }

        public void Dispose()
        {
            cancellationContext.Dispose();
        }
    }
}
