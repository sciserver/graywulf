using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;


namespace Jhu.Graywulf.SqlParser
{
    public partial class SearchConditionBrackets
    {
        public static SearchConditionBrackets Create(SearchCondition sc)
        {
            var scb = new SearchConditionBrackets();

            scb.Stack.AddLast(new BracketOpen());
            scb.Stack.AddLast(sc);
            scb.Stack.AddLast(new BracketClose());

            return scb;
        }

        public LogicalExpressions.Expression GetExpressionTree()
        {
            return new LogicalExpressions.Brackets(FindDescendant<SearchCondition>().GetExpressionTree());
        }
    }
}
