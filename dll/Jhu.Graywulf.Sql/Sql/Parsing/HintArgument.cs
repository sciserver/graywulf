using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class HintArgument
    {
        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

    }
}
