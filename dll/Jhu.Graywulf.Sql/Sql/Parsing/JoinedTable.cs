using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class JoinedTable
    {
        public static JoinedTable Create(JoinOperator joinOperator, TableSourceSpecification tableSource, LogicalExpression joinCondition)
        {
            var jt = new JoinedTable();
            jt.Stack.AddLast(joinOperator);
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(tableSource);
            jt.Stack.AddLast(Whitespace.CreateNewLine());
            jt.Stack.AddLast(JoinCondition.Create(joinCondition));

            return jt;
        }
    }
}
