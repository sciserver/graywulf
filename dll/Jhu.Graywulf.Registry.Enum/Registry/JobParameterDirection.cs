using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    [Flags]
    public enum JobParameterDirection : byte
    {
        Unknown = 0,
        In = 1,
        Out = 2,
        InOut = In | Out
    }
}
