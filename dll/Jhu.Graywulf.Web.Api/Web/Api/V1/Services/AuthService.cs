using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    [Description("Manages user authentication.")]
    public interface IAuthService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Post, UriTemplate = "/")]
        [Description("Authenticates a user based on the submitted credentials.")]
        void Authenticate(
            [Description("User credentials.")]
            AuthRequest authRequest);

        [OperationContract]
        [WebGet(UriTemplate = "/owner")]
        [Description("Returns information on the authenticated user.")]
        UserResponse GetOwner();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class AuthService : RestServiceBase, IAuthService
    {
        public void Authenticate(AuthRequest authRequest)
        {
            var principal = System.Threading.Thread.CurrentPrincipal as GraywulfPrincipal;

            if (authRequest != null && authRequest.Credentials != null)
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);

                // Create an authentication request and include headers from the HTTP request
                var request = new AuthenticationRequest(WebOperationContext.Current.IncomingRequest)
                {
                    Username = authRequest.Credentials.Username,
                    Password = authRequest.Credentials.Password,
                };

                var response = ip.VerifyPassword(request);

                principal = response.Principal;

                // In a WCF service, we need to create the Forms ticket manually
                response.AddFormsAuthenticationTicket(false);

                // Set response headers based on authentication results
                response.SetResponseHeaders(WebOperationContext.Current.OutgoingResponse);
            }

            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                throw new System.Security.Authentication.AuthenticationException("Access denied");  // TODO
            }
        }

        public UserResponse GetOwner()
        {
            var principal = System.Threading.Thread.CurrentPrincipal as GraywulfPrincipal;
                    

            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var user = new User(principal.Identity);
                var userResp = new UserResponse(user);
                return userResp;
            }

            else
            {
                throw new NotImplementedException();
            }

        }
    }
}
