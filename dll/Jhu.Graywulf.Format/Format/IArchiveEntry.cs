using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Exposes members of a generic archive file or directory entry.
    /// </summary>
    public interface IArchiveEntry
    {
        /// <summary>
        /// Gets if the entry is a directory.
        /// </summary>
        bool IsDirectory { get; }

        /// <summary>
        /// Gets the entry file name.
        /// </summary>
        string Filename { get; }
    }
}
