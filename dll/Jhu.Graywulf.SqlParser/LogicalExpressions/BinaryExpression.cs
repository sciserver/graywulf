using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public abstract class BinaryExpression : Expression
    {
        public Expression Left;
        public Expression Right;

        public BinaryExpression()
        {
            this.Left = null;
            this.Right = null;
        }

        public BinaryExpression(Expression left, Expression right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override SearchCondition GetParsingTree()
        {
            var lsc = Left.GetParsingTree(this);
            var rsc = Right.GetParsingTree(this);

            lsc.Stack.AddLast(Whitespace.Create());
            lsc.Stack.AddLast(GetParsingTreeOperator());
            lsc.Stack.AddLast(Whitespace.Create());
            lsc.Stack.AddLast(rsc);

            return lsc;
        }

        protected abstract LogicalOperator GetParsingTreeOperator();
    }
}
