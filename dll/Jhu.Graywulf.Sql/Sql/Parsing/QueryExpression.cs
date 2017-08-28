using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class QueryExpression : ITableReference
    {
        private TableReference tableReference;

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return tableReference; }
        }

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (QueryExpression)other;

            this.tableReference = old.tableReference;
        }

        public static QueryExpression Create(QuerySpecification qs)
        {
            var qe = new QueryExpression();

            qe.Stack.AddLast(qs);

            qe.tableReference = new TableReference(qe);

            return qe;
        }

        public static QueryExpression Create(QueryExpressionBrackets qeb)
        {
            var qe = new QueryExpression();

            qe.Stack.AddLast(qeb);

            qe.tableReference = new TableReference(qe);

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

            qe.tableReference = new TableReference(qe);

            return qe;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableReference = new TableReference(this);
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
