using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class JoinOperator
    {
        public static InnerOuterJoinOperator CreateInner()
        {
            var jt = new InnerOuterJoinOperator();
            jt.Stack.AddLast(Keyword.Create("INNER"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("JOIN"));

            return jt;
        }

        public static InnerOuterJoinOperator CreateInnerLoop()
        {
            var jt = new InnerOuterJoinOperator();
            jt.Stack.AddLast(Keyword.Create("INNER"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("LOOP"));
            jt.Stack.AddLast(Whitespace.Create());
            jt.Stack.AddLast(Keyword.Create("JOIN"));

            return jt;
        }
    }
}
