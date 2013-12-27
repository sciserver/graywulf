using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a tar file output stream and exposes an interface to handle it
    /// as a general archive.
    /// </summary>
    public class TarOutputStream : ICSharpCode.SharpZipLib.Tar.TarOutputStream, IArchiveOutputStream
    {
        /// <summary>
        /// Creates a wrapper around a tar file stream.
        /// </summary>
        /// <param name="outputStream"></param>
        public TarOutputStream(System.IO.Stream outputStream)
            : base(outputStream)
        {
        }

        /// <summary>
        /// Creates a wrapper around a tar file stream.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="blockFactor"></param>
        public TarOutputStream(System.IO.Stream outputStream, int blockFactor)
            : base(outputStream, blockFactor)
        {
        }

        /// <summary>
        /// Appends a directory entry to the archive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IArchiveEntry CreateDirectoryEntry(string name)
        {
            var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateTarEntry(name);
            entry.TarHeader.Mode = 1003;    // Magic number
            entry.TarHeader.TypeFlag = ICSharpCode.SharpZipLib.Tar.TarHeader.LF_DIR;
            entry.TarHeader.Size = 0;
            return new TarEntry(entry);
        }

        /// <summary>
        /// Appends a file entry to the archive.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IArchiveEntry CreateFileEntry(string filename, long size)
        {
            var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateTarEntry(filename);
            entry.TarHeader.Mode = 33216;    // Magic number
            entry.TarHeader.TypeFlag = ICSharpCode.SharpZipLib.Tar.TarHeader.LF_NORMAL;
            entry.TarHeader.Size = size;
            return new TarEntry(entry);
        }

        /// <summary>
        /// Writes an entry into the file.
        /// </summary>
        /// <param name="entry"></param>
        public void WriteNextEntry(IArchiveEntry entry)
        {
            base.PutNextEntry((ICSharpCode.SharpZipLib.Tar.TarEntry)((TarEntry)entry).BaseEntry);
        }


    }
}
