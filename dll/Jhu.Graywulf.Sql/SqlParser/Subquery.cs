using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Subquery
    {
        public SelectStatement SelectStatement
        {
            get { return FindDescendant<SelectStatement>(); }
        }

        public static Subquery Create(SelectStatement ss)
        {
            var sq = new Subquery();

            sq.Stack.AddLast(BracketOpen.Create());
            sq.Stack.AddLast(ss);
            sq.Stack.AddLast(BracketClose.Create());

            return sq;
        }
    }
}
