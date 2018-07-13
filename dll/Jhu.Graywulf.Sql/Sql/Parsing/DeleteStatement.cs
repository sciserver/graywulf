using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DeleteStatement : ISourceTableCollection, ISourceTableConsumer, ITargetTableProvider
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference targetTableReference;

        #endregion

        public CommonTableExpression CommonTableExpression
        {
            get { return FindDescendant<CommonTableExpression>(); }
        }

        public TargetTableSpecification TargetTable
        {
            get { return FindDescendant<TargetTableSpecification>(); }
        }

        public FromClause FromClause
        {
            get { return FindDescendant<FromClause>(); }
        }

        public WhereClause WhereClause
        {
            get { return FindDescendant<WhereClause>(); }
        }

        public Dictionary<string, TableReference> ResolvedSourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        public TableReference TargetTableReference
        {
            get { return targetTableReference; }
            set { targetTableReference = value; }
        }

        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
            this.targetTableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (DeleteStatement)other;

            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
            this.targetTableReference = new TableReference(old.targetTableReference);
        }

        #endregion

        public IEnumerable<TableSource> EnumerateSourceTables(bool recursive)
        {
            yield return TargetTable;

            // Start from the FROM clause, if specified, otherwise no
            // table sources in the query
            var from = FromClause;
            var where = WhereClause;

            if (from != null)
            {
                foreach (var ts in from.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }

            if (where != null)
            {
                foreach (var ts in where.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }
        }
    }
}
