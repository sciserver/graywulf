using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class ColumnName
    {
        public static ColumnName Create(string columnName)
        {
            var ntn = new ColumnName();
            ntn.Stack.AddLast(Identifier.Create(columnName));

            return ntn;
        }
    }
}
