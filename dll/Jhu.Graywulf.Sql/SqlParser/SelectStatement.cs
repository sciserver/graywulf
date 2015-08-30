using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;


namespace Jhu.Graywulf.SqlParser
{
    /// <summary>
    /// Implements a SELECT statement including the ORDER BY clause
    /// </summary>
    public partial class SelectStatement
    {
        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public OrderByClause OrderByClause
        {
            get { return FindDescendant<OrderByClause>(); }
        }

        public virtual bool IsPartitioned
        {
            get
            {
                var qs = this.EnumerateQuerySpecifications().FirstOrDefault<QuerySpecification>();
                var ts = qs.EnumerateSourceTables(false).FirstOrDefault();

                if (ts == null || !(ts is SimpleTableSource))
                {
                    // It might be a constant query (SELECT 1), that's definitely not partitioned
                    return false;
                }

                return ((SimpleTableSource)ts).IsPartitioned;
            }
        }

        public static SelectStatement Create(SelectList selectList, FromClause from)
        {
            var qs = QuerySpecification.Create(selectList, from);
            var qe = QueryExpression.Create(qs);
            
            var ss = new SelectStatement();
            ss.Stack.AddLast(qe);

            return ss;
        }

        public IEnumerable<QuerySpecification> EnumerateQuerySpecifications()
        {
            return QueryExpression.EnumerateDescendants<QuerySpecification>();
        }

        public IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            foreach (var qs in EnumerateQuerySpecifications())
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
