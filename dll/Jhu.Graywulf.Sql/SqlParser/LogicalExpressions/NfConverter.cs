using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public abstract class NfConverter : ExpressionVisitor
    {
        protected internal override Expression VisitOperatorNot(OperatorNot node)
        {
            if (node.Operand is Predicate)
            {
                return node;
            }
            else if (node.Operand is OperatorNot)
            {
                // Cancel double negation
                return ((OperatorNot)node.Operand).Operand;
            }
            else if (node.Operand is OperatorAnd)
            {
                // Apply de Morgan's law
                var op = node.Operand as OperatorAnd;
                return Visit(new OperatorOr(new OperatorNot(op.Left), new OperatorNot(op.Right)));
            }
            else if (node.Operand is OperatorOr)
            {
                // Apply de Morgan's law
                var op = node.Operand as OperatorOr;
                return Visit(new OperatorAnd(new OperatorNot(op.Left), new OperatorNot(op.Right)));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
