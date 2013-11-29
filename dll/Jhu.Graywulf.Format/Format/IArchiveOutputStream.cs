using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Exposes members to handle a generic output archive stream.
    /// </summary>
    public interface IArchiveOutputStream
    {
        /// <summary>
        /// Appends a directory entry to the archive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IArchiveEntry CreateDirectoryEntry(string name);

        /// <summary>
        /// Appends a file entry to the archive.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IArchiveEntry CreateFileEntry(string filename, long size);

        /// <summary>
        /// Writes an entry into the file.
        /// </summary>
        /// <param name="entry"></param>
        void WriteNextEntry(IArchiveEntry entry);
    }
}
