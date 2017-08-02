using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.ServiceModel
{
    public interface ITcpEndpointConfiguration
    {
        TcpEndpointConfiguration Endpoint { get; set;  }
    }
}
