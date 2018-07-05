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

        public static ColumnIdentifier Create(ColumnReference cr)
        {
            if (cr.IsStar)
            {
                throw new InvalidOperationException();
            }

            MultiPartIdentifier mpi;

            if (cr.ParentTableReference != null && !cr.ParentTableReference.IsUndefined)
            {
                if (String.IsNullOrEmpty(cr.ParentTableReference.Alias))
                {
                    // TODO: maybe add schema too?
                    mpi = MultiPartIdentifier.Create(cr.ParentTableReference.DatabaseObjectName, cr.ColumnName);
                }
                else
                {
                    mpi = MultiPartIdentifier.Create(cr.ParentTableReference.Alias, cr.ColumnName);
                }
            }
            else
            {
                mpi = MultiPartIdentifier.Create(cr.ColumnName);
            }

            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;
            nci.Stack.AddLast(mpi);

            return nci;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
