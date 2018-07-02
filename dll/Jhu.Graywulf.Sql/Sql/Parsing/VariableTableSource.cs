using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class VariableTableSource : ITableSource
    {
        private TableReference tableReference;
        private string uniqueKey;
        
        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return tableReference; }
        }

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public TableAlias Alias
        {
            get { return FindDescendant<TableAlias>(); }
        }

        public UserVariable Variable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
        {
            get { return false; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (VariableTableSource)other;

            this.tableReference = old.tableReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableReference = TableReference.Interpret(this);
        }

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
