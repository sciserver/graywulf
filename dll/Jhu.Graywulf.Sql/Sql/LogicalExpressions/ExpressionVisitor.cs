using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class ExpressionVisitor
    {
        public ExpressionTreeNode Visit(ExpressionTreeNode node)
        {
            return node.Accept(this);
        }

        protected internal virtual ExpressionTreeNode VisitOperatorAnd(OperatorAnd node)
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

        protected internal virtual ExpressionTreeNode VisitOperatorOr(OperatorOr node)
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

        protected internal virtual ExpressionTreeNode VisitOperatorNot(OperatorNot node)
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

        protected internal virtual ExpressionTreeNode VisitPredicate(Predicate node)
        {
            return node;
        }

    }
}
