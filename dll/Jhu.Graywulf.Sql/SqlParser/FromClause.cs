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
            var tse = FindDescendant<TableSourceExpression>();

            // Find the first table and try to find the last joined table
            var ft = tse.FindDescendant<TableSource>();
            if (ft != null)
            {
                tse.Stack.AddLast(Whitespace.CreateNewLine());
                tse.Stack.AddLast(joinedTable);

                return;
            }

            var jt = tse.EnumerateDescendantsRecursive<JoinedTable>(typeof(Subquery)).LastOrDefault();

            if (jt != null)
            {
                tse.Stack.AddLast(Whitespace.CreateNewLine());
                tse.Stack.AddLast(joinedTable);
            }

            throw new InvalidOperationException();
        }

        #endregion
    }
}
