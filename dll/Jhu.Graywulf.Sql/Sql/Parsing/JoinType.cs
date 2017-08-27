using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class JoinType
    {
        public static JoinType CreateInner()
        {
            var jt = new JoinType();
            jt.Stack.AddLast(Keyword.Create("INNER"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("JOIN"));

            return jt;
        }

        public static JoinType CreateInnerLoop()
        {
            var jt = new JoinType();
            jt.Stack.AddLast(Keyword.Create("INNER"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("LOOP"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("JOIN"));

            return jt;
        }
    }
}
