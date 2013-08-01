using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public interface IGraywulfActivity
    {
        InArgument<Guid> JobGuid { get; set; }
        InArgument<Guid> UserGuid { get; set; }
    }
}
