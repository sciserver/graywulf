using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.Tasks
{
    [Flags]
    public enum DestinationTableOperation
    {
        Drop = 1,
        Create = 2,
        Clear = 4,
        Append = 8
    }
}
