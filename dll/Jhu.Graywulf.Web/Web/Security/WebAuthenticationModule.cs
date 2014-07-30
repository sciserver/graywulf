using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements a generic authentication scheme for Graywulf
    /// with pluggable authenticator algorithms
    /// </summary>
    /// <remarks>
    /// This class implements an HTTP module that inspects each request coming from
    /// the web browser and calls a set of authenticators to attempt to authenticate
    /// the user based on request parameters or cookies. The authenticated user's
    /// details are cached in the session.
    /// </remarks>
    public class WebAuthenticationModule : AuthenticationModuleBase, IHttpModule
    {
        public WebAuthenticationModule()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Initialize the authentication module by loading authenticators and setting events.
        /// </summary>
        /// <param name="application"></param>
        /// <remarks>
        /// This function if called once by asp.net once, when the
        /// application starts.
        /// </remarks>
        public void Init(HttpApplication application)
        {
            // Load authenticators from the registry
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                // The admin interface doesn't have a domain associated with, this
                // special case has to be handled here
                if (context.Domain != null)
                {
                    // Initialize authenticators            

                    var af = AuthenticatorFactory.Create(context.Domain);
                    RegisterAuthenticators(af.GetWebRequestAuthenticators());
                }
            }

            // Wire up request events

            // --- Call all authenticators in this one
            application.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
            // --- Associate identity with graywulf user
            application.PostAuthenticateRequest += new EventHandler(OnPostAuthenticateRequest);
            // --- Identify authenticated user
            application.PostAcquireRequestState += new EventHandler(OnPostAcquireRequestState);
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            Authenticate(new AuthenticationRequest(HttpContext.Current.Request));
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            var httpContext = HttpContext.Current;
            var principal = DispatchIdentityType(httpContext.User);

            if (principal != null)
            {
                httpContext.User = principal;
            }
        }

        private void OnPostAcquireRequestState(object sender, EventArgs e)
        {
            IdentifyUser();
        }

        protected override void OnAuthenticated(GraywulfPrincipal principal)
        {
            HttpContext.Current.User = principal;
        }

        protected override void OnAuthenticationFailed()
        {
        }

        /// <summary>
        /// Checks if the authenticated user appears for the first time,
        /// and if so, raises an event. Also checks if the user signed out.
        /// </summary>
        private void IdentifyUser()
        {
            // To detect whether a user has left or a new user arrived we
            // store the principal in a session variable. Each request
            // looks into the variable and compares it with the principal
            // of the current request.
            // If the current request is authenticated by Graywulf, and this is the
            // first time we see the user, we need to load details from the registry

            var httpContext = HttpContext.Current;
            var httpApplication = (ApplicationBase)HttpContext.Current.ApplicationInstance;

            if (httpContext.Session != null)
            {
                // Get the saved principal from the session
                var sessionPrincipal = (GraywulfPrincipal)httpContext.Session[Web.Constants.SessionPrincipal];

                if (httpContext.Request.IsAuthenticated && httpContext.User is GraywulfPrincipal)
                {
                    // There is a principal saved in the session. This is either a returning
                    // user or someone who has just left the site.
                    if (sessionPrincipal != null)
                    {
                        // Make sure that the session principal is the same as the one just being authenticated
                        if (!sessionPrincipal.Identity.CompareByIdentifier((GraywulfIdentity)httpContext.User.Identity))
                        {
                            // This is someone we haven't seen, so report the
                            // leaving of the previous user
                            httpApplication.OnUserSignedOut();
                            httpContext.Session.Abandon();
                            sessionPrincipal = null;
                        }
                        else
                        {
                            HttpContext.Current.User = sessionPrincipal;
                        }
                    }

                    if (sessionPrincipal == null)
                    {
                        // A new user has just arrived, load info from the registry
                        using (var registryContext = httpApplication.CreateRegistryContext())
                        {
                            ((GraywulfIdentity)httpContext.User.Identity).LoadUser();
                        }

                        httpContext.Session[Web.Constants.SessionPrincipal] = httpContext.User;

                        // Report the arrival of a new user
                        httpApplication.OnUserSignedIn((GraywulfIdentity)httpContext.User.Identity);
                    }
                }
                else if (!httpContext.Request.IsAuthenticated && sessionPrincipal != null)
                {
                    // No authenticated used but we see someone in the session. That means
                    // someone has just left (or the session has expired)
                    // Report the leaving of the user
                    httpApplication.OnUserSignedOut();
                    httpContext.Session.Abandon();
                }
            }
        }
    }
}
