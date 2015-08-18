using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.SqlClrUtil
{
    enum SqlObjectRank : int
    {
        UserDefinedType = 0,
        Function = 1,
        Aggregate = 2
    }
}
