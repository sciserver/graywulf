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
    public class WebAuthenticationModule : AuthenticationModuleBase, IHttpModule
    {
        public WebAuthenticationModule()
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
            base.Init();

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
            CallRequestAuthenticators(context);
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            DispatchIdentityType(context);
        }
    }
}
