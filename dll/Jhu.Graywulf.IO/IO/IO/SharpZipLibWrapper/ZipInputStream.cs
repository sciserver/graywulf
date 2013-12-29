using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a zip file reader stream and exposes an interface that allows handling
    /// it as a general archive.
    /// </summary>
    public class ZipInputStream : ICSharpCode.SharpZipLib.Zip.ZipInputStream, IArchiveInputStream
    {
        /// <summary>
        /// Creates a wrapper around a zip file input stream.
        /// </summary>
        /// <param name="baseInputStream"></param>
        public ZipInputStream(System.IO.Stream baseInputStream)
            :base(baseInputStream)
        {
        }

        /// <summary>
        /// Creates a wrapper around a zip file input stream.
        /// </summary>
        /// <param name="baseInputStream"></param>
        /// <param name="bufferSize"></param>
        public ZipInputStream(System.IO.Stream baseInputStream, int bufferSize)
            : base(baseInputStream, bufferSize)
        {
        }

        /// <summary>
        /// Advances the stream to next file entry.
        /// </summary>
        /// <returns></returns>
        public IArchiveEntry ReadNextFileEntry()
        {
            return new ZipEntry(GetNextEntry());
        }
    }
}
