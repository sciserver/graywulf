using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Comma
    {
        public static Comma Create()
        {
            var c = new Comma();
            c.Value = ",";

            return c;
        }
    }
}
