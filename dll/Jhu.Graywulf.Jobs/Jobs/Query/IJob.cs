using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    public interface IJob
    {
        InArgument<Guid> UserGuid { get; set; }
        InArgument<Guid> JobGuid { get; set; }
    }
}
