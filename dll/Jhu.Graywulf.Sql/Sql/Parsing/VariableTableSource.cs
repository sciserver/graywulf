using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class VariableTableSource
    {
        private TableReference tableReference;
        private string uniqueKey;
        
        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return tableReference; }
        }

        public override TableReference TableReference
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

        public override string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public override bool IsSubquery
        {
            get { return false; }
        }

        public override bool IsMultiTable
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

        public override IEnumerable<TableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
