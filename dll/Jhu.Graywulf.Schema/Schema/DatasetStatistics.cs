using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    public class DatasetStatistics
    {
        private long dataSpace;
        private long logSpace;
        private long usedSpace;

        public long DataSpace
        {
            get { return dataSpace; }
            internal set { dataSpace = value; }
        }

        public long UsedSpace
        {
            get { return usedSpace; }
            internal set { usedSpace = value; }
        }

        public long LogSpace
        {
            get { return logSpace; }
            internal set { logSpace = value; }
        }
    }
}
