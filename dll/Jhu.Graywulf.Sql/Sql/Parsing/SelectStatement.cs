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
    public partial class SelectStatement : IStatement, ISelect, IOutputTableProvider
    {
        #region Private member variables

        private TableReference outputTableReference;

        #endregion
        #region Properties

        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Query; }
        }

        public CommonTableExpression CommonTableExpression
        {
            get { return FindDescendant<CommonTableExpression>(); }
        }

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

            this.outputTableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (SelectStatement)other;

            this.outputTableReference = new TableReference(old.outputTableReference);
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

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
