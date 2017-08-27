using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class UnaryExpression : Expression
    {
        public Expression Operand;

        public UnaryExpression()
        {
            Operand = null;
        }

        public UnaryExpression(Expression operand)
        {
            Operand = operand;
        }
    }
}
