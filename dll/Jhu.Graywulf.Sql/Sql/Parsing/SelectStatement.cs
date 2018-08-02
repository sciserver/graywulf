using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;


namespace Jhu.Graywulf.Sql.Parsing
{
    /// <summary>
    /// Implements a SELECT statement including the ORDER BY clause
    /// </summary>
    public partial class SelectStatement : ISourceTableProvider, IOutputTableProvider
    {
        #region Private member variables

        #endregion
        #region Properties

        public CommonTableExpression CommonTableExpression
        {
            get { return FindDescendant<CommonTableExpression>(); }
        }

        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public IntoClause IntoClause
        {
            get { return FindDescendantRecursive<IntoClause>(); }
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
                var ts = qs.FirstTableSource;

                if (ts == null || !(ts is SimpleTableSource))
                {
                    // It might be a constant query (SELECT 1), that's definitely not partitioned
                    return false;
                }

                return ((SimpleTableSource)ts).IsPartitioned;
            }
        }

        /// <summary>
        /// Gets the reference to the table that represents
        /// the resultset of the query
        /// </summary>
        public TableReference OutputTableReference
        {
            get
            {
                return QueryExpression?.FirstQuerySpecification?.IntoClause?.TargetTable?.TableReference;
            }
        }

        #endregion
        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (SelectStatement)other;
        }

        #endregion

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

        public IEnumerable<TableSource> EnumerateSourceTables(bool recursive)
        {
            throw new NotImplementedException();
        }
    }
}
