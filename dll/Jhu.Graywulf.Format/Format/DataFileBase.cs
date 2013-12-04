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
        /// Read or write
        /// </summary>
        private DataFileMode fileMode;

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
        /// Gets the stream that can be used to read data
        /// </summary>
        protected virtual Stream BaseStream
        {
            get { return baseStream; }
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
        protected DataFileBase(Uri uri, DataFileMode fileMode, DataFileCompression compression)
        {
            InitializeMembers();

            this.uri = uri;
            this.fileMode = fileMode;
        }

        protected DataFileBase(Stream stream, DataFileMode fileMode, DataFileCompression compression)
        {
            InitializeMembers();

            this.baseStream = stream;
            this.fileMode = fileMode;
        }

        private void InitializeMembers()
        {
            this.baseStream = null;
            this.ownsBaseStream = false;

            this.fileMode = DataFileMode.Unknown;
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
        /// Makes sure that the base stream is not open, if
        /// stream is owned by the class.
        /// </summary>
        /// <remarks>
        /// When overriden in derived classes, the function can also
        /// check other specialized streams wrapping (or substituting)
        /// base stream.
        /// </remarks>
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
            EnsureNotOpen();

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

        private void OpenBaseStream()
        {
            if (baseStream == null)
            {
                // Use stream factory to open stream
                // TODO: replace this to use configured stream factory
                var f = StreamFactory.Create();
                baseStream = f.Open(uri, fileMode);

                ownsBaseStream = true;
            }
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

            OpenBaseStream();
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

            OpenBaseStream();
        }

        /// <summary>
        /// When overloaded in derived classes, closes the data file
        /// </summary>
        public virtual void Close()
        {
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
            get { return baseStream == null; }
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
