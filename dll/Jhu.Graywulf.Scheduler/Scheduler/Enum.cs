using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Possible workflow events
    /// </summary>
    public enum WorkflowEventType
    {
        Unknown,

        /// <summary>
        /// Workflow completed without unhandled exceptions.
        /// </summary>
        Completed,

        /// <summary>
        /// Workflow forcefully cancelled, either by the user, either because of timeout.
        /// </summary>
        Cancelled,

        TimedOut,

        Failed,
        Persisted,
    }

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
