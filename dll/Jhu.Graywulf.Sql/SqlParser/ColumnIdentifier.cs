using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
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
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value; }
        }

        protected override void InitializeMembers()
        {
            base.InitializeMembers();

            this.columnReference = null;
        }

        protected override void CopyMembers(object other)
        {
            base.CopyMembers(other);

            var old = (ColumnIdentifier)other;

            this.columnReference = old.columnReference;
        }

        public static ColumnIdentifier Create(ColumnReference cr)
        {
            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;

            if (!cr.TableReference.IsUndefined)
            {
                if (String.IsNullOrEmpty(cr.TableReference.Alias))
                {
                    nci.Stack.AddLast(TableName.Create(cr.TableReference.DatabaseObjectName));
                    nci.Stack.AddLast(Dot.Create());
                }
                else
                {
                    nci.Stack.AddLast(TableName.Create(cr.TableReference.Alias));
                    nci.Stack.AddLast(Dot.Create());
                }
            }

            nci.Stack.AddLast(ColumnName.Create(cr.ColumnName));

            return nci;
        }

        public override Node Interpret()
        {
            this.columnReference = ColumnReference.Interpret(this);

            return base.Interpret();
        }
    }
}
