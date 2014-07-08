using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public class Predicate : Expression
    {
        public override int Precedence
        {
            get { return 0; }
        }

        public Jhu.Graywulf.SqlParser.Predicate Value;

        public Predicate(Jhu.Graywulf.SqlParser.Predicate value)
        {
            this.Value = value;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitPredicate(this);
        }

        public override SearchCondition GetParsingTree()
        {
            // *** TODO Value.AddLeadingWhitespace();

            var sc = new SearchCondition();
            sc.Stack.AddLast(Value);
            return sc;
        }

        public override string ToString()
        {
            return Value.ToString().Trim();
        }
    }
}
