using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    public interface IJob
    {
        Guid Guid { get; }
        string Name { get; }
        string Type { get; }
        string UserName { get; }
        Guid QueueGuid { get; }
        DateTime TimeStarted { get; }
        TimeSpan Timeout { get; }
        JobStatus Status { get; }
    }
}
