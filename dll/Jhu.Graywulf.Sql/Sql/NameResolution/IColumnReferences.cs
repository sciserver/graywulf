using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public interface IColumnReferences
    {
        IndexedDictionary<string, ColumnReference> ColumnReferences { get; }
    }
}
