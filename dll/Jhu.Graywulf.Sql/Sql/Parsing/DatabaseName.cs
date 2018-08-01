using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DatabaseName
    {
        public static DatabaseName Create(string databaseName)
        {
            var ndn = new DatabaseName();
            ndn.Stack.AddLast(Identifier.Create(databaseName));

            return ndn;
        }
    }
}
