using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class PredicateVisitor : SqlQueryVisitorSink
    {
        private SqlQueryVisitor visitor;
        private bool isSpecificToTable;
        private TableReference matchTableReference;
        private DatabaseObject matchTable;

        public bool IsSpecificToTable(Parsing.Predicate node, TableReference tableReference)
        {
            CreateVisitor();

            matchTableReference = tableReference;
            isSpecificToTable = true;

            visitor.Execute(node);

            matchTableReference = null;
            visitor = null;
            return isSpecificToTable;
        }

        public bool IsSpecificToTable(Parsing.Predicate node, DatabaseObject table)
        {
            CreateVisitor();

            matchTable = table;
            isSpecificToTable = true;

            visitor.Execute(node);

            matchTable = null;
            visitor = null;
            return isSpecificToTable;
        }

        private void CreateVisitor()
        {
            visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Infix,
                    VisitExpressionSubqueries = false,
                    VisitPredicateSubqueries = false
                }
            };
        }

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            // TODO: decide whether compare by reference or equality because multiple-referenced
            // tables might be cached only once or multiple types

            if (node is ITableReference n)
            {
                if (matchTableReference != null)
                {
                    if (n.TableReference != null &&
                        //!TableReferenceEqualityComparer.Default.Equals(n.TableReference, matchTableReference))
                        n.TableReference != matchTableReference)
                    {
                        isSpecificToTable = false;
                    }
                }
                else if (matchTable != null)
                {
                    if (n.TableReference != null &&
                        n.TableReference.DatabaseObject == null || n.TableReference.DatabaseObject != matchTable)
                    {
                        isSpecificToTable = false;
                    }
                }
            }
        }

    }
}
