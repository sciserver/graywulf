using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public class SqlQueryVisitorOptions
    {
        #region Private member variables

        private ExpressionTraversalMode expressionTraversal;
        private ExpressionTraversalMode logicalExpressionTraversal;
        private bool visitExpressionSubqueries;
        private bool visitPredicateSubqueries;
        private bool visitSchemaReferences;

        #endregion
        #region Properties

        public ExpressionTraversalMode ExpressionTraversal
        {
            get { return expressionTraversal; }
            set { expressionTraversal = value; }
        }

        public ExpressionTraversalMode LogicalExpressionTraversal
        {
            get { return logicalExpressionTraversal; }
            set { logicalExpressionTraversal = value; }
        }

        public bool VisitExpressionSubqueries
        {
            get { return visitExpressionSubqueries; }
            set { visitExpressionSubqueries = value; }
        }

        public bool VisitPredicateSubqueries
        {
            get { return visitPredicateSubqueries; }
            set { visitPredicateSubqueries = value; }
        }

        public bool VisitSchemaReferences
        {
            get { return visitSchemaReferences; }
            set { visitSchemaReferences = value; }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryVisitorOptions()
        {
            InitializeMembers();
        }

        public SqlQueryVisitorOptions(SqlQueryVisitorOptions old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.expressionTraversal = ExpressionTraversalMode.Infix;
            this.logicalExpressionTraversal = ExpressionTraversalMode.Infix;
            this.visitExpressionSubqueries = true;
            this.visitPredicateSubqueries = true;
            this.visitSchemaReferences = false;
        }

        private void CopyMembers(SqlQueryVisitorOptions old)
        {
            this.expressionTraversal = old.expressionTraversal;
            this.logicalExpressionTraversal = old.logicalExpressionTraversal;
            this.visitExpressionSubqueries = old.visitExpressionSubqueries;
            this.visitPredicateSubqueries = old.visitPredicateSubqueries;
            this.visitSchemaReferences = old.visitSchemaReferences;
        }

        #endregion
    }
}
