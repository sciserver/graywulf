using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class RemoteSourceTable
    {
        public string UniqueKey { get; set; }
        public TableOrView Table { get; set; }
        public string RemoteQuery { get; set; }
        public List<TableReference> TableReferences { get; set; }
    }
}
