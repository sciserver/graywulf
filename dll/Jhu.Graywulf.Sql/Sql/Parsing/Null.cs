using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Null
    {
        public static Null Create()
        {
            var k = new Keyword("NULL");
            var n = new Null();
            n.Stack.AddLast(k);

            return n;
        }
    }
}
