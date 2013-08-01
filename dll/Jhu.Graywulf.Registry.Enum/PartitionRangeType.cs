using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the partition range
    /// </summary>
    public enum PartitionRangeType : int
    {
        /// <summary>
        /// The partition function is created as LEFT
        /// </summary>
        Left = 0,

        /// <summary>
        /// The partition function is created as RIGHT
        /// </summary>
        Right = 1,
    }
}
