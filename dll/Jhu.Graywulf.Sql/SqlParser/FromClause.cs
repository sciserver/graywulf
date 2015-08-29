using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FromClause
    {
        #region Query construction functions

        public void AppendJoinedTable(JoinedTable joinedTable)
        {
            // Find the last table source
            var tse = FindDescendant<TableSourceExpression>();
            var ts = tse.EnumerateDescendantsRecursive<TableSource>(typeof(Subquery)).LastOrDefault();

            ts.Stack.AddLast(Whitespace.CreateNewLine());
            ts.Stack.AddLast(joinedTable);
        }

        public void PrependJoinedTable(TableSource tableSource, JoinType joinType, SearchCondition joinCondition)
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
