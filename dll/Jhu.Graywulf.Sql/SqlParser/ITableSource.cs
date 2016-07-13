using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public interface ITableSource : ITableReference, ICloneable
    {
        bool IsSubquery { get; }
        bool IsMultiTable { get; }

        IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive);

        IEnumerable<ITableSource> EnumerateMultiTableSources();
    }
}
