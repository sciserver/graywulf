using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.QueryRewriting
{
    public abstract class QueryRewriterBase : SqlQueryVisitorSink
    {
        #region Private member variables

        private SqlQueryVisitor visitor;

        #endregion
        #region Properties

        protected SqlQueryVisitor Visitor
        {
            get { return visitor; }
        }

        #endregion
        #region Constructors and initializers

        protected QueryRewriterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.visitor = CreateQueryVisitor();
        }

        protected virtual SqlQueryVisitor CreateQueryVisitor()
        {
            return new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Infix,
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    VisitExpressionSubqueries = true,
                    VisitPredicateSubqueries = true,
                    VisitSchemaReferences = false
                }
            };
        }

        #endregion
        #region Main entry point

        public virtual void Execute(StatementBlock parsingTree)
        {
            Visitor.Execute(parsingTree);
        }

        #endregion
    }
}
