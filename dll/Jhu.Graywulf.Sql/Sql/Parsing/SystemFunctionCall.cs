using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SystemFunctionCall : IFunctionReference
    {
        public FunctionName FunctionName
        {
            get { return FindDescendant<FunctionName>(); }
        }

        public FunctionReference FunctionReference
        {
            get { return FunctionName.FunctionReference; }
            set { FunctionName.FunctionReference = value; }
        }

        public static SystemFunctionCall Create(string functionName, params Expression[] arguments)
        {
            var fr = new FunctionReference()
            {
                FunctionName = functionName,
                IsSystem = true,
            };

            var f = new SystemFunctionCall();
            var fun = FunctionName.Create(fr);
            var args = FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            return f;
        }
    }
}
