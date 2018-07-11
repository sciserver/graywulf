using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryRewriting
{
    public abstract class QueryRewriterBase
    {

        #region Constructors and initializers

        protected QueryRewriterBase()
        {
        }

        #endregion

        public virtual void Execute(StatementBlock parsingTree)
        {
            foreach (var s in parsingTree.EnumerateSubStatements())
            {
                RewriteStatement(s);
            }
        }

        protected virtual void RewriteStatement(AnyStatement statement)
        {
            switch (statement.SpecificStatement)
            {
                case SelectStatement s:
                    RewriteSelectStatement(s);
                    break;
                default:
                    foreach (var s in statement.SpecificStatement.EnumerateSubStatements())
                    {
                        RewriteStatement(s);
                    }
                    break;
            }
        }

        protected virtual void RewriteSelectStatement(SelectStatement selectStatement)
        {
            var qe = selectStatement.QueryExpression;
            var into = selectStatement.IntoClause;
            var orderby = selectStatement.OrderByClause;

            if (qe != null)
            {
                RewriteQueryExpression(qe, 0, QueryContext.None);
            }

            if (into != null)
            {
                RewriteIntoClause(selectStatement, into);
            }

            if (orderby != null)
            {
                RewriteOrderByClause(selectStatement, orderby);
            }
        }

        protected virtual void RewriteQueryExpression(QueryExpression qe, int depth, QueryContext queryContext)
        {
            // Rewrite query specifications in the FROM clause
            // TODO: make this descend in the tree instead of use the enumerator
            var index = 0;
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                RewriteQuerySpecification(qs, depth, index++, queryContext);
            }
        }

        protected virtual void RewriteQuerySpecification(QuerySpecification qs, int depth, int index, QueryContext queryContext)
        {
            var selectList = qs.SelectList;

            if (selectList != null)
            {
                RewriteSelectList(qs, selectList, depth, index, queryContext);
            }

            foreach (var ts in qs.FromClause.EnumerateSourceTables(false))
            {

            }
        }

        protected virtual void RewriteSelectList(QuerySpecification qs, SelectList selectList, int depth, int index, QueryContext queryContext)
        {
        }

        protected virtual void RewriteIntoClause(SelectStatement selectStatement, IntoClause into)
        {
        }

        protected virtual void RewriteOrderByClause(SelectStatement selectStatement, OrderByClause orderby)
        {
        }

        
    }
}
