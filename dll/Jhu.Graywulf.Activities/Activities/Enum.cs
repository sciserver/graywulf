using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// Possible workflow events
    /// </summary>
    public enum WorkflowEventType
    {
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
}
