using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using ICSharpCode.SharpZipLib.Zip;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Reads and writes recordsets in SQL Server native format
    /// </summary>
    [Serializable]
    public class SqlServerNativeDataFile : DataFileBase
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
                    DefaultExtension = Constants.FileExtensionZip,
                    CanRead = true,
                    CanWrite = true,
                    CanDetectColumnNames = true,
                    CanHoldMultipleDatasets = true,
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

        protected SqlServerNativeDataFile()
            : base()
        {
            InitializeMembers();
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
        #region Read and write

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
