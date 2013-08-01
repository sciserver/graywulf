using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the database file
    /// </summary>
    public enum DatabaseFileType : int
    {
        /// <summary>
        /// Default database file type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The database file contains data
        /// </summary>
        Data = 1,

        /// <summary>
        /// The database file contains log
        /// </summary>
        Log = 2
    }
}
