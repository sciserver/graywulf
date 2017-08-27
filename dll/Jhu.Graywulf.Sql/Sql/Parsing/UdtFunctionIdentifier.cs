using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtFunctionIdentifier
    {
        public static UdtFunctionIdentifier Create(string variableName, string functionName)
        {
            var udtf = new UdtFunctionIdentifier();
            var var = Variable.Create(variableName);
            var fun = FunctionName.Create(functionName);

            udtf.Stack.AddLast(var);
            udtf.Stack.AddLast(Dot.Create());
            udtf.Stack.AddLast(fun);

            return udtf;
        }
    }
}
