using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Configuration;

namespace Jhu.Graywulf.Web.Services
{
    /// <summary>
    /// Implements an IDispatchMessageInspector to set CORS headers required by
    /// cross-domain client side ajax calls.
    /// </summary>
    public class RestCorsMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            // Here we only return null now as the correlation state
            // TODO: we could actually figure out what operation is called from the
            // query string and return the CORS headers accordingly.
            // In the current implementation we simply return whats configured
            // in the web.config
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // This is a REST service, get context

            var context = WebOperationContext.Current;
            var response = context.OutgoingResponse;

            var headers = ConfigurationManager.GetSection("jhu.graywulf/rest/customHeaders") as NameValueCollection;

            if (headers != null)
            {
                response.Headers.Add(headers);
            }
        }
    }
}
