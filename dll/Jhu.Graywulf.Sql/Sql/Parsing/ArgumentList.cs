using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ArgumentList
    {
        public static ArgumentList Create(Argument argument)
        {
            var arglist = new ArgumentList();

            arglist.Stack.AddLast(argument);

            return arglist;
        }

        public static ArgumentList Create(Expression expression)
        {
            var argument = Argument.Create(expression);
            return Create(argument);
        }

        public static ArgumentList Create(params Argument[] arguments)
        {
            ArgumentList arglist = null;
            ArgumentList p = null;

            for (int i = 0; i < arguments.Length; i++)
            {
                var n = ArgumentList.Create(arguments[i]);

                if (p != null)
                {
                    p.Stack.AddLast(Comma.Create());
                    p.Stack.AddLast(Whitespace.Create());
                    p.Stack.AddLast(n);
                }

                if (arglist == null)
                {
                    arglist = n;
                }

                p = n;
            }

            return arglist;
        }

        public static ArgumentList Create(params Expression[] expressions)
        {
            var arguments = new Argument[expressions.Length];

            for (int i = 0; i < expressions.Length; i++)
            {
                arguments[i] = Argument.Create(expressions[i]);
            }

            return Create(arguments);
        }

        
    }
}
