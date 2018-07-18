using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class LogicalNotOperator
    {

        public static LogicalNotOperator Create()
        {
            var not = new LogicalNotOperator();

            not.Stack.AddLast(Keyword.Create("NOT"));
            
            return not;
        }

    }
}
