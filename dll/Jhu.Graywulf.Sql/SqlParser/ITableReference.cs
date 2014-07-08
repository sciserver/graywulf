using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public interface ITableReference
    {
        TableReference TableReference { get; set; }
    }
}
