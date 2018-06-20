using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnDefinition : IColumnReference, ITableReference, IDataTypeReference
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

        public TableReference TableReference
        {
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return DataType.DataTypeReference; }
            set { DataType.DataTypeReference = value; }
        }

        public ColumnName ColumnName
        {
            get { return FindDescendant<ColumnName>(); }
        }

        public DataTypeIdentifier DataType
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }

        public ColumnDefaultDefinition DefaultDefinition
        {
            get { return FindDescendant<ColumnDefaultDefinition>(); }
        }

        public ColumnIdentityDefinition IdentityDefinition
        {
            get { return FindDescendant<ColumnIdentityDefinition>(); }
        }

        public ColumnConstraint Constraint
        {
            get { return FindDescendant<ColumnConstraint>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (ColumnDefinition)other;

            this.columnReference = old.columnReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
