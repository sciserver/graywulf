using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    [Flags]
    public enum RunningState : int
    {
        Unknown = 0,
        Running = 1,
        Stopped = 2,
        Paused = 4,
        Attached = 9,
        Detached = 18,
    }
}
