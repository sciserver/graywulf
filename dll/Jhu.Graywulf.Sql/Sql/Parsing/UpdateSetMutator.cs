using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateSetMutator : IColumnReference, IMethodReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public MethodReference MethodReference
        {
            get { return MethodCall.MethodReference; }
            set { MethodCall.MethodReference = value; }
        }

        public ColumnName ColumnName
        {
            get { return FindDescendant<ColumnName>(); }
        }

        public UdtMethodCall MethodCall
        {
            get { return FindDescendant<UdtMethodCall>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (UpdateSetMutator)other;
            this.columnReference = old.columnReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
