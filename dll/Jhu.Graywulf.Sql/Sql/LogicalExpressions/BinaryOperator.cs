using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class BinaryOperator : ExpressionTreeNode
    {
        public ExpressionTreeNode Left;
        public ExpressionTreeNode Right;

        public BinaryOperator()
        {
            this.Left = null;
            this.Right = null;
        }

        public BinaryOperator(ExpressionTreeNode left, ExpressionTreeNode right)
        {
            this.Left = left;
            this.Right = right;
        }
        
        protected abstract Parsing.LogicalOperator GetParsingTreeOperator();

        protected internal override LogicalExpression GetParsingTree(ExpressionTreeNode parent)
        {
            var lsc = Left.GetParsingTree(this);
            var rsc = Right.GetParsingTree(this);

            lsc.Stack.AddLast(Parsing.Whitespace.Create());
            lsc.Stack.AddLast(GetParsingTreeOperator());
            lsc.Stack.AddLast(Parsing.Whitespace.Create());
            lsc.Stack.AddLast(rsc);

            if (parent != null && parent.Precedence > Precedence)
            {
                return LogicalExpression.Create(false, Parsing.LogicalExpressionBrackets.Create(lsc));
            }
            else
            {
                return lsc;
            }
        }

        public override string ToString(ExpressionTreeNode parent)
        {
            if (parent.Precedence > Precedence)
            {
                return String.Format("({0})", ToString());
            }
            else
            {
                return ToString();
            }
        }
    }
}
