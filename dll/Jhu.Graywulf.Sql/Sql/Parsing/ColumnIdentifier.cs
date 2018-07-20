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
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value;  }
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
            // TODO: this is fine but expressions no longer reference
            // columns with IdentifierPartChain

            throw new NotImplementedException();

            /*
            if (cr.IsStar)
            {
                throw new InvalidOperationException();
            }

            IdentifierChain mpi;

            if (cr.TableReference != null && !cr.TableReference.IsUndefined)
            {
                if (String.IsNullOrEmpty(cr.TableReference.Alias))
                {
                    // TODO: maybe add schema too?
                    mpi = IdentifierChain.Create(cr.TableReference.DatabaseObjectName, cr.ColumnName);
                }
                else
                {
                    mpi = IdentifierChain.Create(cr.TableReference.Alias, cr.ColumnName);
                }
            }
            else
            {
                mpi = IdentifierChain.Create(cr.ColumnName);
            }

            var nci = new ColumnIdentifier();
            nci.ColumnReference = cr;
            nci.Stack.AddLast(mpi);

            return nci;
            */
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
