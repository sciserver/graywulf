using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public class TableCopyResult
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string FileName { get; set; }
        public long RecordsAffected { get; set; }
        public TableCopyStatus Status { get; set; }
        public string Error { get; set; }
    }
}
