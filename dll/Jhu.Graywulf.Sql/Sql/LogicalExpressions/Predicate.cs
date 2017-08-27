using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class Predicate : Expression
    {
        public override int Precedence
        {
            get { return 0; }
        }

        public Parsing.Predicate Value;

        public Predicate(Parsing.Predicate value)
        {
            this.Value = value;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitPredicate(this);
        }

        public override Parsing.BooleanExpression GetParsingTree()
        {
            // *** TODO Value.AddLeadingWhitespace();

            var sc = new Parsing.BooleanExpression();
            sc.Stack.AddLast(Value);
            return sc;
        }

        public override string ToString()
        {
            return Value.Value.Trim();
        }
    }
}
