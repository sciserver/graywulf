using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class RestBehaviorBase : Attribute
    {
        protected void ConfigureEndpoint(ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is WebHttpBinding)
            {
                var whbind = (WebHttpBinding)endpoint.Binding;
                whbind.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                whbind.TransferMode = TransferMode.Streamed;
                whbind.MaxReceivedMessageSize = 0x40000000;     // 1 GB
                whbind.ContentTypeMapper = new Serialization.RestContentTypeMapper();
                // whbind.ReaderQuotas
            }
            else if (endpoint.Binding is CustomBinding)
            {
                var cubind = (CustomBinding)endpoint.Binding;
                var htbe = cubind.Elements.Find<HttpTransportBindingElement>();
                htbe.TransferMode = TransferMode.Streamed;
                htbe.MaxReceivedMessageSize = 0x40000000;     // 1 GB
            }

            // Exchange WHB with custom implementation
            if (endpoint.EndpointBehaviors.Contains(typeof(WebHttpBehavior)))
            {
                var whb = endpoint.EndpointBehaviors[typeof(WebHttpBehavior)];

                var reb = new RestEndpointBehavior();
                reb.HelpEnabled = true;
                reb.AutomaticFormatSelectionEnabled = false;
                reb.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;

                endpoint.EndpointBehaviors.Remove(whb);
                endpoint.Behaviors.Add(reb);
            }
        }
    }
}
