using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    class DBColumnAttribute : Attribute
    {
        public int Size { get; set; }
    }
}
