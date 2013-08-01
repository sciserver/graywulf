using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Deadlock priority of a concurrent query
    /// </summary>
    public enum DeadlockPriority
    {
        /// <summary>
        /// The deadlock priority of the transaction is low.
        /// </summary>
        Low,

        /// <summary>
        /// The deadlock priority of the transaction is normal.
        /// </summary>
        Normal,

        /// <summary>
        /// The deadlock priority of the transaction is high.
        /// </summary>
        High
    }
}
