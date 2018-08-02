using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class ExpressionTreeBuilder : SqlQueryVisitorSink
    {
        private Queue<Node> stack;

        public ExpressionTreeNode Execute(LogicalExpression node)
        {
            var visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Prefix,
                    VisitExpressionSubqueries = false,
                    VisitPredicateSubqueries = false
                }
            };

            stack = new Queue<Node>();
            visitor.Execute(node);

            if (stack.Count > 0)
            {
                return BuildTree();
            }
            else
            {
                return null;
            }
        }

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            switch (node)
            {
                case LogicalNotOperator n:
                    stack.Enqueue(n);
                    break;
                case LogicalOperator n:
                    stack.Enqueue(n);
                    break;
                case Parsing.Predicate n:
                    stack.Enqueue(n);
                    break;
            }
        }

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, IDatabaseObjectReference node)
        {
        }

        private ExpressionTreeNode BuildTree()
        {
            var current = stack.Dequeue();
                        
            switch (current)
            {
                case LogicalNotOperator n:
                    return new OperatorNot(BuildTree());
                case LogicalOperator n when n.IsAnd:
                    return new OperatorAnd(BuildTree(), BuildTree());
                case LogicalOperator n when n.IsOr:
                    return new OperatorOr(BuildTree(), BuildTree());
                case Parsing.Predicate n:
                    return new Predicate(n);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
