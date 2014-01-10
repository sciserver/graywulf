using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Reads and writes recordsets in SQL Server native format
    /// </summary>
    [Serializable]
    [DataContract(Namespace="")]
    public class SqlServerNativeDataFile : DataFileBase, ICloneable
    {
        #region Member variables

        [NonSerialized]
        private SqlServerNativeBinaryWriter nativeWriter;

        #endregion
        #region Properties

        public override FileFormatDescription Description
        {
            get
            {
                return new FileFormatDescription()
                {
                    DisplayName = FileFormatNames.Jhu_Graywulf_Format_SqlServerNativeDataFile,
                    DefaultExtension = Constants.FileExtensionBcp,
                    CanRead = true,
                    CanWrite = true,
                    CanDetectColumnNames = true,
                    CanHoldMultipleDatasets = false,
                    RequiresArchive = true,
                    IsCompressed = false,
                };
            }
        }

        [IgnoreDataMember]
        internal SqlServerNativeBinaryWriter NativeWriter
        {
            get { return nativeWriter; }
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
            this.nativeWriter = null;
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

            if (!IsArchive)
            {
                throw new Exception("Must be an archive");  // **** TODO
            }
        }

        protected override void OpenForWrite()
        {
            base.OpenForWrite();

            if (!IsArchive)
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
