using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FunctionName
    {
        public static FunctionName Create(string name)
        {
            var fun = new FunctionName();
            fun.Value = name;
            return fun;
        }
    }
}
