using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Provides basic support for file-based DataReader implementations.
    /// </summary>
    [Serializable]
    public abstract class DataFileBase : IDisposable
    {
        #region Private member variables

        /// <summary>
        /// Base stream to read from/write to
        /// </summary>
        /// <remarks>
        /// Either set by the constructor (in this case the stream is not owned)
        /// or opened internally (owned)
        /// </remarks>
        [NonSerialized]
        private Stream baseStream;

        /// <summary>
        /// If true, baseStream was opened by the object and will need
        /// to be closed when disposing.
        /// </summary>
        [NonSerialized]
        private bool ownsBaseStream;

        /// <summary>
        /// Compression/decompression stream that wraps baseStream if data is
        /// compressed.
        /// </summary>
        [NonSerialized]
        private Stream uncompressedStream;

        /// <summary>
        /// Read or write
        /// </summary>
        private DataFileMode fileMode;

        /// <summary>
        /// Type of compression
        /// </summary>
        private CompressionMethod compression;

        /// <summary>
        /// Uri to the file. If set, the class can open it internally.
        /// </summary>
        private Uri uri;

        /// <summary>
        /// Determines if an identity column is automatically generated.
        /// </summary>
        private bool generateIdentityColumn;

        /// <summary>
        /// Stores the block of the file, as they are read from the input
        /// </summary>
        private List<DataFileBlockBase> blocks;

        /// <summary>
        /// Points to the current block in the blocks collection
        /// </summary>
        private int blockCounter;


        #endregion
        #region Properties

        /// <summary>
        /// Returns file format description.
        /// </summary>
        public abstract FileFormatDescription Description { get; }

        /// <summary>
        /// Gets the (uncompressed) stream that can be used to read data
        /// </summary>
        protected virtual Stream Stream
        {
            get { return uncompressedStream ?? baseStream; }
        }

        /// <summary>
        /// Gets or sets file mode (read or write)
        /// </summary>
        public DataFileMode FileMode
        {
            get { return fileMode; }
            set
            {
                EnsureNotOpen();
                fileMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the compression method.
        /// </summary>
        public CompressionMethod Compression
        {
            get { return compression; }
            set
            {
                EnsureNotOpen();
                compression = value;
            }
        }

        public Uri Uri
        {
            get { return uri; }
            set
            {
                EnsureNotOpen();
                uri = value;
            }
        }

        public bool GenerateIdentityColumn
        {
            get { return generateIdentityColumn; }
            set { generateIdentityColumn = value; }
        }

        protected List<DataFileBlockBase> Blocks
        {
            get { return blocks; }
        }

        internal DataFileBlockBase CurrentBlock
        {
            get { return blocks[blockCounter]; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Constructs a file without opening it
        /// </summary>
        protected DataFileBase()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructs a file and opens a stream automatically
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        protected DataFileBase(Uri uri, DataFileMode fileMode, CompressionMethod compression)
        {
            InitializeMembers();

            this.uri = uri;
            this.fileMode = fileMode;
            this.compression = compression;
        }

        protected DataFileBase(Stream stream, DataFileMode fileMode, CompressionMethod compression)
        {
            InitializeMembers();

            this.baseStream = stream;
            this.fileMode = fileMode;
            this.compression = compression;
        }

        private void InitializeMembers()
        {
            this.baseStream = null;
            this.ownsBaseStream = false;
            this.uncompressedStream = null;

            this.fileMode = DataFileMode.Unknown;
            this.compression = CompressionMethod.None;
            this.uri = null;

            this.blocks = new List<DataFileBlockBase>();
            this.blockCounter = -1;
        }

        public virtual void Dispose()
        {
            Close();
        }

        #endregion
        #region Stream open/close

        /// <summary>
        /// If compression is set to automatic, figures out compression
        /// method from the file extension
        /// </summary>
        /// <returns></returns>
        private CompressionMethod GetCompressionMethod()
        {
            // Open compressed stream, if necessary
            var cm = compression;

            if (cm == CompressionMethod.Automatic)
            {
                var path = uri.IsAbsoluteUri ? uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : uri.ToString();
                var extension = Path.GetExtension(path).ToLowerInvariant();

                switch (extension)
                {
                    case ".gz":
                        cm = CompressionMethod.GZip;
                        break;
                    case ".bz2":
                        cm = CompressionMethod.BZip2;
                        break;
                    case ".zip":
                        cm = CompressionMethod.Zip;
                        break;
                    default:
                        cm = CompressionMethod.None;
                        break;
                }
            }

            return cm;
        }

        /// <summary>
        /// Makes sure that the base stream is not open, if
        /// stream is owned by the class.
        /// </summary>
        protected virtual void EnsureNotOpen()
        {
            if (ownsBaseStream && baseStream != null)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Opens the base stream for read or write
        /// </summary>
        public void Open()
        {
            switch (fileMode)
            {
                case DataFileMode.Read:
                    OpenForRead();
                    break;
                case DataFileMode.Write:
                    OpenForWrite();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Opens a file by wrapping a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="compression"></param>
        public virtual void Open(Stream stream, DataFileMode fileMode)
        {
            EnsureNotOpen();

            this.baseStream = stream;
            this.ownsBaseStream = false;

            this.fileMode = fileMode;
        }

        /// <summary>
        /// When overloaded in derived classes, opens the data file for reading
        /// </summary>
        protected virtual void OpenForRead()
        {
            if (FileMode != DataFileMode.Read)
            {
                throw new InvalidOperationException();
            }

            EnsureNotOpen();

            // Open stream, if necessary
            if (baseStream == null)
            {
                OpenOwnStreamForRead();
            }

            // Wrap into an uncompressed stream, if necessary
            if (uncompressedStream == null)
            {
                WrapCompressedStreamForRead();
            }
        }

        private void OpenOwnStreamForRead()
        {
            if (!uri.IsAbsoluteUri || uri.IsFile)
            {
                // Open a file
                baseStream = new FileStream(uri.ToString(), System.IO.FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                // Open a stream

                switch (uri.Scheme.ToLowerInvariant())
                {
                    case "http":
                    case "https":
                    case "ftp":
                    default:
                        throw new NotImplementedException();

                    //TODO implement net protocols
                }
            }

            ownsBaseStream = true;
        }

        private void WrapCompressedStreamForRead()
        {
            switch (GetCompressionMethod())
            {
                case CompressionMethod.None:
                    break;
                case CompressionMethod.GZip:
                    uncompressedStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(baseStream);
                    break;
                case CompressionMethod.BZip2:
                    uncompressedStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(baseStream);
                    break;
                case CompressionMethod.Zip:
                    uncompressedStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(baseStream);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overloaded in derived class, opens the data file for writing
        /// </summary>
        protected virtual void OpenForWrite()
        {
            if (FileMode != DataFileMode.Write)
            {
                throw new InvalidOperationException();
            }

            EnsureNotOpen();

            if (baseStream == null)
            {
                // No open stream yet
                if (uri.IsFile)
                {
                    baseStream = new FileStream(uri.PathAndQuery, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
                }
                else
                {
                    throw new NotImplementedException();
                }
                ownsBaseStream = true;
            }

            // Open compressed stream, if necessary
            switch (compression)
            {
                case CompressionMethod.None:
                    break;
                case CompressionMethod.GZip:
                    uncompressedStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(baseStream);
                    break;
                case CompressionMethod.BZip2:
                    uncompressedStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(baseStream);
                    break;
                case CompressionMethod.Zip:
                    uncompressedStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(baseStream);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overloaded in derived classes, closes the data file
        /// </summary>
        public virtual void Close()
        {
            if (uncompressedStream != null)
            {
                uncompressedStream.Close();
                uncompressedStream.Dispose();
                uncompressedStream = null;
            }

            if (ownsBaseStream && baseStream != null)
            {
                baseStream.Close();
                baseStream.Dispose();
                baseStream = null;
                ownsBaseStream = false;
            }
        }

        /// <summary>
        /// Returns true if the underlying data file is closed
        /// </summary>
        public virtual bool IsClosed
        {
            get { return Stream == null; }
        }

        #endregion

        /// <summary>
        /// When overloaded in a derived class, read the file header.
        /// </summary>
        protected internal abstract void OnReadHeader();

        public void AppendBlock(DataFileBlockBase block)
        {
            switch (fileMode)
            {
                case DataFileMode.Read:
                    // For read, blocks can be added to predefine columns, but otherwise
                    // no need to do anything, just write them to the end of the list
                    blocks.Add(block);
                    OnBlockAppended(block);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected abstract void OnBlockAppended(DataFileBlockBase block);

        /// <summary>
        /// Advanced the file to the next block.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Blocks can be predefined, in case data columns are set externally.
        /// </remarks>
        public DataFileBlockBase ReadNextBlock()
        {
            if (blockCounter == -1)
            {
                OnReadHeader();
            }
            else
            {
                blocks[blockCounter].OnReadToFinish();
                blocks[blockCounter].OnReadFooter();
            }

            try
            {
                blockCounter++;

                DataFileBlockBase nextBlock;

                if (blockCounter < blocks.Count)
                {
                    nextBlock = OnReadNextBlock(blocks[blockCounter]);
                }
                else
                {
                    // Create a new block automatically, if collection is not predefined
                    nextBlock = OnReadNextBlock(null);
                    if (nextBlock != null)
                    {
                        blocks.Add(nextBlock);
                    }
                }

                if (nextBlock != null)
                {
                    nextBlock.OnReadHeader();
                    return nextBlock;
                }
                else
                {
                    // If no more blocks, read file footer
                    OnReadFooter();
                }
            }
            catch (EndOfStreamException)
            {
                // Some data formats cannot detect end of blocks and will
                // throw exception at the end of the file instead
            }

            blockCounter = -1;
            return null;
        }

        /// <summary>
        /// When overloaded in a derived class, reads the next block.
        /// </summary>
        /// <returns></returns>
        protected abstract DataFileBlockBase OnReadNextBlock(DataFileBlockBase block);

        protected internal abstract void OnReadFooter();

        protected abstract void OnWriteHeader();

        private void WriteNextBlock(IDataReader dr)
        {
            blockCounter++;

            DataFileBlockBase nextBlock;

            if (blockCounter < blocks.Count)
            {
                nextBlock = OnWriteNextBlock(blocks[blockCounter], dr);
            }
            else
            {
                // Create a new block automatically, if collection is not predefined
                nextBlock = OnWriteNextBlock(null, dr);
                if (nextBlock != null)
                {
                    blocks.Add(nextBlock);
                }
            }

            if (nextBlock != null)
            {
                nextBlock.DetectColumns(dr);
                nextBlock.Write(dr);
            }
        }

        protected abstract void OnWriteFooter();

        protected abstract DataFileBlockBase OnWriteNextBlock(DataFileBlockBase block, IDataReader dr);

        #region DataReader and Writer functions
        /// <summary>
        /// Returns a FileDataReader that can iterate through the rows of
        /// the data file.
        /// </summary>
        /// <returns></returns>
        public FileDataReader OpenDataReader()
        {
            return new FileDataReader(this);
        }

        public void WriteFromDataReader(IDataReader dr)
        {
            OpenForWrite();

            OnWriteHeader();

            do
            {
                WriteNextBlock(dr);
            }
            while (dr.NextResult());

            OnWriteFooter();

            Close();
        }

        #endregion
    }
}
