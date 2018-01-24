using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public enum ExecutionMode
    {
        SingleServer,
        Graywulf
    }

    public enum CommandTarget
    {
        Code,
        Temp
    }
}
