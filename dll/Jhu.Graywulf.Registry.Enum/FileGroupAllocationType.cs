using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the allocation of the file group
    /// </summary>
    public enum FileGroupAllocationType : int
    {
        /// <summary>
        /// Default allocation type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The file group is assigned to a dedicated disk volume
        /// </summary>
        SingleVolume = 1,

        /// <summary>
        /// The file group has files on multiple disk volumes
        /// </summary>
        CrossVolume = 2,
    }
}
