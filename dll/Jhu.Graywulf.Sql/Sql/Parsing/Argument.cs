using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Argument
    {
        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

        public static Argument Create(Expression expression)
        {
            return new Argument(expression);
        }
    }
}
