using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.SqlParser
{
    /// <summary>
    /// Implements a SELECT statement including the ORDER BY clause
    /// </summary>
    public partial class SelectStatement : ISelect
    {
        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public OrderByClause OrderByClause
        {
            get { return FindDescendant<OrderByClause>(); }
        }

        // TODO: remove this and use SET PARTITION or something else for the whole script
        public virtual bool IsPartitioned
        {
            get
            {
                var qs = QueryExpression.EnumerateQuerySpecifications().FirstOrDefault<QuerySpecification>();
                var ts = qs.EnumerateSourceTables(false).FirstOrDefault();

                if (ts == null || !(ts is SimpleTableSource))
                {
                    // It might be a constant query (SELECT 1), that's definitely not partitioned
                    return false;
                }

                return ((SimpleTableSource)ts).IsPartitioned;
            }
        }

        public static SelectStatement Create(QueryExpression qe)
        {
            var ss = new SelectStatement();
            ss.Stack.AddLast(qe);

            return ss;
        }

        public static SelectStatement Create(SelectList selectList, FromClause from)
        {
            var qs = QuerySpecification.Create(selectList, from);
            var qe = QueryExpression.Create(qs);

            return Create(qe);
        }
    }
}
