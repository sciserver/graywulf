using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.ServiceModel
{
    public class LimitedAccessServiceBehavior : IServiceBehavior
    {
        private string configSection;

        public string ConfigSection
        {
            get { return configSection; }
            set { configSection = value; }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var access = serviceHostBase.Extensions.Find<LimitedAccessServiceExtension>();

            if (access == null)
            {
                access = new LimitedAccessServiceExtension();
                access.Init(configSection);
                serviceHostBase.Extensions.Add(access);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
