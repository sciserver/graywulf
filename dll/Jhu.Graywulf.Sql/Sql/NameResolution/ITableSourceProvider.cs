using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public interface ITableSourceProvider
    {
        IEnumerable<ITableSource> EnumerateSourceTables(bool recursive);
        IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive);
    }
}
