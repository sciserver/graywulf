using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StatementBlock
    {
        public IEnumerable<Statement> EnumerateSubStatements()
        {
            return EnumerateDescendants<Statement>(true);
        }

        public static StatementBlock Create(Statement statement)
        {
            var nsb = new StatementBlock(new AnyStatement(statement));
            return nsb;
        }

        public static StatementBlock Create(Statement[] statements)
        {
            StatementBlock nsb = null;
            StatementBlock sb = null;

            for (int i = 0; i < statements.Length; i++)
            {
                var ssb = Create(statements[i]);

                if (i == 0)
                {
                    nsb = ssb;
                }
                else
                {
                    sb.Stack.AddLast(StatementSeparator.Create());
                    sb.Stack.AddLast(ssb);
                }

                sb = ssb;
            }

            return nsb;
        }
    }
}
