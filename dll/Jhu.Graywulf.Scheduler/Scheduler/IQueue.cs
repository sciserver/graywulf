using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    public interface IQueue : IRegistryObject
    {
        int MaxOutstandingJobs { get; }
        int JobCount { get; }
        TimeSpan Timeout { get; }
    }
}
