using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
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

        public override Parsing.LogicalExpression GetParsingTree()
        {
            var lsc = Left.GetParsingTree(this);
            var rsc = Right.GetParsingTree(this);

            lsc.Stack.AddLast(Parsing.Whitespace.Create());
            lsc.Stack.AddLast(GetParsingTreeOperator());
            lsc.Stack.AddLast(Parsing.Whitespace.Create());
            lsc.Stack.AddLast(rsc);

            return lsc;
        }

        protected abstract Parsing.LogicalOperator GetParsingTreeOperator();
    }
}
