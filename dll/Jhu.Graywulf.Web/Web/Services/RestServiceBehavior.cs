using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Services
{
    public class RestServiceBehavior : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // (1.)
        }

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

            // Enable debugging
#if DEBUG
            var sdb = serviceDescription.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                serviceDescription.Behaviors.Add(sdb);
            }

            sdb.IncludeExceptionDetailInFaults = true;
#endif

            // Find all web http endpoint behaviors and
            // turn on help
            foreach (var ep in endpoints)
            {
                var whep = (WebHttpEndpoint)ep;
                whep.AutomaticFormatSelectionEnabled = true;

                if (ep.Binding is WebHttpBinding)
                {
                    var whbind = (WebHttpBinding)ep.Binding;
                    whbind.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    whbind.TransferMode = TransferMode.Streamed;
                    whbind.MaxReceivedMessageSize = 0x40000000;     // 1 GB
                }
                else if (ep.Binding is CustomBinding)
                {
                    var cubind = (CustomBinding)ep.Binding;
                    var htbe = cubind.Elements.Find<HttpTransportBindingElement>();
                    htbe.TransferMode = TransferMode.Streamed;
                    htbe.MaxReceivedMessageSize = 0x40000000;     // 1 GB
                }

                // Exchange WHB with custom implementation
                if (ep.EndpointBehaviors.Contains(typeof(WebHttpBehavior)))
                {
                    var whb = ep.EndpointBehaviors[typeof(WebHttpBehavior)];

                    var reb = new RestEndpointBehavior();
                    reb.HelpEnabled = true;
                    reb.AutomaticFormatSelectionEnabled = true;

                    ep.EndpointBehaviors.Remove(whb);
                    ep.Behaviors.Add(reb);
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // (3.)
            using (new RestLoggingContext())
            {
                const string OptionsMethodName = "HandleHttpOptionsRequest";

                var rob = new RestOperationBehavior();
                var reh = new RestErrorHandler();
                var ram = new RestAuthenticationModule();
                var romi = new RestCorsMessageInspector();

                using (var context = ContextManager.Instance.CreateReadOnlyContext())
                {
                    ram.Init(context.Domain);
                }

                // Automatically add custom operation behavior to all operations except the one
                // that handles the OPTIONS verb
                foreach (var ep in serviceDescription.Endpoints)
                {
                    foreach (var op in ep.Contract.Operations)
                    {
                        if (StringComparer.InvariantCultureIgnoreCase.Compare(op.Name, OptionsMethodName) != 0)
                        {
                            op.Behaviors.Add(rob);
                        }
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

                    // We use an endpoint behavior to authenticate REST requests.
                    // Normaly an authentication manager would be used but this
                    // approach is more similar to web page authentication and
                    // almost the same code can be reused.

                    // At the same time, we add a RestCorsMessageInspector to the
                    // operations which will be responsible for setting the right
                    // http response headers for cross-domain client-side scripting.

                    foreach (EndpointDispatcher ep in cd.Endpoints)
                    {
                        ep.DispatchRuntime.MessageInspectors.Add(ram);
                        ep.DispatchRuntime.MessageInspectors.Add(romi);

                        foreach (DispatchOperation op in ep.DispatchRuntime.Operations)
                        {
                            op.ParameterInspectors.Add(ram);
                        }
                    }
                }
            }
        }
    }
}
