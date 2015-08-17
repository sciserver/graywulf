using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class QueryExpressionBrackets
    {
        public static QueryExpressionBrackets Create(QueryExpression qe)
        {
            var qeb = new QueryExpressionBrackets();

            qeb.Stack.AddLast(BracketOpen.Create());
            qeb.Stack.AddLast(qe);
            qeb.Stack.AddLast(BracketClose.Create());

            return qeb;
        }

        public static QueryExpressionBrackets Create(QuerySpecification qs)
        {
            var qe = QueryExpression.Create(qs);
            return Create(qe);
        }
    }
}
