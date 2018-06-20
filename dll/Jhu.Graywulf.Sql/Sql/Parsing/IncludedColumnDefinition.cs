using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IncludedColumnDefinition : IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return columnReference.TableReference; }
        }

        public ColumnName ColumnName
        {
            get { return FindDescendant<ColumnName>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (IncludedColumnDefinition)other;

            this.columnReference = old.columnReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
