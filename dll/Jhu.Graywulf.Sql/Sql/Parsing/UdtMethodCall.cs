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
        public MethodName MethodName
        {
            get { return FindDescendant<MethodName>(); }
        }

        public MethodReference MethodReference
        {
            get { return MethodName.MethodReference; }
            set { MethodName.MethodReference = value; }
        }
        
        public static UdtMethodCall Create(MethodReference mr, Expression[] arguments)
        {
            var mc = new UdtMethodCall();

            mc.Stack.AddLast(MethodName.Create(mr));
            mc.AppendArguments(arguments);

            return mc;
        }
    }
}
