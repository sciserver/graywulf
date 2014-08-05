using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    public interface IAuthService
    {
        [OperationContract]
        [WebInvoke(Method = HttpMethod.Head, UriTemplate = "/")]
        void Authenticate(AuthRequest authRequest);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class AuthService : RestServiceBase, IAuthService
    {

        public void Authenticate(AuthRequest authRequest)
        {
            if (authRequest != null && authRequest.Auth != null)
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);

                ip.VerifyPassword(authRequest.Auth.Username, authRequest.Auth.Password);
            }
        }
    }
}
