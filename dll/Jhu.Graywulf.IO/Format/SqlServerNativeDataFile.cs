using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using ICSharpCode.SharpZipLib.Zip;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Reads and writes recordsets in SQL Server native format
    /// </summary>
    [Serializable]
    public class SqlServerNativeDataFile : DataFileBase, ICloneable
    {
        #region Member variables

        [NonSerialized]
        private BinaryReader inputReader;
        [NonSerialized]
        private bool ownsInputReader;

        [NonSerialized]
        private BinaryWriter outputWriter;
        [NonSerialized]
        private bool ownsOutputWriter;

        #endregion
        #region Properties

        public override FileFormatDescription Description
        {
            get
            {
                return new FileFormatDescription()
                {
                    DisplayName = FileFormatNames.Jhu_Graywulf_Format_SqlServerNativeDataFile,
                    DefaultExtension = Jhu.Graywulf.IO.Constants.FileExtensionZip,
                    CanRead = true,
                    CanWrite = true,
                    CanDetectColumnNames = true,
                    CanHoldMultipleDatasets = false,
                    RequiresArchive = true,
                    IsCompressed = false,
                };
            }
        }

        internal BinaryReader InputReader
        {
            get { return inputReader; }
        }

        internal BinaryWriter OutputWriter
        {
            get { return outputWriter; }
        }

        #endregion
        #region Constructors and initializers

        public SqlServerNativeDataFile()
            : base()
        {
            InitializeMembers();
        }

        public SqlServerNativeDataFile(SqlServerNativeDataFile old)
            : base(old)
        {
            CopyMembers(old);
        }

        public SqlServerNativeDataFile(Uri uri, DataFileMode fileMode)
            : base(uri, fileMode)
        {
            InitializeMembers();

            Open();
        }

        public SqlServerNativeDataFile(Stream stream, DataFileMode fileMode)
            : base(stream, fileMode)
        {
            InitializeMembers();

            Open();
        }

        private void InitializeMembers()
        {
            this.inputReader = null;
            this.ownsInputReader = false;
            this.outputWriter = null;
            this.ownsOutputWriter = false;
        }

        private void CopyMembers(SqlServerNativeDataFile old)
        {
            this.inputReader = null;
            this.ownsInputReader = false;
            this.outputWriter = null;
            this.ownsOutputWriter = false;
        }

        public override object Clone()
        {
            return new SqlServerNativeDataFile(this);
        }

        #endregion
        #region Stream open and close

        protected override void OpenForRead()
        {
            base.OpenForRead();

            if (!IsArchive)
            {
                throw new Exception("Must be an archive");  // **** TODO
            }

            inputReader = new BinaryReader(new DetachedStream(base.BaseStream));
            ownsInputReader = true;
        }

        protected override void OpenForWrite()
        {
            base.OpenForWrite();

            if (!IsArchive)
            {
                throw new Exception("Must be an archive");  // **** TODO
            }

            // Wrap underlying stream, so it doesn't get disposed automatically
            outputWriter = new BinaryWriter(new DetachedStream(base.BaseStream));
            ownsOutputWriter = true;
        }

        public override void Close()
        {
            if (ownsInputReader && inputReader != null)
            {
                inputReader.Close();
                inputReader.Dispose();
            }

            inputReader = null;
            ownsInputReader = false;

            if (ownsOutputWriter && outputWriter != null)
            {
                outputWriter.Flush();
                outputWriter.Close();
                outputWriter.Dispose();
            }

            outputWriter = null;
            ownsOutputWriter = false;

            base.Close();
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
            var arch = (IArchiveOutputStream)BaseStream;
            var entry = arch.CreateFileEntry(filename, length);
            arch.WriteNextEntry(entry);

            return entry;
        }

        #endregion
        #region Read and write function

        protected internal override void OnReadHeader()
        {
            // Do nothing
        }

        protected override DataFileBlockBase OnReadNextBlock(DataFileBlockBase block)
        {
            return block ?? new SqlServerNativeDataFileBlock(this);
        }

        protected internal override void OnReadFooter()
        {
            // Do nothing
        }

        protected override void OnWriteHeader()
        {
            // Do nothing
        }

        protected override DataFileBlockBase OnWriteNextBlock(DataFileBlockBase block, System.Data.IDataReader dr)
        {
            return block ?? new SqlServerNativeDataFileBlock(this);
        }

        protected override void OnWriteFooter()
        {
            // Do nothing
        }

        protected override void OnBlockAppended(DataFileBlockBase block)
        {
            // Do nothing
        }

        #endregion
    }
}
