using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Api
{
    public class RestServiceBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // (2.)
            foreach (var ep in endpoints)
            {
                var whb = ep.Behaviors.Find<WebHttpBehavior>();

                if (whb != null)
                {
                    whb.HelpEnabled = true;
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // (3.)

            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                cd.IncludeExceptionDetailInFaults = true;
                if (cd.ErrorHandlers.Count > 0)
                {
                    // Remove the System.ServiceModel.Web errorHandler
                    cd.ErrorHandlers.Remove(cd.ErrorHandlers[0]);
                }

                // Add new custom error handler
                cd.ErrorHandlers.Add(new RestErrorHandler());
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // (1.)
        }
    }
}
