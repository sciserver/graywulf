using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Execution status of a job instance
    /// </summary>
    [Flags]
    public enum JobExecutionState : int
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The job is waiting to be picked up by the scheduler.
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// The job is picked up by the scheduler and waiting for execution
        /// </summary>
        Starting = 2,

        /// <summary>
        /// The job is being executed by the workflow runtime.
        /// </summary>
        Executing = 4,

        /// <summary>
        /// The job has been completed.
        /// </summary>
        Completed = 8,

        Persisting = 16,

        /// <summary>
        /// The job is loaded by the scheduler into the workflow runtime.
        /// </summary>
        Persisted = 32,

        /// <summary>
        /// The job has been failed.
        /// </summary>
        Failed = 64,

        /// <summary>
        /// Cancel requested by the user
        /// </summary>
        CancelRequested = 128,

        /// <summary>
        /// Cancel request accepted by the scheduler
        /// </summary>
        Cancelling = 256,

        /// <summary>
        /// Job canceled by the user
        /// </summary>
        Cancelled = 512,

        /// <summary>
        /// Job timed-out and canceled by the scheduler.
        /// </summary>
        TimedOut = 1024,

        /// <summary>
        /// The job has been suspended and requires human interaction
        /// </summary>
        Suspended = 2048,

        /// <summary>
        /// The job has been resumed from the suspended state and waiting in
        /// the queue to be picked up by the scheduler.
        /// </summary>
        Resumed = 4096,

        All = 0xFFFF
    }
}
