using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtStaticMethodCall : IDataTypeReference, IMethodReference
    {
        private MethodReference methodReference;

        public DataTypeIdentifier DataTypeIdentifier
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }

        public MethodName MethodName
        {
            get { return FindDescendant<MethodName>(); }
        }

        public DataTypeReference DataTypeReference
        {
            get { return DataTypeIdentifier.DataTypeReference; }
            set { DataTypeIdentifier.DataTypeReference = value; }
        }
        
        public MethodReference MethodReference
        {
            get { return methodReference; }
            set { methodReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.methodReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (UdtStaticMethodCall)other;
            this.methodReference = old.methodReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            methodReference = MethodReference.Interpret(this);
        }
    }
}
