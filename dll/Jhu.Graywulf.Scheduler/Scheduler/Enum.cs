using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    public enum RunningState
    {
        Up,
        Down
    }

    /// <summary>
    /// Job execution status
    /// </summary>
    public enum JobStatus
    {
        Unknown,
        Starting,
        Resuming,
        Executing,
        TimedOut,
        Persisted,
        Failed,
        Cancelled,
    }
}
