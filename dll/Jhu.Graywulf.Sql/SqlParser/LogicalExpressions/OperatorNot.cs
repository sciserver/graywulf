using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public class OperatorNot : UnaryExpression
    {
        public override int Precedence
        {
            get { return 3; }
        }

        public OperatorNot()
        {
        }

        public OperatorNot(Expression operand)
            : base(operand)
        {
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorNot(this);
        }

        public override BooleanExpression GetParsingTree()
        {
            var sc = Operand.GetParsingTree();
            sc.Stack.AddFirst(Whitespace.Create());
            sc.Stack.AddFirst(LogicalNot.Create());
            return sc;
        }

        public override string ToString()
        {
            return String.Format("NOT {0}", Operand.ToString());
        }
    }
}
