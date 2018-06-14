﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class InsertStatement : IStatement, ITableSourceCollector, ITableSourceProvider
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;

        #endregion

        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Modify; }
        }

        public Dictionary<string, TableReference> SourceTableReferences
        {
            get { return sourceTableReferences; }
        }

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
            get
            {
                return FindDescendant<QueryExpression>();
            }
        }

        public OrderByClause OrderByClause
        {
            get
            {
                return FindDescendant<OrderByClause>();
            }
        }

        public QueryHintClause QueryHintClause
        {
            get
            {
                return FindDescendant<QueryHintClause>();
            }
        }

        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (InsertStatement)other;

            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
        }

        #endregion

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }

        public IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            yield return TargetTable;

            // TODO: SELECT part and VALUES part expression subqueries
            throw new NotImplementedException();
        }

        public IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive)
        {
            return EnumerateSourceTables(recursive).Select(ts => ts.TableReference);
        }
    }
}
