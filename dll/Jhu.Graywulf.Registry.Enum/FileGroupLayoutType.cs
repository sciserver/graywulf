using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Layout type of the file group
    /// </summary>
    public enum FileGroupLayoutType : int
    {
        /// <summary>
        /// Default file group layout type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The file group is monolithic, no partitions
        /// </summary>
        Monolithic = 1,

        /// <summary>
        /// The file group is partitioned (sliced in multi-server terms)
        /// </summary>
        Sliced = 2,
    }
}
