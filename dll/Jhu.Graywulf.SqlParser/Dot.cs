using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Dot
    {
        public static Dot Create()
        {
            var d = new Dot();
            d.Value = ".";

            return d;
        }
    }
}
