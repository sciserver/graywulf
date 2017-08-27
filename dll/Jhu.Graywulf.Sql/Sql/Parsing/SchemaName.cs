using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SchemaName
    {
        public static SchemaName Create(string schemaName)
        {
            var ntn = new SchemaName();
            ntn.Stack.AddLast(Identifier.Create(schemaName));

            return ntn;
        }
    }
}
