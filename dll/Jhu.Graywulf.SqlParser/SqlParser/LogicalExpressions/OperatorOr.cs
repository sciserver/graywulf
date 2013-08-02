using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public class OperatorOr : BinaryExpression
    {
        public override int Precedence
        {
            get { return 1; }
        }

        public OperatorOr()
        {
        }

        public OperatorOr(Expression left, Expression right)
            : base(left, right)
        {
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorOr(this);
        }

        public IEnumerable<Expression> EnumerateTerms()
        {
            var stack = new Stack<Expression>();
            EnumerateTermsInternal(this, stack);

            return stack;
        }

        private void EnumerateTermsInternal(OperatorOr node, Stack<Expression> stack)
        {
            if (node.Left is OperatorOr)
            {
                EnumerateTermsInternal((OperatorOr)node.Left, stack);
            }
            else
            {
                stack.Push(node.Left);
            }

            if (node.Right is OperatorOr)
            {
                EnumerateTermsInternal((OperatorOr)node.Right, stack);
            }
            else
            {
                stack.Push(node.Right);
            }
        }

        protected override LogicalOperator GetParsingTreeOperator()
        {
            return LogicalOperator.CreateOr();
        }

        public override string ToString()
        {
            return String.Format("{0} OR {1}", Left.ToString(), Right.ToString());
        }
    }
}
