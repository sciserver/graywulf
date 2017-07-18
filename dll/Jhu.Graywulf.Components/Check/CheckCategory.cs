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
        File = 0x08,
        Database = 0x10,
        Email = 0x20,
        Registry = 0x40,
        Url = 0x80,
        Service = 0x100,

        Quick = Iis,
        All = 0xFFFF,

        NoDb = All & ~Database,
        NoEmail = NoDb & ~Email,
    }
}
