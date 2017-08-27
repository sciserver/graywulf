using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionName
    {
        public static FunctionName Create(string name)
        {
            var fun = new FunctionName();
            fun.Stack.AddLast(Identifier.Create(name));
            return fun;
        }
    }
}
