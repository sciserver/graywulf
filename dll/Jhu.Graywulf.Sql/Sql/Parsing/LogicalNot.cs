using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class LogicalNot
    {

        public static LogicalNot Create()
        {
            var not = new LogicalNot();

            not.Stack.AddLast(Keyword.Create("NOT"));
            
            return not;
        }

    }
}
