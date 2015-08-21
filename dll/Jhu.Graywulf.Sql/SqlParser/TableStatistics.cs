using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser
{
    public class TableStatistics
    {
        #region Private member variables

        private int binCount;
        private Expression keyColumn;
        private DataType keyColumnDataType;
        private List<IComparable> keyValue;
        private List<long> keyCount;
        private long rowCount;

        #endregion
        #region Properties

        public int BinCount
        {
            get { return binCount; }
            set { binCount = value; }
        }

        public Expression KeyColumn
        {
            get { return keyColumn; }
            set { keyColumn = value; }
        }

        public DataType KeyColumnDataType
        {
            get { return keyColumnDataType; }
            set { keyColumnDataType = value; }
        }

        public List<IComparable> KeyValue
        {
            get { return keyValue; }
        }

        public List<long> KeyCount
        {
            get { return keyCount; }
        }

        public long RowCount
        {
            get { return rowCount; }
            set { rowCount = value; }
        }

        #endregion
        #region Constructors and initializers

        public TableStatistics()
        {
            InitializeMembers();
        }

        public TableStatistics(TableStatistics old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.binCount = 250;
            this.keyColumn = null;
            this.keyColumnDataType = null;
            this.keyValue = new List<IComparable>();
            this.keyCount = new List<long>();
            this.rowCount = 0;
        }

        private void CopyMembers(TableStatistics old)
        {
            this.binCount = old.binCount;
            this.keyColumn = old.keyColumn;
            this.keyColumnDataType = old.keyColumnDataType;
            this.keyValue = new List<IComparable>(old.keyValue);
            this.keyCount = new List<long>(old.keyCount);
            this.rowCount = old.rowCount;
        }

        #endregion
    }
}
