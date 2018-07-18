using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class WhereClause
    {        
        public static WhereClause Create(LogicalExpression sc)
        {
            var wh = new WhereClause();

            wh.Stack.AddLast(Keyword.Create("WHERE"));
            wh.Stack.AddLast(Whitespace.Create());
            wh.Stack.AddLast(sc);

            return wh;
        }

        public void AppendCondition(LogicalExpression sc, string opstring)
        {
            var cond = this.FindDescendant<LogicalExpression>();
            this.Stack.Remove(cond);

            var br1 = LogicalExpressionBrackets.Create(sc);
            var br2 = LogicalExpressionBrackets.Create(cond);

            var op = LogicalOperator.Create(opstring);
            cond = LogicalExpression.Create(br1, LogicalExpression.Create(false, br2), op);

            this.Stack.AddLast(cond);
        }
    }
}
