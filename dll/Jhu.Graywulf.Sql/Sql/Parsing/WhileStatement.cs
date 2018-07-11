using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class WhileStatement
    {
        public BooleanExpression Condition
        {
            get
            {
                return FindDescendant<BooleanExpression>();
            }
        }

        public AnyStatement Statement
        {
            get
            {
                return FindDescendant<AnyStatement>();
            }
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield return Statement;
        }
    }
}
