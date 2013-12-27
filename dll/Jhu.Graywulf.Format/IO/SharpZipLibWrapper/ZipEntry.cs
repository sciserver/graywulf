using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a zip file entry and allows handling it as a general
    /// archive entry.
    /// </summary>
    public class ZipEntry : IArchiveEntry
    {
        /// <summary>
        /// Stores the original zip file entry objects.
        /// </summary>
        private ICSharpCode.SharpZipLib.Zip.ZipEntry baseEntry;

        /// <summary>
        /// Gets the original zip entry object.
        /// </summary>
        internal ICSharpCode.SharpZipLib.Zip.ZipEntry BaseEntry
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
        /// Creates a new wrapper around a zip file entry.
        /// </summary>
        /// <param name="baseEntry"></param>
        public ZipEntry(ICSharpCode.SharpZipLib.Zip.ZipEntry baseEntry)
        {
            this.baseEntry = baseEntry;
        }
    }
}
