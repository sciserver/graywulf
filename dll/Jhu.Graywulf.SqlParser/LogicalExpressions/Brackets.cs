using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    /// <summary>
    /// Represents brackets in the raw espression list during conversion
    /// from the parsing tree.
    /// </summary>
    /// <remarks>
    /// This class is used by the shunting-yard
    /// </remarks>
    public class Brackets : Expression
    {
        public override int Precedence
        {
            get { return 0; }
        }

        public Expression Expression;

        public Brackets(Expression expression)
        {
            this.Expression = expression;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override SearchCondition GetParsingTree()
        {
            throw new NotImplementedException();
        }

        public override string ToString(Expression parent)
        {
            return ToString();
        }

        public override string ToString()
        {
            return String.Format("({0})", Expression.ToString());
        }
    }
}
