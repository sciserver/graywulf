using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtStaticMethodIdentifier : IDataTypeReference, IMethodReference
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

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return methodReference; }
        }

        public FunctionReference FunctionReference
        {
            get { return methodReference; }
            set { methodReference = (MethodReference)value; }
        }

        public MethodReference MethodReference
        {
            get { return methodReference; }
            set { methodReference = value; }
        }

        // TODO: implement create, if necessary, to access UDT static methods in
        // the form dbo.Udt::Method()

        public override void Interpret()
        {
            base.Interpret();

            throw new NotImplementedException();
        }
    }
}
