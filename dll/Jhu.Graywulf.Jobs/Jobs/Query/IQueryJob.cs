using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.Query
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This interface is required by the WebUI which can list all queries in a
    /// unified view
    /// </remarks>
    public interface IQueryJob
    {
        InArgument<QueryBase> Query { get; set; }
        InArgument<Guid> UserGuid { get; set; }
        InArgument<Guid> JobGuid { get; set; }
    }
}
