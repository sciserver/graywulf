using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FunctionName
    {
        public static Variable Create(string name)
        {
            var fun = new Variable();
            fun.Value = name;
            return fun;
        }
    }
}
