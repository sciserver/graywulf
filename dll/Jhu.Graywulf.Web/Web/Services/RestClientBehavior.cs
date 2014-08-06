using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace Jhu.Graywulf.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The behavior accepts all cookies and a limited set of headers and
    /// automatically forwards them to the next request.
    /// </remarks>
    public class RestClientBehavior : IEndpointBehavior, IClientMessageInspector
    {
        private HashSet<string> acceptedHeaders;
        private NameValueCollection headers;
        private CookieContainer cookies;

        public HashSet<string> AcceptedHeaders
        {
            get { return acceptedHeaders; }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public CookieContainer Cookies
        {
            get { return cookies; }
        }

        public RestClientBehavior()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.acceptedHeaders = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            this.headers = new NameValueCollection();
            this.cookies = new CookieContainer();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {   
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // Add REST support
            endpoint.Behaviors.Add(new WebHttpBehavior());

            // Add this class to inspectors
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {   
        }

        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            var property = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

            // Set headers
            foreach (string key in headers.Keys)
            {
                property.Headers.Add(key, headers[key]);
            }

            // Set cookies
            property.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(request.Headers.To));

            return request.Headers.To;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var uri = (Uri)correlationState;
            var property = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];

            // Read headers and cookies
            var keys = property.Headers.AllKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                if (acceptedHeaders.Contains(key))
                {
                    // If the header should be kept for the next request, just save it
                    this.headers.Set(key, property.Headers[i]);
                }
                else if (StringComparer.InvariantCultureIgnoreCase.Compare(key, Constants.HttpHeaderSetCookie) == 0)
                {
                    cookies.SetCookies(uri, property.Headers[i]);
                }
            }
        }
    }
}
