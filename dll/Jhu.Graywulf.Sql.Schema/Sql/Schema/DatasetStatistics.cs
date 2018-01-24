using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class DatasetStatistics
    {
        [NonSerialized]
        private long dataSpace;

        [NonSerialized]
        private long logSpace;

        [NonSerialized]
        private long usedSpace;

        [DataMember]
        public long DataSpace
        {
            get { return dataSpace; }
            internal set { dataSpace = value; }
        }

        [DataMember]
        public long UsedSpace
        {
            get { return usedSpace; }
            internal set { usedSpace = value; }
        }

        [DataMember]
        public long LogSpace
        {
            get { return logSpace; }
            internal set { logSpace = value; }
        }
    }
}
