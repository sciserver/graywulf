using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using System.Web.Security;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Keystone;

namespace Jhu.Graywulf.Web.Security
{

    /// <summary>
    /// Implements functions to authenticate an HTTP request based on
    /// a Keystone token in the header
    /// </summary>
    public class KeystoneAuthenticator : Authenticator
    {
        #region Static cache implementation

        private static readonly Cache<string, Token> tokenCache;

        static KeystoneAuthenticator()
        {
            tokenCache = new Cache<string, Token>(StringComparer.InvariantCultureIgnoreCase)
            {
                AutoExtendLifetime = false,
                CollectionInterval = new TimeSpan(0, 1, 0),     // one minute
                DefaultLifetime = new TimeSpan(0, 20, 0),       // twenty minutes
            };
        }

        #endregion
        #region Private member variables

        private string graywulfDomainPrefix;

        private string adminDomain;
        private string adminProject;
        private string adminUserName;
        private string adminPassword;
        private string adminToken;
        private string domain;
        private string authTokenParameter;
        private string authTokenHeader;

        private DateTime adminTokenExpiresAt;

        #endregion
        #region Properties

        public override string ProtocolName
        {
            get { return Constants.ProtocolNameKeystone; }
        }

        public override AuthenticatorProtocolType ProtocolType
        {
            get
            {
                return AuthenticatorProtocolType.WebRequest |
                       AuthenticatorProtocolType.RestRequest;
            }
        }

        /// <summary>
        /// Gets or sets the Keytone domain of the admin users used
        /// to access the identity service.
        /// </summary>
        public string AdminDomain
        {
            get { return adminDomain; }
            set { adminDomain = value; }
        }

        /// <summary>
        /// Gets or sets the user name of the project/tenant name of
        /// the admin.
        /// </summary>
        public string AdminProject
        {
            get { return adminProject; }
            set { adminProject = value; }
        }

        /// <summary>
        /// Gets or sets the user name of the administrator having
        /// rights to manage the identity service.
        /// </summary>
        public string AdminUserName
        {
            get { return adminUserName; }
            set { adminUserName = value; }
        }

        /// <summary>
        /// Gets or sets the password of the administrator having
        /// rights to manage the identity service.
        /// </summary>
        public string AdminPassword
        {
            get { return adminPassword; }
            set { adminPassword = value; }
        }

        /// <summary>
        /// Gets or sets the admin token used to manage the identity service.
        /// </summary>
        public string AdminToken
        {
            get { return adminToken; }
            set { adminToken = value; }
        }

        /// <summary>
        /// Gets or sets the Keystone domain in which users reside.
        /// </summary>
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter used to convey the
        /// authentication token.
        /// </summary>
        public string AuthTokenParameter
        {
            get { return authTokenParameter; }
            set { authTokenParameter = value; }
        }

        /// <summary>
        /// Gets or sets the name of the header used to convey the
        /// authentication token.
        /// </summary>
        public string AuthTokenHeader
        {
            get { return authTokenHeader; }
            set { authTokenHeader = value; }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneAuthenticator()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            AuthorityName = Constants.AuthorityNameKeystone;

            this.graywulfDomainPrefix = null;
            this.adminProject = null;
            this.adminUserName = null;
            this.adminPassword = null;
            this.adminToken = null;
            this.domain = Constants.KeystoneDefaultUri;
            this.authTokenParameter = Constants.KeystoneDefaultAuthTokenParameter;
            this.authTokenHeader = Constants.KeystoneDefaultAuthTokenHeader;

            this.adminTokenExpiresAt = DateTime.MinValue;
        }

        #endregion

        public override void Initialize(Registry.Domain domain)
        {
            base.Initialize(domain);

            // Save domain name, it will be used to prefix user names
            var name = domain.GetFullyQualifiedName();
            var idx = name.IndexOf(':');

            graywulfDomainPrefix = name.Substring(idx + 1);
        }

        public override GraywulfPrincipal Authenticate(HttpContext httpContext)
        {
            // Keystone tokens (in the simplest case) do not carry any detailed
            // information about the identity of the user. For this reason,
            // every token needs to be validated by calling the Keystone service.
            // To avoid doing this, we need to cache tokens.

            GraywulfPrincipal principal = null;

            // Look for a token in the request headers
            var tokenID = httpContext.Request.Headers.Get(authTokenHeader);

            if (tokenID == null)
            {
                // Try to take header from the query string
                tokenID = httpContext.Request[authTokenParameter];
            }

            if (tokenID != null)
            {
                Token token;

                // Check if the resolved token is already in the cache
                if (!tokenCache.TryGetValue(tokenID, out token))
                {
                    // Need to validate token against Keystone
                    var ksclient = CreateClient();

                    token = new Token()
                    {
                        ID = tokenID
                    };

                    // Keystone doesn't return the user along with
                    // the token, so let's retrieve it now.

                    // TODO: this part might need modifications
                    // if we also accept trusts
                    token.User = ksclient.GetUser(token);

                    tokenCache.TryAdd(token.ID, token);
                }

                // Create a GraywulfPrincipal based on the token
                principal = CreatePrincipal(token);
            }

            return principal;
        }

        private KeystoneClient CreateClient()
        {
            var ksclient = new KeystoneClient(new Uri(AuthorityUrl));

            // If using password authentication, make sure we have a valid admin token
            // Leave e 30 second margin to perform all keystone-related operations with an
            // already existing token
            if (!String.IsNullOrWhiteSpace(adminPassword) && (DateTime.Now - adminTokenExpiresAt).TotalSeconds > -30)
            {
                lock (this)
                {
                    var project = new Keystone.Project()
                    {
                        Domain = new Keystone.Domain()
                        {
                            Name = adminDomain
                        },
                        Name = adminProject
                    };

                    var token = ksclient.Authenticate(adminDomain, adminUserName, adminPassword, project);

                    adminToken = token.ID;
                    adminTokenExpiresAt = token.ExpiresAt;
                }
            }

            // Set the valid admin token
            ksclient.AdminAuthToken = adminToken;

            return ksclient;
        }

        private GraywulfPrincipal CreatePrincipal(Token token)
        {
            var principal = base.CreatePrincipal();
            var identity = principal.Identity;

            identity.Identifier = token.User.ID;

            identity.User = new Registry.User();

            identity.User.Name = token.User.Name;

            // Accept users without the following parameters set but
            // this is not a good practice in general to leave them null 
            // in Keystone
            identity.User.Comments = token.User.Description ?? String.Empty;
            identity.User.Email = token.User.Email ?? String.Empty;

            // TODO: fill in additional information based on user data
            // in the keystone token

            return new GraywulfPrincipal(identity);
        }
    }
}
