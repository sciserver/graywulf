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
        Flow = 1,
        Declaration = 2,
        Command = 3,
        Query = 8,
        Modify = 16,
        Schema = 32,


        Magic = 0x7000
    }
}
