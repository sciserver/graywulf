using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlCodeGen
{
    public enum ColumnListType
    {
        CreateTable,
        CreateIndex,
        CreateView,
        Insert,
        Select
    }
}
