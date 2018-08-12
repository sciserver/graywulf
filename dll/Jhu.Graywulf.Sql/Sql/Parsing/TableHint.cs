using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableHint
    {
        public HintName HintName
        {
            get { return FindDescendant<HintName>(); }
        }

        public Expression[] GetArguments()
        {
            return FindDescendant<HintArguments>()?
                .FindDescendant<HintArgumentList>()?
                .EnumerateDescendants<HintArgument>()
                       .Select(a => (Expression)a.Expression)
                       .ToArray();
        }
    }
}
