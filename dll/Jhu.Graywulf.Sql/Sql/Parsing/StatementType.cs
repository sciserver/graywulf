using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    [Flags]
    public enum StatementType : int
    {
        Unknown = 0,
        Block = 1,
        Flow = 2,
        Declaration = 4,
        Command = 8,
        Query = 16,
        Modify = 32,
        Schema = 64,


        Magic = 0x7000
    }
}
