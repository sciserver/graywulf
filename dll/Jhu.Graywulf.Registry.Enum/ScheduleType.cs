using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Method of scheduling of a job instance
    /// </summary>
    public enum ScheduleType : int
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The job is queued in a queue and will be picked up when it is the
        /// first in the queue.
        /// </summary>
        Queued = 1,

        /// <summary>
        /// The job is scheduled at a given time and will be picked up after all
        /// previous queued jobs and scheduled jobs are completed and the
        /// schedule time has passed.
        /// </summary>
        Timed = 2,

        /// <summary>
        /// The job is scheduled at a given time and must be rescheduled after
        /// it has been completed.
        /// </summary>
        Recurring = 3
    }
}
