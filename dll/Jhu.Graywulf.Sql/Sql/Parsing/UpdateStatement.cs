using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateStatement : ISourceTableCollection, ISourceTableConsumer, ITargetTableProvider
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

        public UpdateSetList UpdateSetList
        {
            get { return FindDescendant<UpdateSetList>(); }
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

            var old = (UpdateStatement)other;

            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
            this.targetTableReference = new TableReference(old.targetTableReference);
        }

        #endregion

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }

        public IEnumerable<TableSource> EnumerateSourceTables(bool recursive)
        {
            yield return TargetTable;

            // Tables referenced in SET part subqueries
            var sets = UpdateSetList;
            if (sets != null)
            {
                foreach (var set in sets.EnumerateSetColumns())
                {
                    var exp = set.RightHandSide?.Expression;
                    if (exp != null)
                    {
                        foreach (var sq in exp.EnumerateDescendantsRecursive<Subquery>())
                        {
                            foreach (var ts in sq.EnumerateSourceTables(recursive))
                            {
                                yield return ts;
                            }
                        }
                    }
                }
            }

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
