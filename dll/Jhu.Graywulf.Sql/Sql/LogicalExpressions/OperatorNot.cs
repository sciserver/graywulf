using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class OperatorNot : UnaryOperator
    {
        public override int Precedence
        {
            get { return 3; }
        }

        public OperatorNot()
        {
        }

        public OperatorNot(ExpressionTreeNode operand)
            : base(operand)
        {
        }

        protected internal override ExpressionTreeNode Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitOperatorNot(this);
        }
        
        protected internal override LogicalExpression GetParsingTree(ExpressionTreeNode parent)
        {
            var sc = Operand.GetParsingTree(this);
            sc.Stack.AddFirst(Parsing.Whitespace.Create());
            sc.Stack.AddFirst(Parsing.LogicalNotOperator.Create());
            return sc;
        }

        public override string ToString()
        {
            return String.Format("NOT {0}", Operand.ToString(this));
        }
    }
}
