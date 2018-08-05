using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class JoinCondition
    {
        public static JoinCondition Create(LogicalExpression joinCondition)
        {
            var jc = new JoinCondition();

            jc.Stack.AddLast(Keyword.Create("ON"));
            jc.Stack.AddLast(Whitespace.Create());
            jc.Stack.AddLast(joinCondition);

            return jc;
        }
    }
}
