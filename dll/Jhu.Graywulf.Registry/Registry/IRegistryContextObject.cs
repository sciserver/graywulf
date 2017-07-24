using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public interface IRegistryContextObject
    {
        RegistryContext RegistryContext { get; set; }
    }
}
