using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the file group
    /// </summary>
    public enum FileGroupType : int
    {
        /// <summary>
        /// Default file group type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The file group contains data files
        /// </summary>
        Data = 1,

        /// <summary>
        /// The file group contains log files
        /// </summary>
        Log = 2,
    }
}
