using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        private KeystoneSettings settings;
        
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

        public KeystoneSettings Settings
        {
            get { return settings; }
            set { settings = value; }
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

            this.settings = new KeystoneSettings();
        }

        #endregion

        public override void Initialize(Registry.Domain domain)
        {
            base.Initialize(domain);

            // Make sure settings holds same info as the authenticator
            settings.AuthorityName = this.AuthorityName;
            settings.AuthorityUri = this.AuthorityUri;
        }

        public override void Authenticate(AuthenticationRequest request, AuthenticationResponse response)
        {
            // Keystone tokens (in the simplest case) do not carry any detailed
            // information about the identity of the user. For this reason,
            // every token needs to be validated by calling the Keystone service.
            // To avoid doing this, we need to cache tokens.

            // Look for a token in the request headers
            var tokenID = request.Headers[settings.AuthTokenHeader];

            if (tokenID == null)
            {
                // Try to take header from the query string
                tokenID = request.QueryString[settings.AuthTokenParameter];
            }

            if (tokenID != null)
            {
                Token token;

                // Check if the resolved token is already in the cache
                if (!tokenCache.TryGetValue(tokenID, out token))
                {
                    // Need to validate token against Keystone
                    var ksclient = settings.CreateClient();

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

                settings.UpdateAuthenticationResponse(response, token, IsMasterAuthority);
            }
        }
    }
}
