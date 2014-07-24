using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using System.Web.Security;
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

        private string adminToken;
        private string domain;

        #endregion
        #region Properties

        public override string ProtocolName
        {
            get { return Constants.ProtocolNameKeystone; }
        }

        public override bool IsWebInteractive
        {
            get { return true; }
        }

        public override bool IsWebRequest
        {
            get { return true; }
        }

        public override bool IsRestRequest
        {
            get { return true; }
        }

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

        #endregion
        #region Constructors and initializers

        public KeystoneAuthenticator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            AuthorityName = Constants.AuthorityNameKeystone;

            this.graywulfDomainPrefix = null;
            this.adminToken = null;
            this.domain = "default";
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
            var tokenID = httpContext.Request.Headers.Get(Constants.KeystoneAuthTokenHeader);   
            // TODO: use custom property

            if (tokenID == null)
            {
                // Try to take header from the query string
                tokenID = httpContext.Request.QueryString["token"]; // TODO: use custom property
            }

            if (tokenID != null)
            {
                Token token;

                // Check if the resolved token is already in the cache
                if (!tokenCache.TryGetValue(tokenID, out token))
                {
                    // Need to validate token against Keystone

                    var ksclient = new KeystoneClient(new Uri(AuthorityUrl))
                    {
                        AdminAuthToken = adminToken,
                    };

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
            return new KeystoneClient(new Uri(AuthorityUrl))
            {
                AdminAuthToken = adminToken,
            };
        }

        private GraywulfPrincipal CreatePrincipal(Token token)
        {
            var principal = base.CreatePrincipal();
            var identity = principal.Identity;

            identity.Identifier = token.User.ID;

            identity.User.Name = token.User.Name;
            identity.User.Comments = token.User.Description;
            identity.User.Email = token.User.Email;
            
            // TODO: fill in additional information based on user data
            // in the keystone token
                        
            return new GraywulfPrincipal(identity);
        }
    }
}
