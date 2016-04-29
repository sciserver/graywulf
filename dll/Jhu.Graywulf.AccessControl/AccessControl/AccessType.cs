using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.AccessControl
{
    public enum AccessType : byte
    {
        Undefined = 0,
        Grant = 1,
        Deny = 2
    }
}
