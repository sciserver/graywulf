using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// Implements a generic authentication scheme for Graywulf
    /// with pluggable authenticator algorithms
    /// </summary>
    public class GraywulfAuthenticationModule : IHttpModule
    {
        private RequestAuthenticatorBase[] authenticators;

        public GraywulfAuthenticationModule()
        {
        }

        /// <summary>
        /// Initialize the authentication module events.
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>
        /// This function if called once by asp.net once, when the
        /// application starts.
        /// </remarks>
        public void Init(HttpApplication context)
        {
            // Initialize identity cache
            context.Application[Constants.ApplicationPrincipalCache] = new GraywulfPrincipalCache();

            // Create authenticators
            var af = AuthenticatorFactory.Create(null);
            this.authenticators = af.CreateRequestAuthenticators();

            // Wire up request events
            // --- Call all authenticators in this one
            context.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
            // --- Associate identity with graywulf user
            context.PostAuthenticateRequest += new EventHandler(OnPostAuthenticateRequest);
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Tries to authenticate the request with all authenticators.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            CallAuthenticators(context);
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            DispatchIdentityType(context);
        }

        private void CallAuthenticators(HttpContext context)
        {
            // If user is not authenticated yet, try to authenticate them now using
            // various types of authenticators

            // Try each authentication protocol
            for (int i = 0; context.User == null && i < authenticators.Length; i++)
            {
                var user = authenticators[i].Authenticate();
                if (user != null)
                {
                    context.User = user;
                }
            }
        }

        private void DispatchIdentityType(HttpContext context)
        {
            // The request is processed now. If the user has been authenticated but
            // the principal is not a Graywulf principal, it has to be replaced now
            if (context != null && context.User != null)
            {
                var identity = context.User.Identity;

                if (identity is GraywulfIdentity)
                {
                    // Nothing to do here
                }
                else if (identity is System.Security.Principal.GenericIdentity)
                {
                    // By default, identity is a generic identity with
                    // IsAuthorized = false, so let this be
                }
                else if (identity is System.Security.Principal.WindowsIdentity)
                {
                    throw new NotImplementedException();
                }
                else if (identity is System.Web.Security.PassportIdentity)
                {
                    throw new NotImplementedException();
                }
                else if (identity is System.Web.Security.FormsIdentity)
                {
                    CreateGraywulfPrincipal(context, (System.Web.Security.FormsIdentity)identity);
                }
                else if (identity is System.Security.Principal.GenericIdentity)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            /*
            if (context.User is GraywulfPrincipal)
            {
                LoadGraywulfPrincipal(context);
            }*/
        }

        /// <summary>
        /// Creates a graywulf principal based on the guid stored in the
        /// forms authentication token.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private void CreateGraywulfPrincipal(HttpContext context, System.Web.Security.FormsIdentity formsIdentity)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameForms,
                Identifier = formsIdentity.Name,
                IsAuthenticated = true,
            };

            identity.UserProperty.Name = formsIdentity.Name;

            context.User = new GraywulfPrincipal(identity);
        }

        /*
        private void LoadGraywulfPrincipal(HttpContext context)
        {
            // See if identity can be found in the cache. If so, use that
            // instance, otherwise load from the database.

            var contextPrincipal = (GraywulfPrincipal)context.User;
            context.User = CacheGraywulfPrincipal(context.Application, contextPrincipal);

            if (!((GraywulfIdentity)context.User.Identity).IsAuthenticated)
            {
                AuthenticateGraywulfPrincipal(context);
            }
        }

        /// <summary>
        /// Authenticates user based on the Graywulf user database
        /// </summary>
        /// <param name="identity"></param>
        private void AuthenticateGraywulfPrincipal(HttpContext context)
        {
            var principal = (GraywulfPrincipal)context.User;
            var identity = (GraywulfIdentity)principal.Identity;

            using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                switch (identity.Protocol)
                {
                    case Constants.ProtocolNameForms:
                        var ef = new EntityFactory(registryContext);
                        identity.User = ef.LoadEntity<User>(registryContext.Domain.GetFullyQualifiedName(), identity.Identifier);
                        break;
                    case Constants.ProtocolNameWindows:
                        // TODO: implement NTLM auth
                        throw new NotImplementedException();
                    default:
                        // All other cases use lookup by protocol name
                        try
                        {
                            var uf = new UserFactory(registryContext);
                            identity.User = uf.FindUserByIdentity(identity.Protocol, identity.Authority, identity.Identifier);
                        }
                        catch (EntityNotFoundException)
                        {
                            identity.User = null;
                        }
                        break;
                }
            }

            // TODO: make sure user is activated
            // if (identity.User.DeploymentState == DeploymentState.Deployed)
        }

        internal static GraywulfPrincipal CacheGraywulfPrincipal(HttpApplicationState context, GraywulfPrincipal principal)
        {
            var principalCache = (GraywulfPrincipalCache)context[Constants.ApplicationPrincipalCache];
            return principalCache.GetOrAdd(principal);
        }

        internal static void FlushGraywulfPrincipal(HttpApplicationState context, GraywulfPrincipal principal)
        {
            var principalCache = (GraywulfPrincipalCache)context[Constants.ApplicationPrincipalCache];
            
            GraywulfPrincipal old;
            principalCache.TryRemove(principal, out old);
        }
         * */
    }
}
