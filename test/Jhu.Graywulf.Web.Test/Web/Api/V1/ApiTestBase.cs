using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    public class ApiTestBase
    {
        private RestClientBehavior restClient;

        protected ApiTestBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            restClient = new RestClientBehavior();
            restClient.AcceptedHeaders.Add(Jhu.Graywulf.Web.Security.Constants.KeystoneDefaultAuthTokenHeader);
        }

        protected void AuthenticateUser()
        {
            var auth = new AuthRequest()
            {
                Auth = new Auth()
                {
                    Username = "test",
                    Password = "alma"
                }
            };

            var client = CreateClient<IAuthService>(new Uri("http://localhost/gwauth/api/auth.svc"));

            client.Authenticate(auth);
        }

        protected T CreateClient<T>(Uri uri)
        {
            return CreateChannel<T>(CreateWebHttpBinding(), CreateEndpointAddress(uri));
        }

        private T CreateChannel<T>(WebHttpBinding www, EndpointAddress endpoint)
        {
            var cf = new ChannelFactory<T>(www, endpoint);
            cf.Endpoint.Behaviors.Add(restClient);
            return cf.CreateChannel();
        }

        private WebHttpBinding CreateWebHttpBinding()
        {
            var www = new WebHttpBinding(WebHttpSecurityMode.None);

            www.ReaderQuotas.MaxArrayLength = 0x7FFFFFFF;
            www.ReaderQuotas.MaxDepth = 0x7FFFFFFF;
            www.ReaderQuotas.MaxStringContentLength = 0x7FFFFFFF;

            return www;
        }

        private EndpointAddress CreateEndpointAddress(Uri uri)
        {
            return new EndpointAddress(uri);
        }

    }
}
