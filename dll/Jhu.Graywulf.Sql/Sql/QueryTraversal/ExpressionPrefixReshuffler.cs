using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    abstract class ExpressionPrefixReshuffler : ExpressionPostfixReshuffler
    {

        protected ExpressionPrefixReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
            : base(visitor, sink)
        {
        }

        public override void Output(Token n)
        {
            
        }

        public override void Route(Token node)
        {
            switch (node)
            {
                // Brackets work the opposite because reading from the end
                case BracketClose n:
                    Push(n);
                    break;
                case BracketOpen n:
                    Pop(n);
                    break;
                default:
                    base.Route(node);
                    break;

                // Windowed functions are rather special, for prefix, push to output
                // to put it after function just like its arguments
                case OverClause n:
                    Output(n);
                    break;

                case Comma n:
                    Push(n);
                    break;
            }
        }

    }
}
