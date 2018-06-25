using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnIdentifier : ITableReference, IColumnReference
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
            set { columnReference.ParentTableReference = value;  }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (ColumnIdentifier)other;

            this.columnReference = old.columnReference;
        }

        public static ColumnIdentifier CreateStar()
        {
            var ci = new ColumnIdentifier();

            ci.columnReference = ColumnReference.CreateStar();
            ci.Stack.AddLast(Mul.Create());

            return ci;
        }

        public static ColumnIdentifier CreateStar(TableReference tableReference)
        {
            var ci = new ColumnIdentifier();

            ci.columnReference = ColumnReference.CreateStar(tableReference);
            ci.Stack.AddLast(Mul.Create());

            return ci;
        }

        public static ColumnIdentifier Create(ColumnReference cr)
        {
            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;

            if (cr.ParentTableReference != null && !cr.ParentTableReference.IsUndefined)
            {
                if (String.IsNullOrEmpty(cr.ParentTableReference.Alias))
                {
                    nci.Stack.AddLast(TableName.Create(cr.ParentTableReference.DatabaseObjectName));
                    nci.Stack.AddLast(Dot.Create());
                }
                else
                {
                    nci.Stack.AddLast(TableName.Create(cr.ParentTableReference.Alias));
                    nci.Stack.AddLast(Dot.Create());
                }
            }

            nci.Stack.AddLast(ColumnName.Create(cr.ColumnName));

            return nci;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
