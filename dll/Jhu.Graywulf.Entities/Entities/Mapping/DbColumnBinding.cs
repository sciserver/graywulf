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
        Identity = 2,
        Column = 4,
        ReadOnlyColumn = 8,

        Any = 0xFFFF
    }
}
