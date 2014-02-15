using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;

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
    public abstract class AuthenticationModuleBase
    {
        private IRequestAuthenticator[] requestAuthenticators;

        /// <summary>
        /// Registeres request authenticators
        /// </summary>
        /// <param name="authenticators"></param>
        protected void RegisterRequestAuthenticators(IRequestAuthenticator[] authenticators)
        {
            requestAuthenticators = authenticators;
        }

        /// <summary>
        /// Calls all registered request authenticators
        /// </summary>
        /// <param name="context"></param>
        protected void CallRequestAuthenticators(HttpContext context)
        {
            // If user is not authenticated yet, try to authenticate them now using
            // various types of authenticators
            
            if (requestAuthenticators != null)
            {
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
        }

        /// <summary>
        /// Converts indentities into Graywulf identity.
        /// </summary>
        /// <param name="context"></param>
        protected GraywulfPrincipal DispatchIdentityType(IPrincipal principal)
        {
            // The request is processed now. If the user has been authenticated but
            // the principal is not a Graywulf principal, it has to be replaced now
            if (principal != null)
            {
                var identity = principal.Identity;

                if (identity is GraywulfIdentity)
                {
                    // Nothing to do here
                    return (GraywulfPrincipal)principal;
                }
                else if (identity is System.Security.Principal.GenericIdentity)
                {
                    // By default, identity is a generic identity with
                    // IsAuthorized = false, so let this be
                    return null;
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
                    return GraywulfPrincipal.Create((System.Web.Security.FormsIdentity)identity);
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
            else
            {
                return null;
            }
        }
    }
}
