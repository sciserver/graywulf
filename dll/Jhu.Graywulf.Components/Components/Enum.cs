using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    [Flags]
    public enum ParameterDirection : byte
    {
        Unknown = 0,
        In = 1,
        Out = 2,
        InOut = In | Out
    }
}
