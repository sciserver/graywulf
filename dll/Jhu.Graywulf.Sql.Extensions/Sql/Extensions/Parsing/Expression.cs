using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Extensions.Parsing
{
    public partial class Expression
    {
        public static new Expression Create(Sql.Parsing.ColumnIdentifier ci)
        {
            var nex = new Expression();
            nex.Stack.AddLast(ci);

            return nex;
        }
    }
}
