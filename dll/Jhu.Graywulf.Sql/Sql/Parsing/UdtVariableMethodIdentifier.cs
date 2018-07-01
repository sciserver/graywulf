using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtVariableMethodIdentifier : IVariableReference, IMethodReference
    {
        private MethodReference methodReference;

        public UserVariable UserVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public MethodName MethodName
        {
            get { return FindDescendant<MethodName>(); }
        }

        public VariableReference VariableReference
        {
            get { return UserVariable.VariableReference; }
            set { UserVariable.VariableReference = value; }
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

        public override void Interpret()
        {
            base.Interpret();

            // TODO: method reference

            throw new NotImplementedException();
        }

        public static UdtVariableMethodIdentifier Create(string variableName, string methodName)
        {
            var uv = UserVariable.Create(variableName);
            var fn = MethodName.Create(methodName);
            var mi = new UdtVariableMethodIdentifier();
            mi.Stack.AddLast(uv);
            mi.Stack.AddLast(Dot.Create());
            mi.Stack.AddLast(mi);
            return mi;
        }

    }
}
