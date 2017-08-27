using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public interface ITableReference
    {
        TableReference TableReference { get; set; }
    }
}
