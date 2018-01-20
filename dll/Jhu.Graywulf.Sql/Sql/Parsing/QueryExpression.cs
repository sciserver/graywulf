using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class QueryExpression
    {
        private TableReference resultsTableReference;
        
        public TableReference ResultsTableReference
        {
            get { return resultsTableReference; }
            set { resultsTableReference = value; }
        }

        public QuerySpecification FirstQuerySpecification
        {
            get { return FindDescendant<QuerySpecification>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.resultsTableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (QueryExpression)other;

            this.resultsTableReference = old.resultsTableReference;
        }

        public static QueryExpression Create(QuerySpecification qs)
        {
            var qe = new QueryExpression();

            qe.Stack.AddLast(qs);

            qe.resultsTableReference = new TableReference(qe);

            return qe;
        }

        public static QueryExpression Create(QueryExpressionBrackets qeb)
        {
            var qe = new QueryExpression();

            qe.Stack.AddLast(qeb);

            qe.resultsTableReference = new TableReference(qe);

            return qe;
        }

        public static QueryExpression Create(QuerySpecification qs1, QuerySpecification qs2, QueryOperator op)
        {
            var qe = new QueryExpression();
            
            var qeb1 = QueryExpressionBrackets.Create(qs1);
            var qeb2 = QueryExpressionBrackets.Create(qs2);
            var qe2 = QueryExpression.Create(qeb2);

            qe.Stack.AddLast(qeb1);
            qe.Stack.AddLast(Whitespace.CreateNewLine());
            qe.Stack.AddLast(op);
            qe.Stack.AddLast(Whitespace.CreateNewLine());
            qe.Stack.AddLast(qe2);

            qe.resultsTableReference = new TableReference(qe);

            return qe;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.resultsTableReference = new TableReference(this);
        }

        public IEnumerable<QuerySpecification> EnumerateQuerySpecifications()
        {
            return EnumerateDescendants<QuerySpecification>();
        }

        public IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            foreach (var qs in EnumerateDescendants<QuerySpecification>())
            {
                foreach (var ts in qs.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }
        }

        public IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive)
        {
            return EnumerateSourceTables(recursive).Select(ts => ts.TableReference);
        }
    }
}
