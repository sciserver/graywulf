using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Entity groups for displaying entities
    /// </summary>
    [Flags]
    public enum EntityGroup : int
    {
        /// <summary>
        /// Default
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Hardware related
        /// </summary>
        Cluster = 1,

        /// <summary>
        /// Security related
        /// </summary>
        Domain = 2,

        /// <summary>
        /// Logical layout related
        /// </summary>
        Federation = 4,

        /// <summary>
        /// Logical to hardware mapping layout
        /// </summary>
        Layout = 8,

        /// <summary>
        /// Job and scheduling related
        /// </summary>
        Jobs = 16,

        /// <summary>
        /// Logging related
        /// </summary>
        Log = 32,

        /// <summary>
        /// System monitor related
        /// </summary>
        Monitor = 64,

        /// <summary>
        /// All groups
        /// </summary>
        All = Cluster | Federation | Layout | Domain | Log | Jobs | Monitor
    }
}
