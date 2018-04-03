using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StatementBlock : IStatement
    {
        #region Properties

        public bool IsResolvable
        {
            get { return false; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Block; }
        }

        #endregion

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            return EnumerateDescendants<Statement>(true);
        }

        public static StatementBlock Create(IStatement statement)
        {
            var nsb = new StatementBlock();
            nsb.Stack.AddLast((Node)statement);
            return nsb;
        }

        public static StatementBlock Create(params IStatement[] statements)
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
