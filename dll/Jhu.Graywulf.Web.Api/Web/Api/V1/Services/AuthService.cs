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
        [WebGet(UriTemplate = "/me")]
        [Description("Returns information on the authenticated user.")]
        UserResponse GetCurrentUser();

        [OperationContract]
        [WebGet(UriTemplate = "/me/roles")]
        [Description("Returns information on group membership.")]
        UserMembershipListResponse GetCurrentUserRoles();

        [OperationContract]
        [WebGet(UriTemplate = "/me/roles/{groupName}")]
        [Description("Returns information on group membership.")]
        UserMembershipResponse GetCurrentUserRoleInGroup(string groupName);

        [OperationContract]
        [WebGet(UriTemplate = "/users/{name}")]
        UserResponse GetUser(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/users/{name}/roles")]
        UserMembershipListResponse GetUserRoles(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/users/{name}/roles/{groupName}")]
        [Description("Returns information on group membership.")]
        UserMembershipResponse GetUserRoleInGroup(string name, string groupName);

        [OperationContract]
        [WebGet(UriTemplate = "/groups")]
        UserGroupListResponse FindUserGroups();

        [OperationContract]
        [WebGet(UriTemplate = "/groups/{name}")]
        UserGroupResponse GetUserGroup(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/groups/{name}/members")]
        UserMembershipListResponse GetUserGroupMembers(string name);
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

        public UserResponse GetCurrentUser()
        {
            User user;
            var principal = System.Threading.Thread.CurrentPrincipal as GraywulfPrincipal;
                    
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                user = new User(principal.Identity);
            }
            else
            {
                user = User.Guest;
            }

            var userResp = new UserResponse(user);
            return userResp;
        }

        public UserMembershipListResponse GetCurrentUserRoles()
        {
            throw new NotImplementedException();
        }

        public UserMembershipResponse GetCurrentUserRoleInGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public UserResponse GetUser(string name)
        {
            throw new NotImplementedException();
        }

        public UserMembershipListResponse GetUserRoles(string name)
        {
            throw new NotImplementedException();
        }

        public UserMembershipResponse GetUserRoleInGroup(string name, string groupName)
        {
            throw new NotImplementedException();
        }

        public UserGroupListResponse FindUserGroups()
        {
            throw new NotImplementedException();
        }

        public UserGroupResponse GetUserGroup(string name)
        {
            throw new NotImplementedException();
        }

        public UserMembershipListResponse GetUserGroupMembers(string name)
        {
            throw new NotImplementedException();
        }
    }
}
