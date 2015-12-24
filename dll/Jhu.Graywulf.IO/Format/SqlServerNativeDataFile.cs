using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Data;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Reads and writes recordsets in SQL Server native format
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class SqlServerNativeDataFile : DataFileBase, ICloneable
    {
        #region Member variables

        [NonSerialized]
        private SqlServerNativeBinaryReader nativeReader;

        [NonSerialized]
        private SqlServerNativeBinaryWriter nativeWriter;

        [NonSerialized]
        private bool generateSqlScripts;

        #endregion
        #region Properties

        [IgnoreDataMember]
        internal SqlServerNativeBinaryReader NativeReader
        {
            get { return nativeReader; }
        }

        [IgnoreDataMember]
        internal SqlServerNativeBinaryWriter NativeWriter
        {
            get { return nativeWriter; }
        }

        public bool GenerateSqlScripts
        {
            get { return generateSqlScripts; }
            set { generateSqlScripts = value; }
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
            Description = new FileFormatDescription()
            {
                DisplayName = FileFormatNames.SqlServerNativeDataFile,
                MimeType = Constants.MimeTypeBcp,
                Extension = Constants.FileExtensionBcp,
                CanRead = true,
                CanWrite = true,
                CanDetectColumnNames = true,
                CanHoldMultipleDatasets = false,
                RequiresArchive = true,
                IsCompressed = false,
                KnowsRecordCount = false,
                RequiresRecordCount = false,
            };

            this.nativeReader = null;
            this.nativeWriter = null;
            this.generateSqlScripts = false;
        }

        private void CopyMembers(SqlServerNativeDataFile old)
        {
            this.nativeWriter = null;
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

            nativeReader = new SqlServerNativeBinaryReader(new DetachedStream(BaseStream));
        }

        protected override void OpenForWrite()
        {
            base.OpenForWrite();

            if (generateSqlScripts && !IsArchive)
            {
                throw new Exception("Must be an archive");  // **** TODO
            }

            // Wrap underlying stream, so it doesn't get disposed automatically
            nativeWriter = new SqlServerNativeBinaryWriter(new DetachedStream(BaseStream));
        }

        public override void Close()
        {
            base.Close();
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

        protected override DataFileBlockBase OnCreateNextBlock(DataFileBlockBase block)
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
