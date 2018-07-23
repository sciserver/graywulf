using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class UnaryExpression : ExpressionTreeNode
    {
        public ExpressionTreeNode Operand;

        public UnaryExpression()
        {
            Operand = null;
        }

        public UnaryExpression(ExpressionTreeNode operand)
        {
            Operand = operand;
        }
    }
}
