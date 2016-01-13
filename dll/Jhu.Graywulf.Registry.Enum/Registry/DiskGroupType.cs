using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    [Flags]
    public enum DiskGroupType : int
    {
        Unknown = 0x0000,
        System = 0x0001,
        Temp = 0x0002,
        Data = 0x0004,
        Log = 0x0008
    }
}
