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
        None = 0,

        /// <summary>
        /// Hardware related
        /// </summary>
        Cluster = 1,

        /// <summary>
        /// Logical layout related
        /// </summary>
        Federation = 2,

        /// <summary>
        /// Logical to hardware mapping layout
        /// </summary>
        Layout = 4,

        /// <summary>
        /// Security related
        /// </summary>
        Security = 8,

        /// <summary>
        /// Logging related
        /// </summary>
        Log = 16,

        /// <summary>
        /// Job and scheduling related
        /// </summary>
        Jobs = 32,

        /// <summary>
        /// System monitor related
        /// </summary>
        Monitor = 64,

        /// <summary>
        /// All groups
        /// </summary>
        All = Cluster | Federation | Layout | Security | Log | Jobs | Monitor
    }
}
