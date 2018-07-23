using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class Predicate : ExpressionTreeNode
    {
        public override int Precedence
        {
            get { return 4; }
        }

        public Parsing.Predicate Value;

        public Predicate(Parsing.Predicate value)
        {
            this.Value = value;
        }

        protected internal override ExpressionTreeNode Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitPredicate(this);
        }

        protected internal override Parsing.LogicalExpression GetParsingTree(ExpressionTreeNode parent)
        {
            // *** TODO Value.AddLeadingWhitespace();

            var sc = new Parsing.LogicalExpression();
            sc.Stack.AddLast(Value);
            return sc;
        }

        public override string ToString()
        {
            return Value.Value.Trim();
        }
    }
}
