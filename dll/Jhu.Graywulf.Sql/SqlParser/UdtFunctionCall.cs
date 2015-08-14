using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class UdtFunctionCall
    {
        public static UdtFunctionCall Create(string variableName, string functionName, params Expression[] arguments)
        {
            var udtf = new UdtFunctionCall();
            var fun = UdtFunctionIdentifier.Create(variableName, functionName);
            var args = FunctionArguments.Create(arguments);

            udtf.Stack.AddLast(fun);
            udtf.Stack.AddLast(args);

            return udtf;
        }
    }
}
