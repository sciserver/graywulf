using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class BooleanExpressionBrackets
    {
        public static BooleanExpressionBrackets Create(BooleanExpression sc)
        {
            var scb = new BooleanExpressionBrackets();

            scb.Stack.AddLast(new BracketOpen());
            scb.Stack.AddLast(sc);
            scb.Stack.AddLast(new BracketClose());

            return scb;
        }

        public LogicalExpressions.Expression GetExpressionTree()
        {
            return new LogicalExpressions.Brackets(FindDescendant<BooleanExpression>().GetExpressionTree());
        }
    }
}
