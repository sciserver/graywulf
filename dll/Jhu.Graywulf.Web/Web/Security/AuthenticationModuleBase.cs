using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implement functions to authenticate web requests using a set
    /// of authenticators.
    /// </summary>
    /// <remarks>
    /// This class is a base for two implementations: for one to authenticate
    /// web page request based on the HTTP request header and for another
    /// 
    /// </remarks>
    public abstract class AuthenticationModuleBase : IDisposable
    {
        #region Static principal cache implementation

        /// <summary>
        /// Holds a cache of principals to identify users without accessing
        /// the registry.
        /// </summary>
        private static Cache<string, GraywulfPrincipal> principalCache;

        /// <summary>
        /// Initializes static variables
        /// </summary>
        static AuthenticationModuleBase()
        {
            principalCache = new Cache<string, GraywulfPrincipal>()
            {
                AutoExtendLifetime = true,
                CollectionInterval = new TimeSpan(0, 1, 0),     // one minute
                // TODO: implement per token expiration...
                DefaultLifetime = new TimeSpan(0, 20, 0),       // twenty minutes
            };
        }

        #endregion
        #region Private member variables

        /// <summary>
        /// Holds a list of authenticators that are tried to identify a user
        /// from a web request.
        /// </summary>
        private Authenticator[] authenticators;

        #endregion
        #region Constructors and initializers

        protected AuthenticationModuleBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public virtual void Dispose()
        {
        }

        #endregion

        /// <summary>
        /// Registers request authenticators.
        /// </summary>
        /// <param name="authenticators"></param>
        protected void RegisterAuthenticators(IEnumerable<Authenticator> authenticators)
        {
            this.authenticators = authenticators.ToArray();
        }

        /// <summary>
        /// Calls all registered request authenticators on.
        /// </summary>
        /// <param name="context"></param>
        protected virtual AuthenticationResponse Authenticate(AuthenticationRequest request)
        {
            // See if we can use the principal as it is, otherwise try
            // other authentication methods
            // The principal is usually valid if a built-in authentication protocol,
            // like forms ticket, successfully identified the user.
            var response = new AuthenticationResponse(request);
            response.SetPrincipal(DispatchPrincipal(request.Principal));

            // If user is not authenticated yet, try to authenticate them now using
            // various types of authenticators
            if (authenticators != null)
            {
                // Try each authentication protocol
                for (int i = 0; i < authenticators.Length; i++)
                {
                    authenticators[i].Authenticate(request, response);
                }
            }

            if (response.Principal != null)
            {
                // Associate user identified by the authentication method with a Graywulf user
                var principal = response.Principal;
                
                LoadUser(ref principal);
                response.SetPrincipal(principal);

                // Report user as authenticated
                OnAuthenticated(response);
            }
            else
            {
                // None of the authenticators could identify the user
                // This only means that the custom authenticators could not
                // identify the user, but it still might have been identified by
                // the web server (from Forms ticket, windows authentication, etc.)
                // In this case, the principal provided by the framework needs to
                // be converted to a graywulf principal

                OnAuthenticationFailed();
            }

            return response;
        }

        /// <summary>
        /// When implemented in a derived class, called when a user is successfully authenticated.
        /// </summary>
        /// <param name="response"></param>
        protected abstract void OnAuthenticated(AuthenticationResponse response);

        /// <summary>
        /// When implemented in a derived class, called when all attempts to authenticate the user have failed.
        /// </summary>
        protected abstract void OnAuthenticationFailed();

        /// <summary>
        /// Loads the user object, from the cache or from the Graywulf registry.
        /// </summary>
        /// <param name="principal"></param>
        protected void LoadUser(ref GraywulfPrincipal principal)
        {
            // REST services do not use a session but we don't want to load the user
            // every single time from the session so do some caching here.
            // The problem is, however, that we don't want to keep the user in the cache
            // for ever, so some cache expiration should be done

            GraywulfPrincipal cachedPrincipal;
            if (principalCache.TryGetValue(principal.UniqueID, out cachedPrincipal))
            {
                principal = cachedPrincipal;
            }
            else
            {
                // User not found in cache, need to load from database
                using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
                {
                    var ip = new GraywulfIdentityProvider(registryContext);
                    ip.LoadOrCreateUser(principal.Identity);
                }

                principalCache.TryAdd(principal.UniqueID, principal);
            }
        }

        #region Principal type conversion methods

        /// <summary>
        /// Converts principals and identities established by built-in authentication
        /// methods into a Graywulf principal and identity.
        /// </summary>
        /// <param name="context"></param>
        protected virtual GraywulfPrincipal DispatchPrincipal(IPrincipal principal)
        {
            // The request is processed now. If the user has been authenticated but
            // the principal is not a Graywulf principal, it has to be replaced now
            if (principal != null)
            {
                var identity = principal.Identity;

                if (identity is System.Security.Principal.GenericIdentity)
                {
                    // By default, identity is a generic identity with
                    // IsAuthorized = false, so let this be
                    return null;
                }
                else if (identity is GraywulfIdentity)
                {
                    // Nothing to do here
                    return (GraywulfPrincipal)principal;
                }
                else if (identity is System.Web.Security.FormsIdentity)
                {
                    return CreatePrincipal((System.Web.Security.FormsIdentity)identity);
                }
                else if (identity is System.Security.Principal.WindowsIdentity)
                {
                    throw new NotImplementedException();
                }
                else if (identity is System.Web.Security.PassportIdentity)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a Graywulf principal based on the user name stored in the
        /// forms identity.
        /// </summary>
        /// <param name="formsIdentity"></param>
        /// <returns></returns>
        /// <remarks>
        /// FormsIdentity is always automatically accepted as master authority.
        /// </remarks>
        private GraywulfPrincipal CreatePrincipal(System.Web.Security.FormsIdentity formsIdentity)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameForms,
                Identifier = formsIdentity.Name,
                IsAuthenticated = true,
                IsMasterAuthority = true,
            };

            identity.UserReference.Name = formsIdentity.Name;

            return new GraywulfPrincipal(identity);
        }

        #endregion
    }
}
