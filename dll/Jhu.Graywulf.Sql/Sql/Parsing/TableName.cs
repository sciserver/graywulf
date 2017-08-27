using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableName
    {
        public static TableName Create(string tableName)
        {
            var ntn = new TableName();
            ntn.Stack.AddLast(Identifier.Create(tableName));

            return ntn;
        }
    }
}
