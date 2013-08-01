using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the database layout
    /// </summary>
    public enum DatabaseLayoutType : int
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The database is monolithic
        /// </summary>
        Monolithic = 1,

        /// <summary>
        /// The database is mirrored on multiple machines
        /// </summary>
        Mirrored = 2,

        /// <summary>
        /// The database is sliced and distributed accross machines
        /// </summary>
        Sliced = 3
    }
}
