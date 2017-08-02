using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    public interface IRegistryObject
    {
        Guid Guid { get; }
        string Name { get; }
        bool IsRunning { get; }
    }
}
