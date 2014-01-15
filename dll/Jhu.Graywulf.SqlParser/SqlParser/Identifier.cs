using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Identifier
    {
        public static Identifier Create(string value)
        {
            var res = new Identifier();
            res.Value = value;
            return res;
        }
    }
}
