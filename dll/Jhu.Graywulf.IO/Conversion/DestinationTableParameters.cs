using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DestinationTableParameters
    {
        private Table table;
        private DestinationTableOperation operation;
        private int bulkInsertBatchSize;
        private int bulkInsertTimeout;

        [DataMember]
        public Table Table
        {
            get { return table; }
            set { table = value; }
        }

        [DataMember]
        public DestinationTableOperation Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        [DataMember]
        public int BulkInsertBatchSize
        {
            get { return bulkInsertBatchSize; }
            set { bulkInsertBatchSize = value; }
        }

        [DataMember]
        public int BulkInsertTimeout
        {
            get { return bulkInsertTimeout; }
            set { bulkInsertTimeout = value; }
        }

        public DestinationTableParameters()
        {
            InitializeMembers();
        }

        public DestinationTableParameters(DestinationTableParameters old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.operation = DestinationTableOperation.Create;
            this.bulkInsertBatchSize = 10000;
            this.bulkInsertTimeout = 1200;
        }

        private void CopyMembers(DestinationTableParameters old)
        {
            this.table = old.table;
            this.operation = old.operation;
            this.bulkInsertBatchSize = old.bulkInsertBatchSize;
            this.bulkInsertTimeout = old.bulkInsertTimeout;
        }
    }
}
