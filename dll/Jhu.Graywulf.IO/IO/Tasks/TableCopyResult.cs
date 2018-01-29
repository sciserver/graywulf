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

        private TableOrView sourceTable;
        private string sourceFileName;
        private Table targetTable;
        private string targetFileName;
        private long recordsAffected;
        private TableCopyStatus status;
        private string error;

        #endregion
        #region Properties

        [DataMember]
        [DefaultValue(null)]
        public TableOrView SourceTable
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
        public Table TargetTable
        {
            get { return targetTable; }
            set { targetTable = value; }
        }

        [DataMember]
        [DefaultValue(null)]
        public string TargetFileName
        {
            get { return targetFileName; }
            set { targetFileName = value; }
        }

        [DataMember]
        [DefaultValue(-1)]
        public long RecordsAffected
        {
            get { return recordsAffected; }
            set { recordsAffected = value; }
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
            this.targetTable = null;
            this.targetFileName = null;
            this.recordsAffected = -1;
            this.status = TableCopyStatus.Unknown;
            this.error = null;
        }

        private void CopyMembers(TableCopyResult old)
        {
            this.sourceTable = old.sourceTable;
            this.sourceFileName = old.sourceFileName;
            this.targetTable = old.targetTable;
            this.targetFileName = old.targetFileName;
            this.recordsAffected = -1;
            this.status = TableCopyStatus.Unknown;
            this.error = null;
        }
    }
}
