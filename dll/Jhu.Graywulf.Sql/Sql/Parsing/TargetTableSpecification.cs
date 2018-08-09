using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TargetTableSpecification : ITableReference
    {
        private TableReference tableReference;
        private string uniqueKey;

        public TableOrViewIdentifier TableOrViewIdentifier
        {
            get { return FindDescendantRecursive<TableOrViewIdentifier>(); }
        }

        public UserVariable UserVariable
        {
            get { return FindDescendantRecursive<UserVariable>(); }
        }

        public override bool IsSubquery
        {
            get { return false; }
        }

        public override bool IsMultiTable
        {
            get { return false; }
        }

        public override string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public override TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (TargetTableSpecification)other;

            this.tableReference = old.tableReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableReference = TableReference.Interpret(this);
        }
    }
}
