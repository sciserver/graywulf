using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IfStatement
    {
        public LogicalExpression Condition
        {
            get { return FindDescendant<LogicalExpression>(); }
        }

        public Statement MainStatement
        {
            get { return FindDescendant<AnyStatement>(0)?.SpecificStatement; }
        }

        public Statement ElseStatement
        {
            get { return FindDescendant<AnyStatement>(1)?.SpecificStatement; }
        }

        public override IEnumerable<Statement> EnumerateSubStatements()
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
