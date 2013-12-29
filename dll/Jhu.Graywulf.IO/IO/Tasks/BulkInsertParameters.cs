using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Jhu.Graywulf.IO
{
    [Serializable]
    public class BulkInsertParameters
    {
        #region Member Variables

        private string filename;
        private bool checkConstraints;
        private string codePage;
        private string dataFileType;
        private string fieldTerminator;
        private int firstRow;
        private bool fireTriggers;
        private string formatFile;
        private bool keepIdentity;
        private bool keepNulls;
        private int kilobytesPerBatch;
        private int lastRow;
        private int maxErrors;
        private string order;
        private int rowsPerBatch;
        private string rowTerminator;
        private bool tabLock;
        private string errorFile;

        #endregion
        #region Properties

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public bool CheckConstraints
        {
            get { return checkConstraints; }
            set { checkConstraints = value; }
        }

        public string CodePage
        {
            get { return codePage; }
            set { codePage = value; }
        }

        public string DataFileType
        {
            get { return dataFileType; }
            set { dataFileType = value; }
        }

        public string FieldTerminator
        {
            get { return fieldTerminator; }
            set { fieldTerminator = value; }
        }

        public int FirstRow
        {
            get { return firstRow; }
            set { firstRow = value; }
        }

        public bool FireTriggers
        {
            get { return fireTriggers; }
            set { fireTriggers = value; }
        }

        public string FormatFile
        {
            get { return formatFile; }
            set { formatFile = value; }
        }

        public bool KeepIdentity
        {
            get { return keepIdentity; }
            set { keepIdentity = value; }
        }

        public bool KeepNulls
        {
            get { return keepNulls; }
            set { keepNulls = value; }
        }

        public int KilobytesPerBatch
        {
            get { return kilobytesPerBatch; }
            set { kilobytesPerBatch = value; }
        }

        public int LastRow
        {
            get { return lastRow; }
            set { lastRow = value; }
        }

        public int MaxErrors
        {
            get { return maxErrors; }
            set { maxErrors = value; }
        }

        public string Order
        {
            get { return order; }
            set { order = value; }
        }

        public int RowsPerBatch
        {
            get { return rowsPerBatch; }
            set { rowsPerBatch = value; }
        }

        public string RowTerminator
        {
            get { return rowTerminator; }
            set { rowTerminator = value; }
        }

        public bool TabLock
        {
            get { return tabLock; }
            set { tabLock = value; }
        }

        public string ErrorFile
        {
            get { return errorFile; }
            set { errorFile = value; }
        }

        #endregion

        public BulkInsertParameters()
        {
            InitializeMembers();
        }

        public BulkInsertParameters(BulkInsertParameters old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.filename = null;
            this.checkConstraints = false;
            this.codePage = "iso-1250";
            this.dataFileType = "";
            this.fieldTerminator = ",";
            this.firstRow = 0;
            this.fireTriggers = false;
            this.formatFile = String.Empty;
            this.keepIdentity = true;
            this.keepNulls = true;
            this.kilobytesPerBatch = 1024;
            this.lastRow = 0;
            this.maxErrors = 10;
            this.order = String.Empty;
            this.rowsPerBatch = 10000;
            this.rowTerminator = "\n";
            this.tabLock = true;
            this.errorFile = null;
        }

        private void CopyMembers(BulkInsertParameters old)
        {
            this.filename = old.filename;
            this.checkConstraints = old.checkConstraints;
            this.codePage = old.codePage;
            this.dataFileType = old.dataFileType;
            this.fieldTerminator = old.fieldTerminator;
            this.firstRow = old.firstRow;
            this.fireTriggers = old.fireTriggers;
            this.formatFile = old.formatFile;
            this.keepIdentity = old.keepIdentity;
            this.keepNulls = old.keepNulls;
            this.kilobytesPerBatch = old.kilobytesPerBatch;
            this.lastRow = old.lastRow;
            this.maxErrors = old.maxErrors;
            this.order = old.order;
            this.rowsPerBatch = old.rowsPerBatch;
            this.rowTerminator = old.rowTerminator;
            this.tabLock = old.tabLock;
            this.errorFile = old.errorFile;
        }
    }
}
