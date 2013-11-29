using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Exposes methods to handle a generic archive input stream.
    /// </summary>
    public interface IArchiveInputStream
    {
        /// <summary>
        /// Advances the stream to next file entry.
        /// </summary>
        /// <returns></returns>
        IArchiveEntry ReadNextFileEntry();
    }
}
