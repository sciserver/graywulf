using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FromClause
    {
        public static FromClause Create(TableSourceExpression tse)
        {
            var from = new FromClause();

            from.Stack.AddLast(Keyword.Create("FROM"));
            from.Stack.AddLast(Whitespace.Create());
            from.Stack.AddLast(tse);

            return from;
        }

        #region Query construction functions

        public void AppendJoinedTable(JoinedTable joinedTable)
        {
            // Find the last table source
            var tse = FindDescendant<TableSourceExpression>();
            var ts = tse.EnumerateDescendantsRecursive<TableSource>(typeof(Subquery)).LastOrDefault();

            ts.Stack.AddLast(Whitespace.CreateNewLine());
            ts.Stack.AddLast(joinedTable);
        }

        public void PrependJoinedTable(TableSource tableSource, JoinType joinType, BooleanExpression joinCondition)
        {
            // Find the first table source
            var tse = FindDescendant<TableSourceExpression>();
            var ts = tse.FindDescendant<TableSource>();

            var jt = JoinedTable.Create(joinType, ts, joinCondition);

            tableSource.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.CreateNewLine()));
            tableSource.Stack.AddLast(jt);

            // Exchange table sources so that the joined table comes first
            ts.ExchangeWith(tableSource);
        }


        #endregion
    }
}
