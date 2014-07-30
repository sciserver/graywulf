using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
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
        private Authenticator[] authenticators;

        #region Constructors and initializers

        protected AuthenticationModuleBase()
        {
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
        /// Calls all registered request authenticators
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Authenticate(AuthenticationRequest request)
        {
            // If user is not authenticated yet, try to authenticate them now using
            // various types of authenticators

            if (authenticators != null)
            {
                GraywulfPrincipal principal = null;

                // Try each authentication protocol
                for (int i = 0; principal == null && i < authenticators.Length; i++)
                {
                    principal = authenticators[i].Authenticate(request);
                }

                if (principal != null)
                {
                    OnAuthenticated(principal);
                    return;
                }
            }

            OnAuthenticationFailed();
        }

        protected abstract void OnAuthenticated(GraywulfPrincipal principal);

        protected abstract void OnAuthenticationFailed();

        /// <summary>
        /// Converts indentities into Graywulf identity.
        /// </summary>
        /// <param name="context"></param>
        protected virtual GraywulfPrincipal DispatchIdentityType(IPrincipal principal)
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
    }
}
