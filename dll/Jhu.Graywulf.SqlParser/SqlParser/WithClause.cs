using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class WithClause
    {

        public CommonTableExpression CommonTableExpression
        {
            get { return FindDescendant<CommonTableExpression>(); }
        }

        public static WithClause Create(CommonTableExpression cte)
        {
            var wi = new WithClause();

            wi.Stack.AddLast(Keyword.Create("WITH"));
            wi.Stack.AddLast(Whitespace.Create());
            wi.Stack.AddLast(cte);

            return wi;
        }


    }
}
