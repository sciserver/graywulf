using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a zip file output stream and exposes an interface to handle it
    /// as a general archive.
    /// </summary>
    public class ZipOutputStream : ICSharpCode.SharpZipLib.Zip.ZipOutputStream, IArchiveOutputStream
    {
        /// <summary>
        /// Creates a wrapper around a zip file stream.
        /// </summary>
        /// <param name="baseOutputStream"></param>
        public ZipOutputStream(System.IO.Stream baseOutputStream)
            : base(baseOutputStream)
        {
        }

        /// <summary>
        /// Creates a wrapper around a zip file stream.
        /// </summary>
        /// <param name="baseOutputStream"></param>
        /// <param name="bufferSize"></param>
        public ZipOutputStream(System.IO.Stream baseOutputStream, int bufferSize)
            : base(baseOutputStream, bufferSize)
        {
        }

        /// <summary>
        /// Appends a directory entry to the archive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IArchiveEntry CreateDirectoryEntry(string name)
        {
            var f = new ICSharpCode.SharpZipLib.Zip.ZipEntryFactory();
            return new ZipEntry(f.MakeDirectoryEntry(name));
        }

        /// <summary>
        /// Appends a file entry to the archive.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IArchiveEntry CreateFileEntry(string filename, long size)
        {
            var f = new ICSharpCode.SharpZipLib.Zip.ZipEntryFactory();
            return new ZipEntry(f.MakeFileEntry(filename));
        }

        /// <summary>
        /// Writes an entry into the file.
        /// </summary>
        /// <param name="entry"></param>
        public void WriteNextEntry(IArchiveEntry entry)
        {
            base.PutNextEntry((ICSharpCode.SharpZipLib.Zip.ZipEntry)((ZipEntry)entry).BaseEntry);
        }


    }
}
