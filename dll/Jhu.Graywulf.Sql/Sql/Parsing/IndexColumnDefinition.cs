using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IndexColumnDefinition : IColumnReference, ITableReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return columnReference.ParentTableReference; }
        }

        public TableReference TableReference
        {
            get { return columnReference.ParentTableReference; }
            set { columnReference.ParentTableReference = value; }
        }

        public ColumnName ColumnName
        {
            get { return FindDescendant<ColumnName>(); }
        }

        public bool IsAscending
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDescending
        {
            get { throw new NotImplementedException(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (IndexColumnDefinition)other;

            this.columnReference = old.columnReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
