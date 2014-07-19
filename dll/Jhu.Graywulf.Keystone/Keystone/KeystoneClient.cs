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

        #endregion
        #region Properties

        public string AdminAuthToken
        {
            get { return adminAuthToken; }
            set { adminAuthToken = value; }
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
                HttpMethod.Get, "/v3");

            return res.Body.Version;
        }

        #endregion
        #region Single user manipulation

        public User CreateUser(User user)
        {
            var req = UserRequest.CreateMessage(user);
            var res = SendRequest<UserRequest, UserResponse>(
                HttpMethod.Post, "/v3/users", req);

            return res.Body.User;
        }

        public User GetUser(string id)
        {
            var res = SendRequest<UserResponse>(
                HttpMethod.Get, String.Format("/v3/users/{0}", id));

            return res.Body.User;
        }

        public User GetUser(Token token)
        {
            // Token might not contain user info, so authenticate with
            // it to get user id
            token = Authenticate(token);

            return GetUser(token.User.ID);
        }

        public User UpdateUser(User user)
        {
            var req = UserRequest.CreateMessage(user);
            var res = SendRequest<UserRequest, UserResponse>(
                HttpMethod.Patch, String.Format("/v3/users/{0}", user.ID), req);

            return res.Body.User;
        }

        public void DeleteUser(string id)
        {
            SendRequest(HttpMethod.Delete, String.Format("/v3/users/{0}", id));
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
                HttpMethod.Post, String.Format("/v3/users/{0}/password", id), req);
        }

        #endregion
        #region User search

        public User[] ListUsers()
        {
            var res = SendRequest<UserListResponse>(
                HttpMethod.Get,
                "/v3/users");

            return res.Body.Users;
        }

        public User[] FindUsers(string name, bool enabledOnly, bool caseInsensitive)
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

            var res = SendRequest<UserListResponse>(
                HttpMethod.Get,
                "/v3/users" + query);

            return res.Body.Users;
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
                HttpMethod.Post,
                "/v3/auth/tokens",
                req);

            var authResponse = resMessage.Body;

            // Token value comes in the header
            authResponse.Token.ID = resMessage.Headers[Constants.KeystoneXSubjectTokenHeader].Value;

            return authResponse.Token;
        }

        public Token Authenticate(Token token)
        {
            var req = AuthRequest.CreateMessage(token);
            var resMessage = SendRequest<AuthRequest, AuthResponse>(
                HttpMethod.Post,
                "/v3/auth/tokens",
                req);

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
                HttpMethod.Head,
                "/v3/auth/tokens",
                headers);

            return true;
        }

        public void RevokeToken(Token token)
        {
            var headers = new RestHeaderCollection();
            headers.Add(new RestHeader(Constants.KeystoneXSubjectTokenHeader, token.ID));

            var resMessage = SendRequest(
                HttpMethod.Delete,
                "/v3/auth/tokens",
                headers);
        }

        #endregion
        #region Specialized request functions

        private RestHeaderCollection PreprocessHeaders(RestHeaderCollection headers)
        {
            if (headers == null)
            {
                headers = new RestHeaderCollection();
            }

            headers.Set(new RestHeader(Constants.KeystoneXAuthTokenHeader, adminAuthToken));

            return headers;
        }

        private RestMessage<T> PreprocessHeaders<T>(RestMessage<T> message)
        {
            message.Headers = PreprocessHeaders(message.Headers);

            return message;
        }

        private RestHeaderCollection SendRequest(HttpMethod method, string path)
        {
            return SendRequest(method, path, (RestHeaderCollection)null);
        }

        protected override RestHeaderCollection SendRequest(HttpMethod method, string path, RestHeaderCollection headers)
        {
            try
            {
                return base.SendRequest(method, path, PreprocessHeaders(headers));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        private RestMessage<R> SendRequest<R>(HttpMethod method, string path)
        {
            return SendRequest<R>(method, path, (RestHeaderCollection)null);
        }

        protected override RestMessage<R> SendRequest<R>(HttpMethod method, string path, RestHeaderCollection headers)
        {
            try
            {
                return base.SendRequest<R>(method, path, PreprocessHeaders(headers));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        protected override RestHeaderCollection SendRequest<T>(HttpMethod method, string path, RestMessage<T> message)
        {
            try
            {
                return base.SendRequest<T>(method, path, PreprocessHeaders(message));
            }
            catch (RestException ex)
            {
                throw CreateException(ex);
            }
        }

        protected override RestMessage<U> SendRequest<T, U>(HttpMethod method, string path, RestMessage<T> message)
        {
            try
            {
                return base.SendRequest<T, U>(method, path, PreprocessHeaders(message));
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
