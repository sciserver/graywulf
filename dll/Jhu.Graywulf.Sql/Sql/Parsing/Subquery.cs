using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Subquery
    {
        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public OrderByClause OrderByClause
        {
            get { return FindDescendant<OrderByClause>(); }
        }

        public static Subquery Create(SelectStatement ss)
        {
            var sq = new Subquery();

            sq.Stack.AddLast(BracketOpen.Create());
            sq.Stack.AddLast(ss);
            sq.Stack.AddLast(BracketClose.Create());

            return sq;
        }

        public IEnumerable<QuerySpecification> EnumerateQuerySpecifications()
        {
            return QueryExpression.EnumerateQuerySpecifications();
        }
    }
}
