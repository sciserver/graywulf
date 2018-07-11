using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class InsertStatement : ISourceTableCollection, ITargetTableProvider
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

        public ColumnList ColumnList
        {
            get
            {
                return FindDescendant<ColumnListBrackets>()?.FindDescendant<ColumnList>();
            }
        }

        public ValuesClause ValuesClause
        {
            get { return FindDescendant<ValuesClause>(); }
        }

        public bool IsDefaultValues
        {
            get
            {
                // TODO: handle special syntax of DEFAULT VALUES
                throw new NotImplementedException();
            }
        }

        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public OrderByClause OrderByClause
        {
            get { return FindDescendant<OrderByClause>(); }
        }

        public QueryHintClause QueryHintClause
        {
            get { return FindDescendant<QueryHintClause>(); }
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

            var old = (InsertStatement)other;

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
            // Target table
            yield return TargetTable;

            // Tables referenced in VALUES part subqueries
            var values = ValuesClause;
            if (values != null)
            {
                foreach (var group in values.EnumerateValueGroups())
                {
                    foreach (var value in group.EnumerateValues())
                    {
                        if (value is Expression exp)
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
            }

            // Tables referenced by the SELECT part and its subqueries
            var query = QueryExpression;
            if (query != null)
            {
                foreach (var ts in query.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }
        }
    }
}
