using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.Validation
{
    public class SqlValidator
    {

        public SqlValidator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public void Execute(StatementBlock parsingTree)
        {
            foreach (var s in parsingTree.EnumerateDescendantsRecursive<IStatement>())
            {
                OnValidateStatement(s);
            }

            // TODO: validate entire script, not just the select
        }

        protected virtual void OnValidateStatement(IStatement statement)
        {
            if (statement is SelectStatement)
            {
                ValidateSelectStatement((SelectStatement)statement, 0);
            }
        }

        protected virtual void ValidateSelectStatement(SelectStatement selectStatement, int depth)
        {
            // Validate top level query specifications
            foreach (QuerySpecification qs in selectStatement.FindDescendantRecursive<QueryExpression>().EnumerateQuerySpecifications())
            {
                ValidateQuerySpecification(qs);
            }

            // Validate Order By
            OrderByClause orderby = selectStatement.FindDescendant<OrderByClause>();

            if (orderby != null)
            {
                // **** TODO
            }
        }

        protected void ValidateQuerySpecification(QuerySpecification qs)
        {
            // Call recursively for subqueries
#if false
            foreach (var sq in qs.EnumerateSubqueries())
            {
                ValidateQuerySpecification(sq);
            }
#endif

            ValidateTableReferences(qs);
            ValidateStarInSelectList(qs);

            // Loop through all column expressions of the select list and try to resolve them
            foreach (ColumnExpression ce in qs.FindDescendant<SelectList>().EnumerateDescendants<ColumnExpression>())
            {
                ValidateColumnExpression(qs, ce);
            }
        }

        protected void ValidateTableReferences(QuerySpecification qs)
        {
#if false
            foreach (var ats in qs.EnumerateTableSources(false))
            {
                var ts = ats.FindDescendant<SimpleTableSource>();
                if (ts != null)
                {
                    TableReference tr = ts.TableReference;

                    // Make sure no database name specified -> invalid in skyquery
                    if (tr.DatabaseName != null)
                    {
                        throw CreateException(ExceptionMessages.DatabaseNameNotAllowed, null, tr.DatabaseName, ts);
                    }

                    return;
                }

                throw new NotImplementedException();
            }
#endif
        }

        /// <summary>
        /// Looks for a * in the select list and throws an exception if there's any
        /// </summary>
        /// <param name="qs"></param>
        protected void ValidateStarInSelectList(QuerySpecification qs)
        {
            // Look for stars (*) in the select list
            foreach (var i in qs.FindDescendant<SelectList>().Nodes)
            {
                if (i is Mul)
                {
                    throw CreateException(ExceptionMessages.StarColumnNotAllowed, i);
                }
            }
        }

        protected void ValidateColumnExpression(QuerySpecification qs, ColumnExpression ce)
        {
            foreach (ColumnIdentifier ci in ce.EnumerateDescendantsRecursive<ColumnIdentifier>(null))
            {
                ValidateColumnIdentifier(qs, ci);
            }

            // *** TODO: look for duplicate aliases
        }

        protected void ValidateColumnIdentifier(QuerySpecification qs, ColumnIdentifier ci)
        {
            // TODO: modify to look for stars inside expressions
            /*
            if (ci.ColumnReference.IsStar)
            {
                // Check if it's a count(*), that's the only expression allowed to
                // contain a *
                var fc = ci.FindAscendant<FunctionCall>();
                if (fc != null)
                {
                    var fn = fc.FindDescendantRecursive<FunctionName>();
                    if (fn != null && SqlParser.ComparerInstance.Compare(fn.Value, "COUNT") == 0)
                    {
                        return;
                    }
                }

                throw CreateException(ExceptionMessages.StarColumnNotAllowed, null, null, ci);
            }*/
        }

        protected Exception CreateException(string message, Exception innerException, string objectName, Node node)
        {
            string msg;
            Token t = null;

            if (node != null)
            {
                msg = String.Format(message, objectName, node.Line + 1, node.Col + 1);
            }
            else
            {
                msg = String.Format(message, objectName);
            }

            ValidatorException ex = new ValidatorException(msg, innerException);
            ex.Token = t;

            return ex;
        }

        protected Exception CreateException(string message, Token token)
        {
            string msg;

            msg = String.Format(message, null, token.Line + 1, token.Pos + 1);

            ValidatorException ex = new ValidatorException(msg);
            ex.Token = token;

            return ex;
        }
    }
}
