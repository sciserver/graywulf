using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionArguments
    {
        public static FunctionArguments Create(params Expression[] expressions)
        {
            var args = new FunctionArguments();

            args.Stack.AddLast(BracketOpen.Create());

            if (expressions != null)
            {
                var arglist = ArgumentList.Create(expressions);
                args.Stack.AddLast(arglist);
            }

            args.Stack.AddLast(BracketClose.Create());

            return args;
        }
    }
}
