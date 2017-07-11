using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Activities
{
    interface IJobInfo
    {
        Guid ClusterGuid { get; set; }
        Guid DomainGuid { get; set; }
        Guid FederationGuid { get; set; }
        Guid JobGuid { get; set; }
        Guid UserGuid { get; set; }
        string UserName { get; set; }
        string JobID { get; set; }
    }
}
