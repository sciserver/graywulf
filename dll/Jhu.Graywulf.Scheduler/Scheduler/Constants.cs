using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    public static class Constants
    {
        public const string DefaultRole = "Default";

        public static readonly TimeSpan DrainStopTimeout = TimeSpan.FromSeconds(120);
        public static readonly TimeSpan UnloadAppDomainTimeout = TimeSpan.FromSeconds(30);

        public static readonly TimeSpan WorkflowPollingInterval = TimeSpan.FromMilliseconds(100);
    }
}
