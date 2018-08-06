using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    class TestVisitorSink : SqlQueryVisitorSink
    {
        private SqlQueryVisitor visitor;
        private StringWriter w;

        public string Execute(StatementBlock node)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Infix,
                    VisitExpressionSubqueries = false,
                    VisitPredicateSubqueries = false,
                    VisitLiterals = true,
                    VisitSymbols = true,
                }
            };
            using (w = new StringWriter())
            {
                visitor.Execute(node);
                return w.ToString();
            }
        }

        public string Execute(Expression node, ExpressionTraversalMethod direction)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = direction,
                    VisitExpressionSubqueries = false,
                    VisitExpressionPredicates = false,
                    VisitLiterals = true,
                    VisitSymbols = true,
                }
            };
            using (w = new StringWriter())
            {
                visitor.Execute(node);
                return w.ToString();
            }
        }

        public string Execute(LogicalExpression node, ExpressionTraversalMethod direction)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    LogicalExpressionTraversal = direction,
                    VisitPredicateSubqueries = false,
                    VisitPredicateExpressions = false,
                    VisitLiterals = true,
                    VisitSymbols = true,
                }
            };
            using (w = new StringWriter())
            {
                visitor.Execute(node);
                return w.ToString();
            }
        }

        public string Execute(Expression node, ExpressionTraversalMethod expressionTraversal, ExpressionTraversalMethod logicalExpressionTraversal)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = expressionTraversal,
                    LogicalExpressionTraversal = logicalExpressionTraversal,
                    VisitPredicateSubqueries = false,
                    VisitExpressionSubqueries = false,
                    VisitLiterals = true,
                    VisitSymbols = true,
                }
            };
            using (w = new StringWriter())
            {
                visitor.Execute(node);
                return w.ToString();
            }
        }

        public string Execute(LogicalExpression node, ExpressionTraversalMethod expressionTraversal, ExpressionTraversalMethod logicalExpressionTraversal)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = expressionTraversal,
                    LogicalExpressionTraversal = logicalExpressionTraversal,
                    VisitPredicateSubqueries = false,
                    VisitExpressionSubqueries = false,
                    VisitLiterals = true,
                    VisitSymbols = true,
                }
            };
            using (w = new StringWriter())
            {
                visitor.Execute(node);
                return w.ToString();
            }
        }

        protected override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            Accept((dynamic)node);
        }

        protected override void AcceptVisitor(SqlQueryVisitor visitor, IDatabaseObjectReference node)
        {
        }

        protected virtual void Accept(Token node)
        {
            // Default dispatch, write out token
        }

        private void Write(string s)
        {
            w.Write(s);
        }

        private void Write(Jhu.Graywulf.Parsing.Token token)
        {
            w.Write(token.Value + " ");
        }

        private void Write(FunctionCall node)
        {
            if (visitor.Options.ExpressionTraversal != ExpressionTraversalMethod.Infix)
            {
                w.Write("`" + node.ArgumentCount + " ");
            }
        }

        public virtual void Accept(Label node)
        {
            Write(node);
        }
        
        public virtual void Accept(Operator node)
        {
            Write(node);
        }

        public virtual void Accept(Literal node)
        {
            Write(node);
        }

        public virtual void Accept(NumericConstant node)
        {
            Write(node);
        }

        public virtual void Accept(StringConstant node)
        {
            Write(node);
        }

        public virtual void Accept(DatePart node)
        {
            Write(node);
        }

        public virtual void Accept(Symbol node)
        {
            Write(node);
        }

        public virtual void Accept(StarArgument node)
        {
            Write(node);
        }

        public virtual void Accept(BracketOpen node)
        {
            Write(node);
        }

        public virtual void Accept(BracketClose node)
        {
            Write(node);
        }

        public virtual void Accept(ValueAssignmentOperator node)
        {
            Write(node);
        }

        public virtual void Accept(TableOrViewIdentifier node)
        {
            Write(node);
        }

        public virtual void Accept(IndexName node)
        {
            Write(node);
        }

        public virtual void Accept(ConstraintName node)
        {
            Write(node);
        }
        
        public virtual void Accept(QueryOperator node)
        {
            Write(node);
        }
        
        public virtual void Accept(DataTypeIdentifier node)
        {
            Write(node);
        }

        public virtual void Accept(ObjectName node)
        {
            Write(node);
        }

        public virtual void Accept(MemberAccess node)
        {
            Write(node.MemberName);
        }

        public virtual void Accept(MemberCall node)
        {
            Write(node.MemberName);
            Write(node);
        }

        public virtual void Accept(Constant node)
        {
            Write(node);
        }

        public virtual void Accept(UserVariable node)
        {
            Write(node);
        }

        public virtual void Accept(SystemVariable node)
        {
            Write(node);
        }

        public virtual void Accept(ColumnName node)
        {
            Write(node);
        }

        public virtual void Accept(ColumnIdentifier node)
        {
            Write(node);
        }

        public virtual void Accept(ColumnAlias node)
        {
            Write(node);
        }

        public virtual void Accept(ExpressionSubquery node)
        {
            Write("<subquery> ");
        }

        public virtual void Accept(SemiJoinSubquery node)
        {
            Write("<subquery> ");
        }

        public virtual void Accept(UdtStaticPropertyAccess node)
        {
            Write(node.PropertyName);
        }

        public virtual void Accept(UdtPropertyAccess node)
        {
            Write(node.PropertyName);
        }

        public virtual void Accept(UdtStaticMethodCall node)
        {
            Write(node.MethodName);
            Write(node);
        }

        public virtual void Accept(UdtMethodCall node)
        {
            Write(node.MethodName);
            Write(node);
        }

        public virtual void Accept(SystemFunctionCall node)
        {
            Write(node.FunctionName);
            Write(node);
        }

        public virtual void Accept(ScalarFunctionCall node)
        {
            Write(node.FunctionIdentifier);
            Write(node);
        }

        public virtual void Accept(WindowedFunctionCall node)
        {
            Write(node.FunctionIdentifier);
            Write(node);
        }

        public virtual void Accept(ConvertFunctionCall node)
        {
            Write(node.FunctionName);
            Write(node);
        }

        public virtual void Accept(CastAndParseFunctionCall node)
        {
            Write(node.FunctionName);
            Write(node);
        }

        public virtual void Accept(DateFunctionCall node)
        {
            Write(node.FunctionName);
            Write(node);
        }

        public virtual void Accept(IifFunctionCall node)
        {
            Write(node.FunctionName);
            Write(node);
        }

        public virtual void Accept(DataTypeArgument node)
        {
            Write(node);
        }

        public virtual void Accept(Comma node)
        {
            Write(", ");
        }

        public virtual void Accept(OverClause node)
        {
            Write("over ");
        }

        public virtual void Accept(PartitionByClause node)
        {
            Write("partitionby ");
        }

        public virtual void Accept(OrderByClause node)
        {
            Write("orderby ");
        }
    }
}
