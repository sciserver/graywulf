using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Variable
    {
        public static Variable Create(string name)
        {
            var var = new Variable();
            var.Value = name;
            return var;
        }
    }
}
