using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class JoinedTable
    {
        public static JoinedTable Create(JoinType joinType, TableSourceSpecification tableSource, LogicalExpression joinCondition)
        {
            var jt = new JoinedTable();
            jt.Stack.AddLast(joinType);
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(tableSource);
            jt.Stack.AddLast(Whitespace.CreateNewLine());
            jt.Stack.AddLast(Keyword.Create("ON"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(joinCondition);

            return jt;
        }
    }
}
