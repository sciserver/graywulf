using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    public class SqlQueryVisitorOptions
    {
        #region Private member variables

        private ExpressionTraversalMethod expressionTraversal;
        private ExpressionTraversalMethod logicalExpressionTraversal;
        private bool visitExpressionSubqueries;
        private bool visitExpressionPredicates;
        private bool visitPredicateSubqueries;
        private bool visitPredicateExpressions;
        private bool visitSchemaReferences;

        #endregion
        #region Properties

        public ExpressionTraversalMethod ExpressionTraversal
        {
            get { return expressionTraversal; }
            set { expressionTraversal = value; }
        }

        public ExpressionTraversalMethod LogicalExpressionTraversal
        {
            get { return logicalExpressionTraversal; }
            set { logicalExpressionTraversal = value; }
        }

        public bool VisitExpressionSubqueries
        {
            get { return visitExpressionSubqueries; }
            set { visitExpressionSubqueries = value; }
        }

        public bool VisitExpressionPredicates
        {
            get { return visitExpressionPredicates; }
            set { visitExpressionPredicates = value; }
        }

        public bool VisitPredicateSubqueries
        {
            get { return visitPredicateSubqueries; }
            set { visitPredicateSubqueries = value; }
        }

        public bool VisitPredicateExpressions
        {
            get { return visitPredicateExpressions; }
            set { visitPredicateExpressions = value; }
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
            this.expressionTraversal = ExpressionTraversalMethod.Infix;
            this.logicalExpressionTraversal = ExpressionTraversalMethod.Infix;
            this.visitExpressionSubqueries = true;
            this.visitExpressionPredicates = true;
            this.visitPredicateSubqueries = true;
            this.visitPredicateExpressions = true;
            this.visitSchemaReferences = false;
        }

        private void CopyMembers(SqlQueryVisitorOptions old)
        {
            this.expressionTraversal = old.expressionTraversal;
            this.logicalExpressionTraversal = old.logicalExpressionTraversal;
            this.visitExpressionSubqueries = old.visitExpressionSubqueries;
            this.visitExpressionPredicates = old.visitExpressionPredicates;
            this.visitPredicateSubqueries = old.visitPredicateSubqueries;
            this.visitPredicateExpressions = old.visitPredicateExpressions;
            this.visitSchemaReferences = old.visitSchemaReferences;
        }

        #endregion
    }
}
