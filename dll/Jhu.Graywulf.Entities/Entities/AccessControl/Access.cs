using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public class Access : Dictionary<string, AccessType>
    {
        public Access()
            : base(EntityAcl.Comparer)
        {
        }
    }
}
