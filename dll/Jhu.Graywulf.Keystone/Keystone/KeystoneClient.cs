using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    /// <summary>
    /// Implements a client for the OpenStack Keystone v3 identity service
    /// REST interface.
    /// </summary>
    /// <remarks>
    /// https://github.com/openstack/identity-api/blob/master/v3/src/markdown/identity-api-v3.md
    /// </remarks>
    public class KeystoneClient : RestClient
    {
        #region Private member variables

        private string adminAuthToken;
        private string userAuthToken;

        #endregion
        #region Properties

        public string AdminAuthToken
        {
            get { return adminAuthToken; }
            set { adminAuthToken = value; }
        }

        public string UserAuthToken
        {
            get { return userAuthToken; }
            set { userAuthToken = value; }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneClient(Uri baseUri)
            : base(baseUri)
        {
        }

        #endregion
        #region Version

        public Version GetVersion()
        {
            var res = SendRequest<VersionResponse>(
                HttpMethod.Get, "/v3", adminAuthToken);

            return res.Body.Version;
        }

        #endregion
        #region Domain manipulation

        public Domain CreateDomain(Domain domain)
        {
            var req = DomainRequest.CreateMessage(domain);
            var res = SendRequest<DomainRequest, DomainResponse>(
                HttpMethod.Post, "/v3/domains", req, adminAuthToken);

            return res.Body.Domain;
        }

        public Domain UpdateDomain(Domain domain)
        {
            var req = DomainRequest.CreateMessage(domain);
            var res = SendRequest<DomainRequest, DomainResponse>(
                HttpMethod.Patch, String.Format("/v3/domains/{0}", domain.ID), req, adminAuthToken);

            return res.Body.Domain;
        }

        public void DeleteDomain(string id)
        {
            // Domain needs to be deleted first
            var domain = GetDomainByID(id);
            domain.Enabled = false;
            UpdateDomain(domain);

            // Now it can be deleted
            SendRequest(HttpMethod.Delete, String.Format("/v3/domains/{0}", id), adminAuthToken);
        }

        public Domain GetDomainByID(string id)
        {
            var res = SendRequest<DomainResponse>(
                HttpMethod.Get, String.Format("/v3/domains/{0}", id), adminAuthToken);

            return res.Body.Domain;
        }

        public Domain[] ListDomains()
        {
            var res = SendRequest<DomainListResponse>(
                HttpMethod.Get, "/v3/domains", adminAuthToken);

            return res.Body.Domains;
        }

        #endregion
        #region Project manipulation

        public Project CreateProject(Project project)
        {
            var req = ProjectRequest.CreateMessage(project);
            var res = SendRequest<ProjectRequest, ProjectResponse>(
                HttpMethod.Post, "/v3/projects", req, adminAuthToken);

            return res.Body.Project;
        }

        public Project UpdateProject(Project project)
        {
            var req = ProjectRequest.CreateMessage(project);
            var res = SendRequest<ProjectRequest, ProjectResponse>(
                HttpMethod.Patch, String.Format("/v3/projects/{0}", project.ID), req, adminAuthToken);

            return res.Body.Project;
        }

        public void DeleteProject(string id)
        {
            // Now it can be deleted
            SendRequest(HttpMethod.Delete, String.Format("/v3/projects/{0}", id), adminAuthToken);
        }

        public Project GetProjectByID(string id)
        {
            var res = SendRequest<ProjectResponse>(
                HttpMethod.Get, String.Format("/v3/projects/{0}", id), adminAuthToken);

            return res.Body.Project;
        }

        public Project[] ListProjects()
        {
            var res = SendRequest<ProjectListResponse>(
                HttpMethod.Get, "/v3/projects", adminAuthToken);

            return res.Body.Projects;
        }

        public Project[] ListProjects(string domainID)
        {
            var res = SendRequest<ProjectListResponse>(
                HttpMethod.Get, String.Format("/v3/domains/{0}/projects", domainID), adminAuthToken);

            return res.Body.Projects;
        }

        #endregion
        #region Role manipulation

        public Role CreateRole(Role project)
        {
            var req = RoleRequest.CreateMessage(project);
            var res = SendRequest<RoleRequest, RoleResponse>(
                HttpMethod.Post, "/v3/roles", req, adminAuthToken);

            return res.Body.Role;
        }

        public Role UpdateRole(Role project)
        {
            var req = RoleRequest.CreateMessage(project);
            var res = SendRequest<RoleRequest, RoleResponse>(
                HttpMethod.Patch, String.Format("/v3/roles/{0}", project.ID), req, adminAuthToken);

            return res.Body.Role;
        }

        public void DeleteRole(string id)
        {
            // Now it can be deleted
            SendRequest(HttpMethod.Delete, String.Format("/v3/roles/{0}", id), adminAuthToken);
        }

        public Role GetRoleByID(string id)
        {
            var res = SendRequest<RoleResponse>(
                HttpMethod.Get, String.Format("/v3/roles/{0}", id), adminAuthToken);

            return res.Body.Role;
        }

        public Role[] ListRoles()
        {
            var res = SendRequest<RoleListResponse>(
                HttpMethod.Get, "/v3/roles", adminAuthToken);

            return res.Body.Roles;
        }

        public Role[] ListRole(string domainID)
        {
            var res = SendRequest<RoleListResponse>(
                HttpMethod.Get, String.Format("/v3/domains/{0}/roles", domainID), adminAuthToken);

            return res.Body.Roles;
        }

        #endregion
        #region Group manipulation

        #endregion
        #region User manipulation

        public User CreateUser(User user)
        {
            var req = UserRequest.CreateMessage(user);
            var res = SendRequest<UserRequest, UserResponse>(
                HttpMethod.Post, "/v3/users", req, adminAuthToken);

            return res.Body.User;
        }

        public User UpdateUser(User user)
        {
            var req = UserRequest.CreateMessage(user);
            var res = SendRequest<UserRequest, UserResponse>(
                HttpMethod.Patch, String.Format("/v3/users/{0}", user.ID), req, adminAuthToken);

            return res.Body.User;
        }

        public void DeleteUser(string id)
        {
            SendRequest(HttpMethod.Delete, String.Format("/v3/users/{0}", id), adminAuthToken);
        }

        public void ChangePassword(string id, string oldPassword, string newPassword)
        {
            var user = new User()
            {
                OriginalPassword = oldPassword,
                Password = newPassword
            };

            var req = UserRequest.CreateMessage(user);

            SendRequest<UserRequest>(
                HttpMethod.Post, String.Format("/v3/users/{0}/password", id), req, adminAuthToken);
        }

        public User GetUserByID(string id)
        {
            var res = SendRequest<UserResponse>(
                HttpMethod.Get, String.Format("/v3/users/{0}", id), adminAuthToken);

            return res.Body.User;
        }

        public User GetUser(Token token)
        {
            // Token might not contain user info, so authenticate with
            // it to get user id
            token = Authenticate(token);

            return GetUserByID(token.User.ID);
        }

        public User[] ListUsers()
        {
            var res = SendRequest<UserListResponse>(
                HttpMethod.Get, "/v3/users", adminAuthToken);

            return res.Body.Users;
        }

        public User[] FindUsers(string name, bool enabledOnly, bool caseInsensitive)
        {
            var query = BuildSearchQueryString(name, enabledOnly, caseInsensitive);
            var res = SendRequest<UserListResponse>(
                HttpMethod.Get, "/v3/users" + query, adminAuthToken);

            return res.Body.Users;
        }

        public void GrantRoleToUserOnDomain(string domainID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Put,
                String.Format("/v3/domains/{0}/users/{1}/roles/{2}", domainID, userID, roleID),
                adminAuthToken);
        }

        public void RevokeRoleFromUserOnDomain(string domainID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Delete,
                String.Format("/v3/domains/{0}/users/{1}/roles/{2}", domainID, userID, roleID),
                adminAuthToken);
        }

        public void CheckRoleOfUserOnDomain(string domainID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Head,
                String.Format("/v3/domains/{0}/users/{1}/roles/{2}", domainID, userID, roleID),
                adminAuthToken);
        }

        public void ListRolesOfUserOnDomain(string domainID, string userID, string roleID)
        {
        }

        public void GrantRoleToUserOnProject(string projectID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Put,
                String.Format("/v3/projects/{0}/users/{1}/roles/{2}", projectID, userID, roleID),
                adminAuthToken);
        }

        public void RevokeRoleFromUserOnProject(string projectID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Delete,
                String.Format("/v3/projects/{0}/users/{1}/roles/{2}", projectID, userID, roleID),
                adminAuthToken);
        }

        public void CheckRoleOfUserOnProject(string projectID, string userID, string roleID)
        {
            SendRequest(
                HttpMethod.Head,
                String.Format("/v3/projects/{0}/users/{1}/roles/{2}", projectID, userID, roleID),
                adminAuthToken);
        }

        public void ListRolesOfUserOnProject(string projectID, string userID, string roleID)
        {
        }

        #endregion
        #region Authentication and token manipulation

        public Token Authenticate(string domain, string username, string password)
        {
            return Authenticate(domain, username, password, null, null);
        }

        // TODO: test
        public Token Authenticate(string domain, string username, string password, Domain scope)
        {
            return Authenticate(domain, username, password, scope, null);
        }

        // TODO: test
        public Token Authenticate(string domain, string username, string password, Project scope)
        {
            return Authenticate(domain, username, password, null, scope);
        }

        private Token Authenticate(string domain, string username, string password, Domain scopeDomain, Project scopeProject)
        {
            var req = AuthRequest.CreateMessage(domain, username, password, null, null);
            var resMessage = SendRequest<AuthRequest, AuthResponse>(
                HttpMethod.Post, "/v3/auth/tokens", req);

            var authResponse = resMessage.Body;

            // Token value comes in the header
            authResponse.Token.ID = resMessage.Headers[Constants.KeystoneXSubjectTokenHeader].Value;

            return authResponse.Token;
        }

        public Token Authenticate(Token token)
        {
            return Authenticate(token, null);
        }

        public Token Authenticate(Token token, Trust trust)
        {
            var req = AuthRequest.CreateMessage(token, trust);
            var resMessage = SendRequest<AuthRequest, AuthResponse>(
                HttpMethod.Post, "/v3/auth/tokens", req);

            var authResponse = resMessage.Body;

            // Token value comes in the header
            authResponse.Token.ID = resMessage.Headers[Constants.KeystoneXSubjectTokenHeader].Value;

            return authResponse.Token;
        }

        public bool ValidateToken(Token token)
        {
            var headers = new RestHeaderCollection();
            headers.Add(new RestHeader(Constants.KeystoneXSubjectTokenHeader, token.ID));

            var resMessage = SendRequest(
                HttpMethod.Head, "/v3/auth/tokens", headers, adminAuthToken);

            return true;
        }

        public void RevokeToken(Token token)
        {
            var headers = new RestHeaderCollection();
            headers.Add(new RestHeader(Constants.KeystoneXSubjectTokenHeader, token.ID));

            var resMessage = SendRequest(
                HttpMethod.Delete, "/v3/auth/tokens", headers, adminAuthToken);
        }

        #endregion
        #region Trusts

        public Trust CreateTrust(Trust trust)
        {
            var req = TrustRequest.CreateMessage(trust);
            var res = SendRequest<TrustRequest, TrustResponse>(
                HttpMethod.Post, "/v3/OS-TRUST/trusts", req, userAuthToken);

            return res.Body.Trust;
        }

        #endregion
        #region Specialized request functions

        private string BuildSearchQueryString(string name, bool enabledOnly, bool caseInsensitive)
        {
            // Build query string
            var query = "";

            if (name != null)
            {
                name = name.Trim();

                query += "&name";

                // Check if wildcard is used
                string method = null;
                var startstar = name.StartsWith("*");
                var endstar = name.EndsWith("*");

                if (startstar && endstar)
                {
                    method = "contains";
                }
                else if (startstar)
                {
                    method = "endswith";
                }
                else if (endstar)
                {
                    method = "startswith";
                }

                // Check if case insensitive
                string caseis = caseInsensitive ? "i" : "";

                if (method != null)
                {
                    query += "_" + caseis + method;
                }

                query += "=" + UrlEncode(name.Trim('*'));
            }

            if (enabledOnly)
            {
                query += "&enabled";
            }

            if (query != "")
            {
                query = "?" + query.Substring(1);
            }

            return query;
        }

        private RestHeaderCollection PreprocessHeaders(RestHeaderCollection headers, string authToken)
        {
            if (headers == null)
            {
                headers = new RestHeaderCollection();
            }

            headers.Set(new RestHeader(Constants.KeystoneXAuthTokenHeader, authToken));

            return headers;
        }

        private RestMessage<T> PreprocessHeaders<T>(RestMessage<T> message, string authToken)
        {
            message.Headers = PreprocessHeaders(message.Headers, authToken);

            return message;
        }

        private RestHeaderCollection SendRequest(HttpMethod method, string path, string authToken)
        {
            return SendRequest(method, path, (RestHeaderCollection)null, authToken);
        }

        protected RestHeaderCollection SendRequest(HttpMethod method, string path, RestHeaderCollection headers, string authToken)
        {
            try
            {
                return base.SendRequest(method, path, PreprocessHeaders(headers, authToken));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        private RestMessage<R> SendRequest<R>(HttpMethod method, string path, string authToken)
        {
            return SendRequest<R>(method, path, (RestHeaderCollection)null, authToken);
        }

        protected RestMessage<R> SendRequest<R>(HttpMethod method, string path, RestHeaderCollection headers, string authToken)
        {
            try
            {
                return base.SendRequest<R>(method, path, PreprocessHeaders(headers, authToken));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        protected RestHeaderCollection SendRequest<T>(HttpMethod method, string path, RestMessage<T> message, string authToken)
        {
            try
            {
                return base.SendRequest<T>(method, path, PreprocessHeaders(message, authToken));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        protected RestMessage<U> SendRequest<T, U>(HttpMethod method, string path, RestMessage<T> message, string authToken)
        {
            try
            {
                return base.SendRequest<T, U>(method, path, PreprocessHeaders(message, authToken));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        /// <summary>
        /// Creates a Keystone exception from a generic REST exception.
        /// </summary>
        /// <remarks>
        /// Interprets the response body as a ErrorResponse and reads
        /// properties.
        /// </remarks>
        /// <param name="ex"></param>
        /// <returns></returns>
        private KeystoneException CreateException(RestException ex)
        {
            KeystoneException kex = null;
            var error = DeserializeJson<ErrorResponse>(ex.Body);

            if (error != null)
            {
                kex = new KeystoneException(error.Error.Message, ex)
                {
                    Title = error.Error.Title,
                    StatusCode = (HttpStatusCode)error.Error.Code
                };
            }
            else if (ex.InnerException is WebException)
            {
                kex = new KeystoneException(ex.Message, ex)
                {
                    StatusCode = ((HttpWebResponse)((WebException)ex.InnerException).Response).StatusCode
                };
            }
            else
            {
                kex = new KeystoneException(ex.Message, ex);
            }

            return kex;
        }

        #endregion
    }
}
