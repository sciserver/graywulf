using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtMethodCall : IMethodReference
    {
        private MethodReference methodReference;
                
        public MethodName MethodName
        {
            get { return FindDescendant<MethodName>(); }
        }
                
        public MethodReference MethodReference
        {
            get { return methodReference; }
            set { methodReference = value; }
        }

        public override void Interpret()
        {
            base.Interpret();

            methodReference = MethodReference.Interpret(this);
        }

        public static UdtMethodCall Create(string variableName, string methodName)
        {
            throw new NotImplementedException();

            /* TODO: review

            var uv = UserVariable.Create(variableName);
            var fn = MethodName.Create(methodName);
            var mi = new UdtVariableMethodIdentifier();
            mi.Stack.AddLast(uv);
            mi.Stack.AddLast(Dot.Create());
            mi.Stack.AddLast(mi);
            return mi;

    */
        }

    }
}
