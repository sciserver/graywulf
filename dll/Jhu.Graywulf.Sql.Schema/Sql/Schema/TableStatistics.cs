using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class TableStatistics
    {
        [NonSerialized]
        private long rowCount;

        [NonSerialized]
        private long dataSpace;

        [NonSerialized]
        private long indexSpace;

        [DataMember]
        public long RowCount
        {
            get { return rowCount; }
            internal set { rowCount = value; }
        }

        [DataMember]
        public long DataSpace
        {
            get { return dataSpace; }
            internal set { dataSpace = value; }
        }

        [DataMember]
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
