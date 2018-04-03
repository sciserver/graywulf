using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class RemoteTable
    {
        public string UniqueKey { get; set; }
        public TableOrView Table { get; set; }
        public List<TableReference> TableReferences { get; set; }
        public string TempTableUniqueKey { get; set; }
        public Table TempTable { get; set; }
    }
}
