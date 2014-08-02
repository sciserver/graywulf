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
            var ram = new RestAuthenticationModule();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                ram.Init(context.Domain);
            }

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

                // We use an endpoint behavior to authenticate REST requests.
                // Normaly an authentication manager would be used but this
                // approach is more similar to web page authentication and
                // almost the same code can be reused.
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
