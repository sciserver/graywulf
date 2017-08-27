using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public interface ITableSource : ITableReference, ICloneable
    {
        bool IsSubquery { get; }
        bool IsMultiTable { get; }

        IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive);

        IEnumerable<ITableSource> EnumerateMultiTableSources();
    }
}
