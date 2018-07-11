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
    public partial class SelectStatement : ISelect, ISourceTableCollection, ISourceTableConsumer, IOutputTableProvider
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference outputTableReference;

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
                var ts = qs.EnumerateSourceTables(false).FirstOrDefault();

                if (ts == null || !(ts is SimpleTableSource))
                {
                    // It might be a constant query (SELECT 1), that's definitely not partitioned
                    return false;
                }

                return ((SimpleTableSource)ts).IsPartitioned;
            }
        }

        public Dictionary<string, TableReference> ResolvedSourceTableReferences
        {
            get { return sourceTableReferences; }
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

            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
            this.outputTableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (SelectStatement)other;

            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
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

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }

        public IEnumerable<TableSource> EnumerateSourceTables(bool recursive)
        {
            throw new NotImplementedException();
        }
    }
}
