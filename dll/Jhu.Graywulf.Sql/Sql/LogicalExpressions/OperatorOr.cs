using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
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

        public OperatorOr(ExpressionTreeNode left, ExpressionTreeNode right)
            : base(left, right)
        {
        }

        protected internal override ExpressionTreeNode Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorOr(this);
        }

        public IEnumerable<ExpressionTreeNode> EnumerateTerms()
        {
            var stack = new Stack<ExpressionTreeNode>();
            EnumerateTermsInternal(this, stack);

            return stack;
        }

        private void EnumerateTermsInternal(OperatorOr node, Stack<ExpressionTreeNode> stack)
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

        protected override Parsing.LogicalOperator GetParsingTreeOperator()
        {
            return Parsing.LogicalOperator.CreateOr();
        }

        public override string ToString()
        {
            return String.Format("{0} OR {1}", Left.ToString(), Right.ToString());
        }
    }
}
