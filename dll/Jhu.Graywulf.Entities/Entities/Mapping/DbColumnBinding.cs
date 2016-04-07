using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.Mapping
{
    [Flags]
    public enum DbColumnBinding : int
    {
        Key = 1,
        Column = 2,
        Auxiliary = 4,

        Any = 0xFFFF
    }
}
