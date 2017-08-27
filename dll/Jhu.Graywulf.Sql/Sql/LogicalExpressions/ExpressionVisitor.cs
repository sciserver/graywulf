using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class ExpressionVisitor
    {
        public Expression Visit(Expression node)
        {
            return node.Accept(this);
        }

        protected internal virtual Expression VisitOperatorAnd(OperatorAnd node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            if (node.Left == left && node.Right == right)
            {
                return node;
            }
            else
            {
                return new OperatorAnd(left, right);
            }
        }

        protected internal virtual Expression VisitOperatorOr(OperatorOr node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            if (node.Left == left && node.Right == right)
            {
                return node;
            }
            else
            {
                return new OperatorOr(left, right);
            }
        }

        protected internal virtual Expression VisitOperatorNot(OperatorNot node)
        {
            var op = Visit(node.Operand);

            if (op == node.Operand)
            {
                return node;
            }
            else
            {
                return new OperatorNot(op);
            }
        }

        protected internal virtual Expression VisitPredicate(Predicate node)
        {
            return node;
        }

    }
}
