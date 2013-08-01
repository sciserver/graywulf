using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Jobs.Query
{
    [Flags]
    public enum ResultsetTarget
    {
        TemporaryTable = 1,
        DestinationTable = 2
    }

    public enum ExecutionMode
    {
        SingleServer,
        Graywulf
    }
}
