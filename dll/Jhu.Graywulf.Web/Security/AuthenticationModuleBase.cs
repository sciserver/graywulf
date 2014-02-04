using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// Implement functions to authenticate web requests using a set
    /// of authenticators.
    /// </summary>
    public abstract class AuthenticationModuleBase
    {
        private IRequestAuthenticator[] requestAuthenticators;

        public void Init()
        {
            // Create authenticators
            // TODO: add factory type name here
            var af = AuthenticatorFactory.Create(null);
            this.requestAuthenticators = af.CreateRequestAuthenticators();
        }

        protected void CallRequestAuthenticators(HttpContext context)
        {
            // If user is not authenticated yet, try to authenticate them now using
            // various types of authenticators

            // Try each authentication protocol
            for (int i = 0; context.User == null && i < requestAuthenticators.Length; i++)
            {
                var user = requestAuthenticators[i].Authenticate();
                if (user != null)
                {
                    context.User = user;
                }
            }
        }

        protected void DispatchIdentityType(HttpContext context)
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
                    context.User = GraywulfPrincipal.Create((System.Web.Security.FormsIdentity)identity);
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
        }
    }
}
