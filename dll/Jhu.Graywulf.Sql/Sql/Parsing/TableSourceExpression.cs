using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableSourceExpression
    {
        public static TableSourceExpression Create(TableSourceSpecification tableSource)
        {
            return Create(tableSource, null);
        }

        public static TableSourceExpression Create(TableSourceSpecification tableSource, JoinedTable joinedTable)
        {
            var tse = new TableSourceExpression();

            tse.Stack.AddLast(tableSource);

            if (joinedTable != null)
            {
                tse.Stack.AddLast(Whitespace.CreateNewLine());
                tse.Stack.AddLast(joinedTable);
            }

            return tse;
        }
    }
}
