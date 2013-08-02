using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the logical disk volume
    /// </summary>
    [Flags]
    public enum DiskVolumeType : int
    {
        /// <summary>
        /// Default disk volume type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Disk volume is a system volume
        /// </summary>
        System = 1,

        /// <summary>
        /// Disk volume is dedicated to log files
        /// </summary>
        Log = 2,

        /// <summary>
        /// Disk volume is dedicated for temporary files
        /// </summary>
        Temporary = 4,

        /// <summary>
        /// Disk volume is dedicated for data files
        /// </summary>
        Data = 8
    }
}
