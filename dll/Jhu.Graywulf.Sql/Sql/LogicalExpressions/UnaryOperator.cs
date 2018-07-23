using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class UnaryOperator : ExpressionTreeNode
    {
        public ExpressionTreeNode Operand;

        public UnaryOperator()
        {
            Operand = null;
        }

        public UnaryOperator(ExpressionTreeNode operand)
        {
            Operand = operand;
        }
    }
}
