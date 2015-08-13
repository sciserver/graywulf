using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class WhereClause
    {
        public static WhereClause Create(SearchCondition sc)
        {
            var wh = new WhereClause();

            wh.Stack.AddLast(Keyword.Create("WHERE"));
            wh.Stack.AddLast(Whitespace.Create());
            wh.Stack.AddLast(sc);

            return wh;
        }

        public void AppendCondition(SearchCondition sc, string opstring)
        {
            var cond = this.FindDescendant<SearchCondition>();
            this.Stack.Remove(cond);

            var br1 = SearchConditionBrackets.Create(sc);
            var br2 = SearchConditionBrackets.Create(cond);

            var op = LogicalOperator.Create(opstring);
            cond = SearchCondition.Create(br1, SearchCondition.Create(false, br2), op);

            this.Stack.AddLast(cond);
        }
    }
}
