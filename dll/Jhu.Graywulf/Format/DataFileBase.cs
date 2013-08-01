using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Provides basic support for file-based DataReader implementations.
    /// </summary>
    [Serializable]
    public abstract class DataFileBase : IDisposable
    {
        [NonSerialized]
        private Stream baseStream;
        [NonSerialized]
        private bool ownsBaseStream;
        [NonSerialized]
        private Stream compressedStream;

        private DataFileMode fileMode;
        private CompressionMethod compression;
        private string path;

        private List<DataFileColumn> columns;

        [NonSerialized]
        private object[] rowValues;

        public abstract FileFormatDescription Description { get; }

        protected Stream Stream
        {
            get { return compressedStream ?? baseStream; }
        }

        public DataFileMode FileMode
        {
            get { return fileMode; }
            set
            {
                EnsureNotOpen();
                fileMode = value;
            }
        }

        public CompressionMethod Compression
        {
            get { return compression; }
            set
            {
                EnsureNotOpen();
                compression = value;
            }
        }

        public string Path
        {
            get { return path; }
            set
            {
                EnsureNotOpen();
                path = value;
            }
        }

        /// <summary>
        /// Gets the collection containing columns of the data file
        /// </summary>
        public List<DataFileColumn> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets the field values of the current row
        /// </summary>
        internal object[] RowValues
        {
            get { return rowValues; }
        }

        #region Constructors and initializers

        protected DataFileBase()
        {
            InitializeMembers();
        }

        protected DataFileBase(string path, DataFileMode fileMode)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            InitializeMembers();

            this.fileMode = fileMode;
            this.Path = path;
        }

        private void InitializeMembers()
        {
            this.baseStream = null;
            this.ownsBaseStream = false;
            this.compressedStream = null;

            this.fileMode = DataFileMode.Unknown;
            this.compression = CompressionMethod.None;
            this.path = null;

            this.columns = new List<DataFileColumn>();

            this.rowValues = null;
        }

        public virtual void Dispose()
        {
        }

        #endregion
        #region Stream open/close

        protected virtual void EnsureNotOpen()
        {
            if (ownsBaseStream && baseStream != null)
            {
                throw new InvalidOperationException();
            }
        }

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

        public virtual void Open(Stream stream, DataFileMode mode, CompressionMethod compression)
        {
            EnsureNotOpen();

            this.baseStream = stream;
            this.ownsBaseStream = false;

            this.FileMode = mode;
            this.compression = compression;
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

            if (baseStream == null)
            {
                // No open stream yet
                baseStream = new FileStream(Path, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read);
                ownsBaseStream = true;
            }

            // Open compressed stream, if necessary
            switch (compression)
            {
                case CompressionMethod.None:
                    break;
                case CompressionMethod.GZip:
                    compressedStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(baseStream);
                    break;
                case CompressionMethod.BZip2:
                    compressedStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(baseStream);
                    break;
                case CompressionMethod.Zip:
                    compressedStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(baseStream);
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
                baseStream = new FileStream(Path, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
                ownsBaseStream = true;
            }

            // Open compressed stream, if necessary
            switch (compression)
            {
                case CompressionMethod.None:
                    break;
                case CompressionMethod.GZip:
                    compressedStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(baseStream);
                    break;
                case CompressionMethod.BZip2:
                    compressedStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(baseStream);
                    break;
                case CompressionMethod.Zip:
                    compressedStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(baseStream);
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
            if (compressedStream != null)
            {
                compressedStream.Close();
                compressedStream.Dispose();
                compressedStream = null;
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
        public abstract bool IsClosed { get; }

        #endregion
        #region DataReader functions

        /// <summary>
        /// Returns a FileDataReader that can iterate through the rows of
        /// the data file.
        /// </summary>
        /// <returns></returns>
        public FileDataReader OpenDataReader()
        {
            if (rowValues == null)
            {
                if (!NextResult())
                {
                    throw new Exception();  // TODO
                }
            }

            return new FileDataReader(this);
        }

        /// <summary>
        /// Reads the next row from the data file
        /// </summary>
        /// <returns></returns>
        internal bool Read()
        {
            if (!OnRead(rowValues))
            {
                rowValues = null;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Advances to the next resultset in the data file
        /// </summary>
        /// <returns></returns>
        internal bool NextResult()
        {
            // Advance to the next resultset
            if (!OnNextResult())
            {
                return false;
            }

            OnColumnsCreated();

            return true;
        }

        /// <summary>
        /// When overriden in derived classes, advances the data file to the
        /// next resultset
        /// </summary>
        /// <returns></returns>
        protected abstract bool OnNextResult();

        /// <summary>
        /// When overriden in derived classes, reads the next data row from
        /// the data file
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected abstract bool OnRead(object[] values);

        /// <summary>
        /// Overriden classes call this function to mark the end of the resultset
        /// </summary>
        protected void MarkEnd()
        {
            rowValues = null;
        }

        #endregion
        #region DataWriter functions

        public void DetectColumns(IDataReader dr)
        {
            columns.Clear();

            var dt = dr.GetSchemaTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var r = dt.Rows[i];

                var col = new DataFileColumn((string)r["ColumnName"], (Type)r["DataType"]);
                col.AllowNull = (bool)r["AllowDBNull"];
                col.Format = "{0}";
                col.IsIdentity = (bool)r["IsIdentity"];
                col.Size = (int)r["ColumnSize"];

                if (col.Type == typeof(string) && col.Size == 1)
                {
                    col.Type = typeof(char);
                }

                columns.Add(col);
            }

            OnColumnsCreated();
        }

        public void WriteFromDataReader(IDataReader dr)
        {
            if (columns.Count == 0)
            {
                DetectColumns(dr);
            }

            OpenForWrite();

            OnWriteHeader();

            while (dr.Read())
            {
                dr.GetValues(rowValues);
                OnWrite(rowValues);
            }

            OnWriteFooter();

            Close();
        }

        protected abstract void OnWriteHeader();

        protected abstract void OnWrite(object[] values);

        protected abstract void OnWriteFooter();

        #endregion
        #region Column functions

        protected virtual void OnColumnsCreated()
        {
            // Instantiate array to store actual row values
            rowValues = new object[columns.Count];
        }

        #endregion
    }
}
