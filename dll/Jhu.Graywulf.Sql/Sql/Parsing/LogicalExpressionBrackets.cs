using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class LogicalExpressionBrackets
    {
        public static LogicalExpressionBrackets Create(LogicalExpression sc)
        {
            var scb = new LogicalExpressionBrackets();

            scb.Stack.AddLast(new BracketOpen());
            scb.Stack.AddLast(sc);
            scb.Stack.AddLast(new BracketClose());

            return scb;
        }
    }
}
