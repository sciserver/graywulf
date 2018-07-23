using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public abstract class ExpressionTreeNode
    {
        public abstract int Precedence { get; }

        internal protected abstract ExpressionTreeNode Accept(ExpressionVisitor visitor);

        public Parsing.LogicalExpression GetParsingTree()
        {
            return GetParsingTree(null);
        }

        protected internal virtual Parsing.LogicalExpression GetParsingTree(ExpressionTreeNode parent)
        {
            return GetParsingTree();
        }

        public virtual string ToString(ExpressionTreeNode parent)
        {
            return ToString();
        }
    }
}
