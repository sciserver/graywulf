using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Provides basic support for file-based DataReader implementations.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class DataFileBase : IDisposable, ICloneable
    {
        #region Private member variables

        [NonSerialized]
        private FileFormatDescription description;

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
        /// Type name of the stream factory to use when opening the stream automatically.
        /// </summary>
        [NonSerialized]
        private StreamFactory streamFactory;

        /// <summary>
        /// Read or write
        /// </summary>
        [NonSerialized]
        private DataFileMode fileMode;

        /// <summary>
        /// Compress file or read compressed file
        /// </summary>
        [NonSerialized]
        private DataFileCompression compression;

        /// <summary>
        /// Uri to the file. If set, the class can open it internally.
        /// </summary>
        [NonSerialized]
        private Uri uri;

        /// <summary>
        /// Credentials to access the URI pointing to the file. Used
        /// by the stream facroty to automatically open the URI as
        /// a stream.
        /// </summary>
        [NonSerialized]
        private Credentials credentials;

        /// <summary>
        /// Determines if an identity column is automatically generated.
        /// </summary>
        [NonSerialized]
        private bool generateIdentityColumn;

        /// <summary>
        /// Name of the data file to be used as the dataset name by SmartDataReader
        /// and consequently in table naming patterns.
        /// </summary>
        [NonSerialized]
        private string name;

        [NonSerialized]
        private DatasetMetadata metadata;

        /// <summary>
        /// Stores the blocks of the file, as they are read from the input
        /// or added programatically when writing the file.
        /// </summary>
        [NonSerialized]
        private List<DataFileBlockBase> blocks;

        /// <summary>
        /// Points to the current block in the blocks collection
        /// </summary>
        /// <remarks>
        /// This can be different from blocks.Count as blocks can be
        /// predefined by the user or automatically generated as new
        /// blocks are discovered while reading the file.
        /// </remarks>
        [NonSerialized]
        private int blockCounter;

        #endregion
        #region Properties

        /// <summary>
        /// Returns file format description.
        /// </summary>
        public FileFormatDescription Description
        {
            get { return description; }
            protected set { description = value; }
        }

        /// <summary>
        /// Gets the stream that can be used to read data
        /// </summary>
        [IgnoreDataMember]
        public virtual Stream BaseStream
        {
            get { return baseStream; }
            set { baseStream = value; }
        }

        /// <summary>
        /// Gets or sets the stream factory to
        /// use when opening the base stream automatically.
        /// </summary>
        [IgnoreDataMember]
        public StreamFactory StreamFactory
        {
            get { return streamFactory; }
            set { streamFactory = value; }
        }

        /// <summary>
        /// Gets or sets file mode (read or write)
        /// </summary>
        [IgnoreDataMember]
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
        /// Gets or sets file compression method.
        /// </summary>
        [DataMember]
        public DataFileCompression Compression
        {
            get { return compression; }
            set
            {
                EnsureNotOpen();
                compression = value;
            }
        }

        /// <summary>
        /// Gets or sets the location of the file
        /// </summary>
        /// <remarks>
        /// For archives, use relative URI
        /// </remarks>
        [DataMember]
        public Uri Uri
        {
            get { return uri; }
            set
            {
                EnsureNotOpen();
                uri = value;
            }
        }

        /// <summary>
        /// Gets or sets the credentials to be used to access the URI.
        /// </summary>
        /// <remarks>
        /// Do not use for file within archives.
        /// </remarks>
        [DataMember]
        public Credentials Credentials
        {
            get { return credentials; }
            set
            {
                EnsureNotOpen();
                credentials = value;
            }
        }

        /// <summary>
        /// Gets or sets if an identity column is to be generated automatically
        /// when reading tables from a file.
        /// </summary>
        [DataMember]
        public bool GenerateIdentityColumn
        {
            get { return generateIdentityColumn; }
            set { generateIdentityColumn = value; }
        }

        /// <summary>
        /// Gets or sets the name of this file that can be used
        /// by the FileDataReader as dataset name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets metadata associated with this file
        /// </summary>
        [DataMember]
        public DatasetMetadata Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        /// <summary>
        /// Gets a collection of data file blocks.
        /// </summary>
        /// <remarks>
        /// Data file blocks can be created manually, in this case the class
        /// will reuse them when reading/writing files. This method allows
        /// defining column mappings. If no blocks are created manually, they
        /// will be created automatically during read/write with default settings
        /// and column.
        /// </remarks>
        [IgnoreDataMember]
        protected List<DataFileBlockBase> Blocks
        {
            get { return blocks; }
        }

        [DataMember(Name = "Blocks")]
        private DataFileBlockBase[] Blocks_ForXml
        {
            get { return blocks.ToArray(); }
            set { blocks = new List<DataFileBlockBase>(value); }
        }

        /// <summary>
        /// Gets a reference to the current data file block.
        /// </summary>
        [IgnoreDataMember]
        internal DataFileBlockBase CurrentBlock
        {
            get { return blocks[blockCounter]; }
        }

        /// <summary>
        /// Gets if the underlying data file is closed
        /// </summary>
        [IgnoreDataMember]
        public virtual bool IsClosed
        {
            get { return baseStream == null; }
        }

        /// <summary>
        /// Gets if the underlying data file is an archive
        /// </summary>
        [IgnoreDataMember]
        public bool IsArchive
        {
            get { return BaseStream is IArchiveOutputStream || BaseStream is IArchiveInputStream; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Constructs a file without opening it
        /// </summary>
        protected DataFileBase()
        {
            InitializeMembers(new StreamingContext());
        }

        protected DataFileBase(DataFileBase old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Constructs a file and opens a stream automatically.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        protected DataFileBase(Uri uri, DataFileMode fileMode)
        {
            InitializeMembers(new StreamingContext());

            this.uri = uri;
            this.fileMode = fileMode;
        }

        /// <summary>
        /// Constructs a file object around an already open stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode"></param>
        protected DataFileBase(Stream stream, DataFileMode fileMode)
        {
            InitializeMembers(new StreamingContext());

            OpenExternalStream(stream, fileMode);
        }

        /// <summary>
        /// Initializes private members to their default values
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.description = null;

            this.baseStream = null;
            this.ownsBaseStream = false;
            this.streamFactory = null;

            this.fileMode = DataFileMode.Unknown;
            this.compression = DataFileCompression.None;
            this.uri = null;
            this.credentials = null;
            this.generateIdentityColumn = true;
            this.name = null;
            this.metadata = null;

            this.blocks = new List<DataFileBlockBase>();
            this.blockCounter = -1;
        }

        private void CopyMembers(DataFileBase old)
        {
            this.description = Util.DeepCloner.CloneObject(old.description);

            this.baseStream = null;
            this.ownsBaseStream = false;
            this.streamFactory = old.streamFactory;

            this.fileMode = old.fileMode;
            this.compression = old.compression;
            this.uri = old.uri;
            this.credentials = old.credentials;
            this.generateIdentityColumn = old.generateIdentityColumn;
            this.name = old.name;
            this.metadata = Util.DeepCloner.CloneObject(old.metadata);

            this.blocks = new List<DataFileBlockBase>(Util.DeepCloner.CloneCollection(old.blocks));

            this.blockCounter = -1;
        }

        public virtual void Dispose()
        {
            Close();
        }

        public abstract object Clone();

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
                throw new InvalidOperationException();  // TODO
            }
        }

        protected void Open()
        {
            Util.TaskHelper.Wait(OpenAsync());
        }

        /// <summary>
        /// Opens the file by opening a stream to the resource
        /// identified by the Uri property.
        /// </summary>
        public async Task OpenAsync()
        {
            EnsureNotOpen();

            switch (fileMode)
            {
                case DataFileMode.Read:
                    await OpenForReadAsync();
                    break;
                case DataFileMode.Write:
                    await OpenForWriteAsync();
                    break;
                default:
                    throw new InvalidOperationException();  // TODO
            }
        }

        /// <summary>
        /// Opens a file by wrapping an external file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode"></param>
        public void Open(Stream stream, DataFileMode fileMode)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");  // TODO
            }

            OpenExternalStream(stream, fileMode);
            Open();
        }

        /// <summary>
        /// Opens a file by opening a new stream.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileMode"></param>
        public async Task OpenAsync(Uri uri, DataFileMode fileMode)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri"); // TODO
            }

            this.uri = uri;
            this.fileMode = fileMode;

            await OpenAsync();
        }

        /// <summary>
        /// Opens a file by wrapping a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="compression"></param>
        protected void OpenExternalStream(Stream stream, DataFileMode fileMode)
        {
            this.baseStream = stream;
            this.ownsBaseStream = false;

            this.fileMode = fileMode;
        }

        /// <summary>
        /// Opens the underlying stream, if it is not set externally via
        /// a constructor or the OpenStream method.
        /// </summary>
        private async Task OpenOwnStreamAsync()
        {
            // Use stream factory to open stream
            var sf = streamFactory ?? Jhu.Graywulf.IO.StreamFactory.Create(null);
            baseStream = await sf.OpenAsync(uri, credentials, fileMode, compression);

            ownsBaseStream = true;
        }

        /// <summary>
        /// When overloaded in derived classes, opens the data file for reading
        /// </summary>
        protected virtual async Task OpenForReadAsync()
        {
            if (FileMode != DataFileMode.Read)
            {
                throw new InvalidOperationException();
            }

            if (baseStream == null)
            {
                await OpenOwnStreamAsync();
            }
        }

        /// <summary>
        /// When overloaded in derived class, opens the data file for writing
        /// </summary>
        protected virtual async Task OpenForWriteAsync()
        {
            if (FileMode != DataFileMode.Write)
            {
                throw new InvalidOperationException();
            }

            if (baseStream == null)
            {
                await OpenOwnStreamAsync();
            }

            // When writing into an archive a new entry for the file is to be created
            if (IsArchive)
            {
                // Determine file name form the archive's name
                var filename = Util.UriConverter.ToFilePath(uri);
                CreateArchiveEntry(filename, 0);
            }
        }

        /// <summary>
        /// When overloaded in derived classes, closes the data file
        /// </summary>
        public virtual void Close()
        {
            if (FileMode == DataFileMode.Write && IsArchive)
            {
                CloseArchiveEntry();
            }

            if (ownsBaseStream && baseStream != null)
            {
                if (baseStream.CanWrite)
                {
                    baseStream.Flush();
                }

                baseStream.Close();
                baseStream.Dispose();
                baseStream = null;
                ownsBaseStream = false;
            }
        }

        #endregion
        #region Archive handler functions

        internal IArchiveEntry ReadArchiveEntry()
        {
            var arch = (IArchiveInputStream)BaseStream;
            return arch.ReadNextFileEntry();
        }

        internal IArchiveEntry CreateArchiveEntry(string filename, long length)
        {
            // There's two cases here. If it is a file that's written into an archive,
            // but the archive was opened independently then the file name is probably
            // correct. If the current file opened the archive, then the file name
            // probably contains the extension of the archive so we should strip it.
            if (Compression == DataFileCompression.Automatic || Compression != DataFileCompression.None)
            {
                filename = Path.GetFileNameWithoutExtension(filename);
            }

            var arch = (IArchiveOutputStream)BaseStream;
            var entry = arch.CreateFileEntry(filename, length);
            arch.WriteNextEntry(entry);
            return entry;
        }

        internal void CloseArchiveEntry()
        {
            var arch = (IArchiveOutputStream)BaseStream;
            arch.CloseEntry();
        }

        #endregion
        #region Block-based read and write functions

        /// <summary>
        /// When overloaded in a derived class, reads the file header.
        /// </summary>
        protected internal abstract Task OnReadHeaderAsync();

        // TODO: add comments and rename to more meaningful
        protected internal virtual void OnSetMetadata()
        {
            // TODO: where to get name from if uri is not set?
            // TODO: remove compressed file's extension?
            this.name = uri == null ? "" : Util.UriConverter.GetFileNameWithoutExtension(uri).Replace(".", "_");
            this.metadata = null;    // TODO: set metadata
        }

        /// <summary>
        /// Appends a new block to a file.
        /// </summary>
        /// <param name="block"></param>
        /// <remarks>
        /// File block are either created automatically during read, or appended
        /// manually before reading or when writing.
        /// </remarks>
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

        /// <summary>
        /// Called by the library after a new block has been appended to the file.
        /// </summary>
        /// <param name="block"></param>
        protected abstract void OnBlockAppended(DataFileBlockBase block);

        /// <summary>
        /// Advanced the file to the next block.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Blocks can be predefined, in case data columns are set externally.
        /// </remarks>
        public async Task<DataFileBlockBase> ReadNextBlockAsync()
        {
            if (blockCounter == -1)
            {
                // If this would be the very first block, read the file header first.
                await OnReadHeaderAsync();
                OnSetMetadata();
            }
            else
            {
                // If we are not at the beginning of the file, read to the end of the
                // block, read the block footer and position stream on the beginning
                // of the next file block
                await blocks[blockCounter].OnReadToFinishAsync();
                await blocks[blockCounter].OnReadFooterAsync();
            }

            try
            {
                blockCounter++;

                DataFileBlockBase nextBlock;

                // If blocks are created manually, the blocks collection might already
                // contain an object for the next file block. In this case, use the
                // manually created object, otherwise create one automatically.
                if (blockCounter < blocks.Count)
                {
                    nextBlock = await OnReadNextBlockAsync(blocks[blockCounter]);
                }
                else
                {
                    // Create a new block automatically, if collection is not predefined
                    nextBlock = await OnReadNextBlockAsync(null);
                    if (nextBlock != null)
                    {
                        blocks.Add(nextBlock);
                    }
                }

                // See if there's a new block in the file.
                if (nextBlock != null)
                {
                    await nextBlock.OnReadHeaderAsync();

                    // TODO: theres' something to do here, see OnSetMetadata abstract
                    nextBlock.OnSetMetadata(blockCounter);
                    return nextBlock;
                }
                else
                {
                    // If no more blocks, read file footer
                    await OnReadFooterAsync();
                }
            }
            catch (EndOfStreamException)
            {
                // Some data formats cannot detect end of blocks and will
                // throw exception at the end of the file instead
                // Eat this exception now. Note, that this behaviour won't
                // occur when block contents are read, so the integrity of
                // reading a block will be kept anyway.
            }

            // No additional blocks found, return with null
            blockCounter = -1;
            return null;
        }

        /// <summary>
        /// When overloaded in a derived class, reads the next block.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<DataFileBlockBase> OnReadNextBlockAsync(DataFileBlockBase block);

        /// <summary>
        /// When overloaded in a derived class, reads the file footer.
        /// </summary>
        protected internal abstract Task OnReadFooterAsync();

        /// <summary>
        /// When overloaded in a derived class, writers the file header.
        /// </summary>
        protected abstract Task OnWriteHeaderAsync();

        /// <summary>
        /// Writes the next block into the file. Data is taken from the current
        /// results set of a data reader.
        /// </summary>
        /// <param name="dr"></param>
        private async Task WriteNextBlockAsync(ISmartDataReader dr)
        {
            blockCounter++;

            DataFileBlockBase nextBlock;

            // See if the file contains manually predefined blocks. If so, use
            // them, otherwise create a new one automatically.
            if (blockCounter < blocks.Count)
            {
                nextBlock = await OnCreateNextBlockAsync(blocks[blockCounter]);
            }
            else
            {
                // Create a new block automatically, if collection is not predefined
                nextBlock = await OnCreateNextBlockAsync(null);
                if (nextBlock != null)
                {
                    blocks.Add(nextBlock);
                }
            }

            // If a new block is found, detect columns from the data reader and
            // write contents into the file.
            if (nextBlock != null)
            {
                nextBlock.SetProperties(dr);
                await nextBlock.WriteFromDataReaderAsync(dr);
            }
        }

        /// <summary>
        /// When overriden in a derived class, writes the file footer.
        /// </summary>
        protected abstract Task OnWriteFooterAsync();

        /// <summary>
        /// When overriden in a derived class, writes the next file block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected abstract Task<DataFileBlockBase> OnCreateNextBlockAsync(DataFileBlockBase block);

        #endregion
        #region DataReader and Writer functions

        public async Task WriteFromDataCommandAsync(ISmartCommand cmd)
        {
            using (var dr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, CancellationToken.None))
            {
                await WriteFromDataReaderAsync((ISmartDataReader)dr, true);
            }
        }

        public void WriteFromDataCommand(ISmartCommand cmd)
        {
            Util.TaskHelper.Wait(WriteFromDataCommandAsync(cmd));
        }

        public Task WriteFromDataReaderAsync(ISmartDataReader dr)
        {
            return WriteFromDataReaderAsync(dr, false);
        }

        public void WriteFromDataReader(ISmartDataReader dr)
        {
            Util.TaskHelper.Wait(WriteFromDataReaderAsync(dr));
        }

        /// <summary>
        /// Writes the resultsets from a data reader into a file.
        /// </summary>
        /// <param name="dr"></param>
        private async Task WriteFromDataReaderAsync(ISmartDataReader dr, bool multiple)
        {
            if (multiple && !this.Description.CanHoldMultipleDatasets)
            {
                throw new InvalidOperationException("File can hold a single table only.");    // TODO
            }

            await OnWriteHeaderAsync();

            do
            {
                await WriteNextBlockAsync(dr);
            }
            while (multiple && dr.NextResult());

            await OnWriteFooterAsync();
        }

        #endregion
    }
}
