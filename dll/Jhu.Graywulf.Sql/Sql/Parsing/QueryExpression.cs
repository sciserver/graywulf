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
        public TableReference ResultsTableReference
        {
            get { return FirstQuerySpecification.ResultsTableReference; }
            set { FirstQuerySpecification.ResultsTableReference = value; }
        }

        public QuerySpecification FirstQuerySpecification
        {
            get { return FindDescendant<QuerySpecification>(); }
        }

        public static QueryExpression Create(QuerySpecification qs)
        {
            var qe = new QueryExpression();
            qe.Stack.AddLast(qs);
            return qe;
        }

        public static QueryExpression Create(QueryExpressionBrackets qeb)
        {
            var qe = new QueryExpression();
            qe.Stack.AddLast(qeb);
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

            return qe;
        }

        public IEnumerable<QuerySpecification> EnumerateQuerySpecifications()
        {
            return EnumerateDescendants<QuerySpecification>();
        }
    }
}
