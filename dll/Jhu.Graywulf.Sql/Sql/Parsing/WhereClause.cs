using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class WhereClause : ISourceTableConsumer
    {
        public IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            foreach (var sq in EnumerateDescendantsRecursive<Subquery>(typeof(Subquery)))
            {
                foreach (var ts in sq.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }

            // TODO: add functionality to handle semi-join constructs
            // verify this, because might be covered by the where clause above
        }
        
        public static WhereClause Create(BooleanExpression sc)
        {
            var wh = new WhereClause();

            wh.Stack.AddLast(Keyword.Create("WHERE"));
            wh.Stack.AddLast(Whitespace.Create());
            wh.Stack.AddLast(sc);

            return wh;
        }

        public void AppendCondition(BooleanExpression sc, string opstring)
        {
            var cond = this.FindDescendant<BooleanExpression>();
            this.Stack.Remove(cond);

            var br1 = BooleanExpressionBrackets.Create(sc);
            var br2 = BooleanExpressionBrackets.Create(cond);

            var op = LogicalOperator.Create(opstring);
            cond = BooleanExpression.Create(br1, BooleanExpression.Create(false, br2), op);

            this.Stack.AddLast(cond);
        }
    }
}
