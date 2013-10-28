using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public interface IFunctionReference
    {
        FunctionReference FunctionReference { get; set; }
    }
}
