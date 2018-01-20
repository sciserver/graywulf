using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public enum TableReferenceType
    {
        Unknown,
        TableOrView,
        Subquery,
        CommonTable,
        Variable,
        UserDefinedFunction,
        SelectInto,
        CreateTable,
    }
}
