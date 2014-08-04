using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Kind of dependency between two jobs
    /// </summary>
    [Flags]
    public enum JobDependencyType : int
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Predecessor job must have completed to proceed.
        /// </summary>
        Completed = 8,

        /// <summary>
        /// Predecessor job must have failed to proceed.
        /// </summary>
        Failed = 64,

        /// <summary>
        /// Predecessor job must have been cancelled to proceed.
        /// </summary>
        Cancelled = 512,

        /// <summary>
        /// Predecessor job must have timed out to proceed.
        /// </summary>
        TimedOut = 1024,
    }
}
