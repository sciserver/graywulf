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
        private TableReference table;
        private int binCount;
        private string keyColumn;
        private List<double> keyValue;
        private List<long> keyCount;
        private long rowCount;

        public TableReference Table
        {
            get { return table; }
            set { table = value; }
        }

        public int BinCount
        {
            get { return binCount; }
            set { binCount = value; }
        }

        public string KeyColumn
        {
            get { return keyColumn; }
            set { keyColumn = value; }
        }

        public List<double> KeyValue
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
            this.table = null;
            this.binCount = 250;
            this.keyColumn = null;
            this.keyValue = new List<double>();
            this.keyCount = new List<long>();
            this.rowCount = 0;
        }

        private void CopyMembers(TableStatistics old)
        {
            this.table = old.table;
            this.binCount = old.binCount;
            this.keyColumn = old.keyColumn;
            this.keyValue = new List<double>(old.keyValue);
            this.keyCount = new List<long>(old.keyCount);
            this.rowCount = old.rowCount;
        }
    }
}
