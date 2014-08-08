using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Web.Services
{
    public class RestClientSession : IDisposable
    {
        private RestClientBehavior restClientBehavior;

        public CookieContainer Cookies
        {
            get { return restClientBehavior.Cookies; }
        }

        public NameValueCollection Headers
        {
            get { return restClientBehavior.Headers; }
        }

        public HashSet<string> AcceptedHeaders
        {
            get { return restClientBehavior.AcceptedHeaders; }
        }

        public RestClientSession()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.restClientBehavior = new RestClientBehavior();
        }

        public void Dispose()
        {
        }

        public T CreateClient<T>(Uri uri)
        {
            return CreateChannel<T>(CreateWebHttpBinding(), CreateEndpointAddress(uri));
        }

        private T CreateChannel<T>(WebHttpBinding webHttpBinding, EndpointAddress endpoint)
        {
            var cf = new ChannelFactory<T>(webHttpBinding, endpoint);
            cf.Endpoint.Behaviors.Add(restClientBehavior);
            return cf.CreateChannel();
        }

        private WebHttpBinding CreateWebHttpBinding()
        {
            var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);

            webHttpBinding.ReaderQuotas.MaxArrayLength = 0x7FFFFFFF;
            webHttpBinding.ReaderQuotas.MaxDepth = 0x7FFFFFFF;
            webHttpBinding.ReaderQuotas.MaxStringContentLength = 0x7FFFFFFF;

            webHttpBinding.AllowCookies = false;
            webHttpBinding.TransferMode = TransferMode.Streamed;

            return webHttpBinding;
        }

        private EndpointAddress CreateEndpointAddress(Uri uri)
        {
            return new EndpointAddress(uri);
        }
    }
}
