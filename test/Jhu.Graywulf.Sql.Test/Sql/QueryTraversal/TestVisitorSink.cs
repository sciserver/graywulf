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

        public string Execute(Expression node, ExpressionTraversalMode direction)
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = direction,
                    VisitExpressionSubqueries = false,
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

        protected virtual void Accept(Token node)
        {
            // Default dispatch, do nothing
        }

        private void Write(string s)
        {
            w.Write(s);
        }

        private void Write(Jhu.Graywulf.Parsing.Token token)
        {
            w.Write(token.Value + " ");
        }

        public virtual void Accept(UnaryOperator node)
        {
            Write(node);
        }

        public virtual void Accept(BinaryOperator node)
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

        public virtual void Accept(CountStar node)
        {
            Write(node);
        }

        public virtual void Accept(ColumnIdentifier node)
        {
            Write(node);
        }

        public virtual void Accept(ExpressionSubquery node)
        {
            Write("subquery ");
        }

        public virtual void Accept(UdtStaticPropertyAccess node)
        {
            Write(node.DataTypeIdentifier);
            Write("::");
            Write(node.PropertyName);
        }

        public virtual void Accept(UdtStaticMethodCall node)
        {
            Write(node.DataTypeIdentifier);
            Write("::");
            Write(node.MethodName);
        }

        public virtual void Accept(UdtPropertyAccess node)
        {
            Write(".");
            Write(node.PropertyName);
        }

        public virtual void Accept(UdtMethodCall node)
        {
            Write(".");
            Write(node.MethodName);
        }

        public virtual void Accept(ScalarFunctionCall node)
        {
            Write(node.FunctionIdentifier);
        }

        public virtual void Accept(WindowedFunctionCall node)
        {
            Write(node.FunctionIdentifier);
        }

        public virtual void Accept(Argument node)
        {
            Write(", ");
        }

        public virtual void Accept(OrderByArgument node)
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
