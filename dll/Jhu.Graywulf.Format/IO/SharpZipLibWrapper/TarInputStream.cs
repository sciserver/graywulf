using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.SharpZipLibWrapper
{
    /// <summary>
    /// Wraps a tar file reader stream and exposes an interface that allows handling
    /// it as a general archive.
    /// </summary>
    public class TarInputStream : ICSharpCode.SharpZipLib.Tar.TarInputStream, IArchiveInputStream
    {
        /// <summary>
        /// Creates a wrapper around a tar file input stream.
        /// </summary>
        /// <param name="inputStream"></param>
        public TarInputStream(System.IO.Stream inputStream)
            : base(inputStream)
        {
        }

        /// <summary>
        /// Creates a wrapper around a tar file input stream.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="blockFactor"></param>
        public TarInputStream(System.IO.Stream inputStream, int blockFactor)
            : base(inputStream, blockFactor)
        {
        }

        /// <summary>
        /// Advances the stream to next file entry.
        /// </summary>
        /// <returns></returns>
        public IArchiveEntry ReadNextFileEntry()
        {
            return new TarEntry(GetNextEntry());
        }
    }
}
