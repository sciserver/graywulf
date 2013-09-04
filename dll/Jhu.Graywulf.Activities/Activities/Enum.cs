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
        Completed,
        Cancelled,
        Failed,
    }
}
