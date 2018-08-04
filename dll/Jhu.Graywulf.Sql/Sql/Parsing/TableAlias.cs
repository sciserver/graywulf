using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableAlias
    {
        public TableSource TableSource
        {
            get { return FindAscendant<TableSource>(); }
        }

        public static TableAlias Create(string alias)
        {
            var res = new TableAlias();

            res.Stack.AddLast(Identifier.Create(alias));

            return res;
        }
    }
}
