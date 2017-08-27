using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Subquery : ISelect
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

        public IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            return QueryExpression.EnumerateSourceTables(recursive);
        }

        public IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive)
        {
            return QueryExpression.EnumerateSourceTableReferences(recursive);
        }
    }
}
