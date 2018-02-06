using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    public class TableCopySettings : ICloneable
    {
        #region Private member variables
        
        [NonSerialized]
        private string batchName;

        [NonSerialized]
        private int batchSize;

        [NonSerialized]
        private int timeout;

        [NonSerialized]
        private bool bypassExceptions;

        [NonSerialized]
        private string fileFormatFactoryType;

        [NonSerialized]
        private string streamFactoryType;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the name of the batch. Used when
        /// importing and exporting archives.
        /// </summary>
        public string BatchName
        {
            get { return batchName; }
            set { batchName = value; }
        }

        /// <summary>
        /// Gets or sets the batch size of bulk insert operations.
        /// </summary>
        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of bulk insert operations.
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Gets or sets if the task ignores problems and proceeds with table
        /// copy even when an exception is thrown. Exception bypass logic is
        /// implemented differently in derived classes.
        /// </summary>
        public bool BypassExceptions
        {
            get { return bypassExceptions; }
            set { bypassExceptions = value; }
        }

        /// <summary>
        /// Gets or sets the file format factory to use when creating output files
        /// or opening input files.
        /// </summary>
        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        /// <summary>
        /// Gets or sets the stream factory to use when opening input and output
        /// streams to read and write files.
        /// </summary>
        public string StreamFactoryType
        {
            get { return streamFactoryType; }
            set { streamFactoryType = value; }
        }

        #endregion

        #region Constructors and initializers

        public TableCopySettings()
        {
            InitializeMembers();
        }
        
        public TableCopySettings(TableCopySettings old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.batchName = null;
            this.batchSize = Constants.DefaultBulkInsertBatchSize;
            this.timeout = Constants.DefaultBulkInsertTimeout;
            this.bypassExceptions = false;
            this.fileFormatFactoryType = null;
            this.streamFactoryType = null;
        }

        private void CopyMembers(TableCopySettings old)
        {
            this.batchName = old.batchName;
            this.batchSize = old.batchSize;
            this.timeout = old.timeout;
            this.bypassExceptions = old.bypassExceptions;
            this.fileFormatFactoryType = old.fileFormatFactoryType;
            this.streamFactoryType = old.streamFactoryType;
        }

        public virtual object Clone()
        {
            return new TableCopySettings(this);
        }

        #endregion
    }
}
