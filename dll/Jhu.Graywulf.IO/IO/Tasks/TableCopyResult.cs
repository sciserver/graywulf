using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    [DataContract]
    public class TableCopyResult
    {
        #region Private member variables

        private string sourceTable;
        private string sourceFileName;
        private string destinationTable;
        private string destinationFileName;
        private long recordsAffected;
        private TimeSpan elapsed;
        private TableCopyStatus status;
        private string error;

        #endregion
        #region Properties

        [DataMember]
        [DefaultValue(null)]
        public string SourceTable
        {
            get { return sourceTable; }
            set { sourceTable = value; }
        }

        [DataMember]
        [DefaultValue(null)]
        public string SourceFileName
        {
            get { return sourceFileName; }
            set { sourceFileName = value; }
        }

        [DataMember]
        [DefaultValue(null)]
        public string DestinationTable
        {
            get { return destinationTable; }
            set { destinationTable = value; }
        }

        [DataMember]
        [DefaultValue(null)]
        public string DestinationFileName
        {
            get { return destinationFileName; }
            set { destinationFileName = value; }
        }

        [DataMember]
        [DefaultValue(-1)]
        public long RecordsAffected
        {
            get { return recordsAffected; }
            set { recordsAffected = value; }
        }

        [DataMember]
        public TimeSpan Elapsed
        {
            get { return elapsed; }
            set { elapsed = value; }
        }

        [DataMember]
        public TableCopyStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        [DataMember]
        [DefaultValue(null)]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        #endregion

        public TableCopyResult()
        {
            InitializeMembers();
        }

        public TableCopyResult(TableCopyResult old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sourceTable = null;
            this.sourceFileName = null;
            this.destinationTable = null;
            this.destinationFileName = null;
            this.recordsAffected = -1;
            this.status = TableCopyStatus.Unknown;
            this.error = null;
        }

        private void CopyMembers(TableCopyResult old)
        {
            this.sourceTable = old.sourceTable;
            this.sourceFileName = old.sourceFileName;
            this.destinationTable = old.destinationTable;
            this.destinationFileName = old.destinationFileName;
            this.recordsAffected = -1;
            this.status = TableCopyStatus.Unknown;
            this.error = null;
        }
    }
}
