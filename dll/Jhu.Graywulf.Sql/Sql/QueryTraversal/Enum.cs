using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    public enum ExpressionTraversalMethod
    {
        Infix,
        Prefix,
        Postfix,
    }

    public enum TraversalDirection
    {
        Forward,
        Backward
    }
}
