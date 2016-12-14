using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Security.Principal;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web.UI;

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
        #region Static declarations

        public static FormsAuthenticationConfiguration Configuration
        {
            get { return (FormsAuthenticationConfiguration)ConfigurationManager.GetSection("jhu.graywulf/authentication/forms"); }
        }

        #endregion
        #region Properties

        public AuthenticationRequest AuthenticationRequest
        {
            get { return (AuthenticationRequest)HttpContext.Current.Items[Constants.HttpContextAuthenticationRequest]; }
        }

        public AuthenticationResponse AuthenticationResponse
        {
            get { return (AuthenticationResponse)HttpContext.Current.Items[Constants.HttpContextAuthenticationResponse]; }
        }

        #endregion
        #region Constructors and initializers


        public WebAuthenticationModule()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
        #region HttpModule life-cycle methods

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
            // (0.) Called by the Http framework when the entire module is initializing

            // Load authenticators from the registry
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                // The admin interface doesn't have a domain associated with, this
                // special case has to be handled here
                if (context.Domain != null)
                {
                    // Initialize authenticators            
                    var af = AuthenticationFactory.Create(context.Domain);
                    RegisterAuthentications(af.GetAuthentications(AuthenticatorProtocolType.WebRequest));
                }
            }

            // Wire up request events

            // --- Call all authenticators in this one
            application.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
            // --- Associate identity with graywulf user
            application.PostAuthenticateRequest += new EventHandler(OnPostAuthenticateRequest);
            // --- Identify authenticated user
            application.PostAcquireRequestState += new EventHandler(OnPostAcquireRequestState);
            // --- Handle failed authentication
            application.EndRequest += new EventHandler(OnEndRequest);
        }

        #endregion
        #region Authentication life-cycle methods

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            // (1.) Called when a new page request is made and the built in
            // security module has established the identity of the user.

            var request = new AuthenticationRequest(HttpContext.Current);

            HttpContext.Current.Items[Constants.HttpContextAuthenticationRequest] = request;

            Authenticate(request);
        }

        protected override void OnAuthenticated(AuthenticationResponse response)
        {
            // (2.) Called after successfull authentication

            var context = HttpContext.Current;

            // Assign principal to both thread and HTTP contexts
            System.Threading.Thread.CurrentPrincipal = response.Principal;
            context.User = response.Principal;

            // Save authentication response for later
            // we cannot write headers here because this step is called for
            // WCF services but we want to write WCF headers from the
            // REST authentication module

            context.Items[Constants.HttpContextAuthenticationResponse] = response;
        }

        protected override void OnAuthenticationFailed(AuthenticationResponse response)
        {
            // (3.) Called after authentication was unsuccessful

            // This only means that the custom authenticators could not
            // identify the user, but it still might have been identified by
            // the web server (from Forms ticket, windows authentication, etc.)
            // In this case, the principal provided by the framework needs to
            // be converted to a graywulf principal

            var context = HttpContext.Current;
            context.Items[Constants.HttpContextAuthenticationResponse] = response;
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            // (4.) Called after the web request is authenticated

            SetAuthResponseHeaders();
        }

        private void OnPostAcquireRequestState(object sender, EventArgs e)
        {
            // (5.) This is where we get the session object the first time

            // To detect whether a user has left or a new user arrived we
            // store the principal in a session variable. Each request
            // looks into the variable and compares it with the principal
            // of the current request.
            // If the current request is authenticated by Graywulf, and this is the
            // first time we see the user, we need to load details from the registry

            var httpContext = HttpContext.Current;
            var httpApplication = (ApplicationBase)httpContext.ApplicationInstance;

            if (httpContext.Session != null)
            {
                // Get the saved principal from the session
                var sessionPrincipal = (GraywulfPrincipal)httpContext.Session[UI.Constants.SessionPrincipal];

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
                            httpApplication.OnUserLeft(sessionPrincipal);
                            httpContext.Session.Abandon();
                            sessionPrincipal = null;
                        }
                    }

                    if (sessionPrincipal == null)
                    {
                        // Save user into the session
                        httpContext.Session[UI.Constants.SessionPrincipal] = httpContext.User;

                        // Report the arrival of a new user
                        httpApplication.OnUserArrived((GraywulfPrincipal)httpContext.User);
                    }
                }
                else if (!httpContext.Request.IsAuthenticated && sessionPrincipal != null)
                {
                    // No authenticated uses but we see someone in the session. That means
                    // someone has just left (or the session has expired)
                    // Report the leaving of the user
                    httpApplication.OnUserLeft(sessionPrincipal);
                    httpContext.Session.Abandon();
                }
            }
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            // Here we need to handle the case of a failed authentication and redirect
            // the user a login page, if possible

            var application = (HttpApplication)sender;
            var context = application.Context;

            // If response is not access denied we don't have to do anything
            if (context.Response.StatusCode != 401)
            {
                return;
            }

            // Use the first authenticator in the list for redirect
            RedirectToLoginPage();
        }

        #endregion

        public void SetAuthResponseHeaders()
        {
            var context = HttpContext.Current;
            var response = (AuthenticationResponse)context.Items[Constants.HttpContextAuthenticationResponse];

            // Write out headers set by authenticators

            if (response != null)
            {
                response.SetResponseHeaders(context.Response);
            }
        }

        public void DeleteAuthResponseHeaders()
        {
            var context = HttpContext.Current;
            var response = (AuthenticationResponse)context.Items[Constants.HttpContextAuthenticationResponse];

            // Delete headers set by authenticators

            if (response != null)
            {
                response.DeleteResponseHeaders(context.Response);
            }
        }

        public string GetSignInUrl()
        {
            return BuildUrl(Configuration.SignInUri);
        }

        public string GetSignOutUrl()
        {
            return BuildUrl(Configuration.SignOutUri);
        }

        public string GetRegisternUrl()
        {
            return BuildUrl(Configuration.RegisterUri);
        }

        public string GetUserAccountUrl()
        {
            return BuildUrl(Configuration.AccountUri);
        }

        private string BuildUrl(Uri pageUri)
        {
            // Check if the redirect should be made to another host. If so, then the return
            // url should contain the host name, if not, it is enough to use path only

            string returnUrl;
            string url;

            if (Configuration.BaseUri.IsAbsoluteUri)
            {
                // Different host, use full URL with scheme and host name

                // TODO: figure out how it would work with reverse-proxy, without
                // URL rewrite...
                returnUrl = HttpContext.Current.Request.Url.ToString();
            }
            else
            {
                // Same host, use path and query only
                returnUrl = HttpContext.Current.Request.Url.PathAndQuery;
            }

            pageUri = Util.UriConverter.Combine(Configuration.BaseUri, pageUri);
            url = pageUri.ToString();

            returnUrl = HttpUtility.UrlEncode(returnUrl);

            if (url.Contains("[$ReturnUrl]"))
            {
                url = url.Replace("[$ReturnUrl]", returnUrl);
            }
            else
            {
                if (url.IndexOf('?') < 0)
                {
                    url += "?";
                }
                else
                {
                    url += "&";
                }

                url += "ReturnUrl=" + returnUrl;
            }

            return url;
        }

        private void RedirectToLoginPage()
        {
            HttpContext.Current.Response.Redirect(GetSignInUrl(), false);
        }
    }
}
