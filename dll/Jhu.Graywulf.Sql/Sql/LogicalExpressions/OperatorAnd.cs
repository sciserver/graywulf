using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class OperatorAnd : BinaryOperator
    {
        public override int Precedence
        {
            get { return 2; }
        }

        public OperatorAnd()
        {
        }

        public OperatorAnd(ExpressionTreeNode left, ExpressionTreeNode right)
            : base(left, right)
        {
        }

        protected internal override ExpressionTreeNode Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorAnd(this);
        }

        public IEnumerable<ExpressionTreeNode> EnumerateTerms()
        {
            var stack = new Stack<ExpressionTreeNode>();
            EnumerateTermsInternal(this, stack);

            return stack;
        }

        private void EnumerateTermsInternal(OperatorAnd node, Stack<ExpressionTreeNode> stack)
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
