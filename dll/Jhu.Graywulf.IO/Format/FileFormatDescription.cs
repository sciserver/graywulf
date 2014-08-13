using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public class FileFormatDescription : ICloneable
    {
        #region Private member variables

        /// <summary>
        /// Name of file format
        /// </summary>
        private string displayName;

        /// <summary>
        /// MIME-type of the file format.
        /// </summary>
        private string mimeType;

        /// <summary>
        /// Default extension of file format.
        /// </summary>
        private string extension;

        /// <summary>
        /// File format class can read data.
        /// </summary>
        private bool canRead;

        /// <summary>
        /// File format class can write data.
        /// </summary>
        private bool canWrite;

        /// <summary>
        /// File format class can detect column names (CSV, for instance)
        /// </summary>
        private bool canDetectColumnNames;

        /// <summary>
        /// File format supports multiple tables in a single file.
        /// </summary>
        private bool canHoldMultipleDatasets;

        /// <summary>
        /// File format requires an archive stream to read and write
        /// </summary>
        private bool requiresArchive;

        /// <summary>
        /// File format is inherently compressed
        /// </summary>
        private bool isCompressed;

        /// <summary>
        /// If true, the number of records is known before enumerating the
        /// records during read, for example, the number of records is
        /// in the header.
        /// </summary>
        private bool knowsRecordCount;

        /// <summary>
        /// If true, the number of records has to be known prior to enumerating
        /// the records, for example, to write it into the file header.
        /// </summary>
        private bool requiresRecordCount;

        #endregion
        #region

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        public bool CanRead
        {
            get { return canRead; }
            set { canRead = value; }
        }

        public bool CanWrite
        {
            get { return canWrite; }
            set { canWrite = value; }
        }

        /// <summary>
        /// Gets or sets whether file format class can detect column names.
        /// </summary>
        /// <remarks>
        /// This must be set to false for all file formats that contain detailed
        /// schema description, but true for files that don't, e.g. CSV.
        /// </remarks>
        public bool CanDetectColumnNames
        {
            get { return canDetectColumnNames; }
            set { canDetectColumnNames = value; }
        }

        public bool CanHoldMultipleDatasets
        {
            get { return canHoldMultipleDatasets; }
            set { canHoldMultipleDatasets = value; }
        }

        public bool RequiresArchive
        {
            get { return requiresArchive; }
            set { requiresArchive = value; }
        }

        public bool IsCompressed
        {
            get { return isCompressed; }
            set { isCompressed = value; }
        }

        public bool KnowsRecordCount
        {
            get { return knowsRecordCount; }
            set { knowsRecordCount = value; }
        }

        public bool RequiresRecordCount
        {
            get { return requiresRecordCount; }
            set { requiresRecordCount = value; }
        }

        #endregion
        #region Constructors and initializers

        public FileFormatDescription()
        {
            InitializeMembers();
        }

        public FileFormatDescription(FileFormatDescription old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.displayName = null;
            this.mimeType = null;
            this.extension = null;
            this.canRead = false;
            this.canWrite = false;
            this.canDetectColumnNames = false;
            this.canHoldMultipleDatasets = false;
            this.requiresArchive = false;
            this.isCompressed = false;
            this.knowsRecordCount = false;
            this.requiresRecordCount = false;
        }

        private void CopyMembers(FileFormatDescription old)
        {
            this.displayName = old.displayName;
            this.mimeType = old.mimeType;
            this.extension = old.extension;
            this.canRead = old.canRead;
            this.canWrite = old.canWrite;
            this.canDetectColumnNames = old.canDetectColumnNames;
            this.canHoldMultipleDatasets = old.canHoldMultipleDatasets;
            this.requiresArchive = old.requiresArchive;
            this.isCompressed = old.isCompressed;
            this.knowsRecordCount = old.knowsRecordCount;
            this.requiresRecordCount = old.requiresRecordCount;
        }

        public object Clone()
        {
            return new FileFormatDescription(this);
        }

        #endregion
    }
}
