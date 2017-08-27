using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class OperatorAnd : BinaryExpression
    {
        public override int Precedence
        {
            get { return 2; }
        }

        public OperatorAnd()
        {
        }

        public OperatorAnd(Expression left, Expression right)
            : base(left, right)
        {
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorAnd(this);
        }

        public IEnumerable<Expression> EnumerateTerms()
        {
            var stack = new Stack<Expression>();
            EnumerateTermsInternal(this, stack);

            return stack;
        }

        private void EnumerateTermsInternal(OperatorAnd node, Stack<Expression> stack)
        {
            if (node.Left is OperatorAnd)
            {
                EnumerateTermsInternal((OperatorAnd)node.Left, stack);
            }
            else
            {
                stack.Push(node.Left);
            }

            if (node.Right is OperatorAnd)
            {
                EnumerateTermsInternal((OperatorAnd)node.Right, stack);
            }
            else
            {
                stack.Push(node.Right);
            }
        }

        protected override Parsing.LogicalOperator GetParsingTreeOperator()
        {
            return Parsing.LogicalOperator.CreateAnd();
        }

        public override string ToString()
        {
            return String.Format("{0} AND {1}", Left.ToString(this), Right.ToString(this));
        }
    }
}
