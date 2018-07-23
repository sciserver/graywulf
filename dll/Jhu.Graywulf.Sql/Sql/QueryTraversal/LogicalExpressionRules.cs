using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    /// <summary>
    /// Implements a specialized version of the shuntling yard algorithm to output
    /// expressions in reverse polish notation.
    /// </summary>
    class LogicalExpressionRules : ExpressionReshufflerRules
    {
        public override void Route(Token node)
        {
            switch (node)
            {
                // Predicates are inlined and sent directly to the output
                case Predicate n:
                    Inline(n);
                    break;

                // Default behavior: skip token
                default:
                    break;
            }
        }
    }
}
