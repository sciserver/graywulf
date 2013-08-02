using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public class TableStatistics
    {
        private long rowCount;
        private long dataSpace;
        private long indexSpace;

        public long RowCount
        {
            get { return rowCount; }
            internal set { rowCount = value; }
        }

        public long DataSpace
        {
            get { return dataSpace; }
            internal set { dataSpace = value; }
        }

        public long IndexSpace
        {
            get { return indexSpace; }
            internal set { indexSpace = value; }
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
            this.rowCount = 0;
            this.dataSpace = 0;
            this.indexSpace = 0;
        }

        private void CopyMembers(TableStatistics old)
        {
            this.rowCount = old.rowCount;
            this.dataSpace = old.dataSpace;
            this.indexSpace = old.indexSpace;
        }
    }
}
