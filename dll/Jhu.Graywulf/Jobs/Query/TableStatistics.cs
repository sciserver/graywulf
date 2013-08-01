using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
	public class TableStatistics
	{
        private TableReference table;
        private string sampledColumn;
        private List<decimal> binMin;
        private List<decimal> binMax;
        private List<int> binCount;
        private int count;

        public TableReference Table
        {
            get { return table; }
            set { table = value; }
        }

        public string SampledColumn
        {
            get { return sampledColumn; }
            set { sampledColumn = value; }
        }

        public List<decimal> BinMin
        {
            get { return binMin; }
        }

        public List<decimal> BinMax
        {
            get { return binMax; }
        }

        public List<int> BinCount
        {
            get { return binCount; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
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
            this.sampledColumn = null;
            this.binMin = new List<decimal>();
            this.binMax = new List<decimal>();
            this.binCount = new List<int>();
            this.count = -1;
        }

        private void CopyMembers(TableStatistics old)
        {
            this.table = old.table;
            this.sampledColumn = old.sampledColumn;
            this.binMin = new List<decimal>(old.binMin);
            this.binMax = new List<decimal>(old.binMax);
            this.binCount = new List<int>(old.binCount);
            this.count = old.count;
        }
	}
}
