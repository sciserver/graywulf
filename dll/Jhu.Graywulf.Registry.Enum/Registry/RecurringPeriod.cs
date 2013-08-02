using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Recurring period of recurring job instances
    /// </summary>
    public enum RecurringPeriod : int
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// The job is rescheduled on a daily basis.
        /// </summary>
        Daily,

        /// <summary>
        /// The job is rescheduled on a weekly basis.
        /// </summary>
        Weekly,

        /// <summary>
        /// The job is rescheduled on a monthly basis.
        /// </summary>
        Monthly
    }
}
