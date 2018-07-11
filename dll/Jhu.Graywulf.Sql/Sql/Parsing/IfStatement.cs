using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IfStatement
    {
        public BooleanExpression Condition
        {
            get { return FindDescendant<BooleanExpression>(); }
        }

        public AnyStatement MainStatement
        {
            get { return FindDescendant<AnyStatement>(0); }
        }

        public AnyStatement ElseStatement
        {
            get { return FindDescendant<AnyStatement>(1); }
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
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
