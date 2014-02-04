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
        /// <summary>
        /// Sets up default binding parameters.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // (2.)

            // Find all web http endpoint behaviors and
            // turn on help
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

            var rob = new RestOperationBehavior();
            var reh = new RestErrorHandler();
            var ram = new Security.RestAuthenticationModule();

            // Automatically add custom operation behavior to all operations
            foreach (var ep in serviceDescription.Endpoints)
            {
                foreach (var op in ep.Contract.Operations)
                {
                    op.Behaviors.Add(rob);
                }
            }

            // Remove any error handlers and replace
            // them with own implementation to catch
            // exceptions and turn them into simple
            // error messages that aren't wrapped into xml
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                cd.IncludeExceptionDetailInFaults = true;
                if (cd.ErrorHandlers.Count > 0)
                {
                    // Remove the System.ServiceModel.Web errorHandler
                    cd.ErrorHandlers.Remove(cd.ErrorHandlers[0]);
                }

                // Add new custom error handler
                cd.ErrorHandlers.Add(reh);


                foreach (EndpointDispatcher ep in cd.Endpoints)
                {
                    ep.DispatchRuntime.MessageInspectors.Add(ram);
                    foreach (DispatchOperation op in ep.DispatchRuntime.Operations)
                    {
                        op.ParameterInspectors.Add(ram);
                    }
                }
            }
        }


        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // (1.)
        }
    }
}
