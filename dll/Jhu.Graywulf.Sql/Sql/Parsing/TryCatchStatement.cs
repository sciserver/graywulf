using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TryCatchStatement
    {
        public StatementBlock TryBlock
        {
            get { return FindDescendant<StatementBlock>(0); }
        }

        public StatementBlock CatchBlock
        {
            get { return FindDescendant<StatementBlock>(1); }
        }

        public override IEnumerable<Statement> EnumerateSubStatements()
        {
            var t = TryBlock;
            if (t != null)
            {
                foreach (var s in t.EnumerateSubStatements())
                {
                    yield return s;
                }
            }

            var c = CatchBlock;
            if (c != null)
            {
                foreach (var s in c.EnumerateSubStatements())
                {
                    yield return s;
                }
            }
        }
    }
}
