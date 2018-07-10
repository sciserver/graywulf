using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnName : IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public static ColumnName Create(string columnName)
        {
            var ntn = new ColumnName();
            ntn.Stack.AddLast(Identifier.Create(columnName));

            return ntn;
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (ColumnName)other;
            this.columnReference = old.columnReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
