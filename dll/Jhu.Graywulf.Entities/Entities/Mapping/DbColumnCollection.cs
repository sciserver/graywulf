using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.Mapping
{
    internal class DbColumnCollection : Dictionary<string, DbColumn>
    {
        public DbColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}
