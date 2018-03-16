using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IfStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Flow; }
        }

        public BooleanExpression Condition
        {
            get { return FindDescendant<BooleanExpression>(); }
        }

        public Statement MainStatement
        {
            get { return FindDescendant<Statement>(0); }
        }

        public Statement ElseStatement
        {
            get { return FindDescendant<Statement>(1); }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield return MainStatement;

            var @else = ElseStatement;
            if (@else != null)
            {
                yield return @else;
            }
        }
    }
}
