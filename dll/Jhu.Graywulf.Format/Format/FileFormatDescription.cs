using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public class FileFormatDescription
    {
        /// <summary>
        /// .Net type implementing the file format
        /// </summary>
        private Type type;

        /// <summary>
        /// Name of file format
        /// </summary>
        private string displayName;

        /// <summary>
        /// Default extension of file format.
        /// </summary>
        private string defaultExtension;

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

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public string DefaultExtension
        {
            get { return defaultExtension; }
            set { defaultExtension = value; }
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

        public FileFormatDescription()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.type = null;
            this.displayName = null;
            this.defaultExtension = null;
            this.canRead = false;
            this.canWrite = false;
            this.canDetectColumnNames = false;
            this.canHoldMultipleDatasets = false;
            this.requiresArchive = false;
            this.isCompressed = false;
        }
    }
}
