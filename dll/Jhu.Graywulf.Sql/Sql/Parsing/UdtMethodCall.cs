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
        
        public static UdtMethodCall Create(MethodReference mr, Expression[] args)
        {
            var mc = new UdtMethodCall()
            {
                methodReference = mr
            };
            mc.Stack.AddLast(Dot.Create());
            mc.Stack.AddLast(MethodName.Create(mr.MethodName));
            mc.Stack.AddLast(FunctionArguments.Create(args));

            return mc;
        }

        public override void Interpret()
        {
            base.Interpret();

            methodReference = MethodReference.Interpret(this);
        }
    }
}
