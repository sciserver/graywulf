using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    public class RemoteServiceConfiguration : ConfigurationSection, ITcpEndpointConfiguration
    {
        [ConfigurationProperty("endpoint")]
        public TcpEndpointConfiguration Endpoint
        {
            get { return (TcpEndpointConfiguration)base["endpoint"]; }
            set { base["endpoint"] = value; }
        }
    }
}
