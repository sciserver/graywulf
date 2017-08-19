using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public abstract class Expression
    {
        public abstract int Precedence { get; }

        internal protected abstract Expression Accept(ExpressionVisitor visitor);

        public abstract BooleanExpression GetParsingTree();

        protected internal virtual BooleanExpression GetParsingTree(Expression parent)
        {
            if (Precedence > 0 && parent.Precedence > Precedence)
            {
                return BooleanExpression.Create(false, BooleanExpressionBrackets.Create(GetParsingTree()));
            }
            else
            {
                return GetParsingTree();
            }
        }

        public virtual string ToString(Expression parent)
        {
            if (Precedence > 0 && parent.Precedence > Precedence)
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
