using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Check
{
    [Flags]
    public enum CheckCategory : int
    {
        Iis = 0x01,
        Assembly = 0x02,
        Plugin = 0x04,
        Database = 0x08,
        Email = 0x10,
        Registry = 0x20,
        Url = 0x40,
        Service = 0x80,

        Quick = Iis,
        All = 0xFFFF,

        NoDb = All & ~Database,
        NoEmail = NoDb & ~Email,
    }
}
