using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TryCatchStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Flow; }
        }

        public StatementBlock TryBlock
        {
            get { return FindDescendant<StatementBlock>(0); }
        }

        public StatementBlock CatchBlock
        {
            get { return FindDescendant<StatementBlock>(1); }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
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
