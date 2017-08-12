using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// This class is currently used for debugging purposes
    /// </summary>
    [Serializable]
    public class SchedulerDebugOptions
    {
        public int InstanceCount { get; set; } = 1;
        public bool IsLayoutRequired { get; set; } = true;
        public bool IsControlServiceEnabled { get; set; } = true;
    }
}
