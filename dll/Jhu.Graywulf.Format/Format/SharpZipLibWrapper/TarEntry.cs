using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a tar file entry and exposes an interface around it
    /// to allow managing all archives similarly
    /// </summary>
    public class TarEntry : IArchiveEntry
    {
        /// <summary>
        /// Stores the original tar entry objects
        /// </summary>
        private ICSharpCode.SharpZipLib.Tar.TarEntry baseEntry;

        /// <summary>
        /// Gets the original tar entry object.
        /// </summary>
        internal ICSharpCode.SharpZipLib.Tar.TarEntry BaseEntry
        {
            get { return baseEntry; }
        }

        /// <summary>
        /// Gets if the entry is a directory.
        /// </summary>
        public bool IsDirectory
        {
            get { return baseEntry.IsDirectory; }
        }

        /// <summary>
        /// Gets the entry file name.
        /// </summary>
        public string Filename
        {
            get { return baseEntry.Name; }
        }

        /// <summary>
        /// Creates a new wrapper around a tar file entry.
        /// </summary>
        /// <param name="baseEntry"></param>
        public TarEntry(ICSharpCode.SharpZipLib.Tar.TarEntry baseEntry)
        {
            this.baseEntry = baseEntry;
        }
    }
}
