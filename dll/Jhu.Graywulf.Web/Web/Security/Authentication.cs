using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// When overriden in derived classes, performs per request
    /// authentication based on form data, query parameters, HTTP headers or cookies.
    /// </summary>
    public abstract class Authentication : ICheckable
    {
        #region Private member variables

        private string authorityName;
        private Uri authorityUri;
        private bool isMasterAuthority;
        private string displayName;
        private bool isEnabled;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the name of the authentication protocol.
        /// </summary>
        public abstract string ProtocolName { get; }

        public abstract AuthenticatorProtocolType ProtocolType { get; }

        /// <summary>
        /// Gets the name of the authentication authority.
        /// </summary>
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the URL of the authority.
        /// </summary>
        public Uri AuthorityUri
        {
            get { return authorityUri; }
            set { authorityUri = value; }
        }

        /// <summary>
        /// Gets or sets if the authenticator is accepted as a master authority
        /// </summary>
        public bool IsMasterAuthority
        {
            get { return isMasterAuthority; }
            set { isMasterAuthority = value; }
        }

        /// <summary>
        /// Gets or sets the display name of the authority.
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        #endregion
        #region Constructors and initializers

        protected Authentication()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityName = null;
            this.authorityUri = null;
            this.isMasterAuthority = false;
            this.displayName = null;
            this.isEnabled = true;
        }

        #endregion

        public virtual void Initialize(Domain domain)
        {
        }

        /// <summary>
        /// Authenticates the user based on data in the HTTP context
        /// and if succeeds, returns a principal identifying the user.
        /// </summary>
        /// <remarks>
        /// When this function is implemented by an authenticator, it
        /// should verify if the identity of the user has been established already
        /// by testing response.Principal not being null and being marked as
        /// authenticated, as indicated by response.Principal.Identity.IsAuthenticated.
        /// Once the principal is valid, is should not be changed. An authenticator
        /// can use this call to refresh its tickets and tokens.
        /// </remarks>
        public abstract void Authenticate(AuthenticationRequest request, AuthenticationResponse response);

        public abstract void Deauthenticate(AuthenticationRequest request, AuthenticationResponse response);

        public abstract void Reset(AuthenticationRequest request, AuthenticationResponse response);

        /// <summary>
        /// Create a Graywulf principal with a pre-initialized identity
        /// </summary>
        /// <returns></returns>
        protected virtual GraywulfPrincipal CreatePrincipal()
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = ProtocolName,
                AuthorityName = authorityName,
                AuthorityUri = authorityUri == null ? null : authorityUri.ToString(),
                IsMasterAuthority = isMasterAuthority,
                IsAuthenticated = true,
            };

            return new GraywulfPrincipal(identity);
        }

        /// <summary>
        /// When implemented in derived classes, this function removes any tokens from a
        /// URI that corresponds to the authentication method. Override to remove any
        /// authentication tokens conveyed in urls.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public virtual string RemoveUrlTokens(string url)
        {
            return url;
        }

        /// <summary>
        /// When implemented in derived classes, this function redirects the web browser
        /// to the authentication page associated with the authenticator. This is typically
        /// an OpenID provider.
        /// </summary>
        public virtual void RedirectToLoginPage()
        {
            if ((ProtocolType & AuthenticatorProtocolType.WebInteractive) == 0)
            {
                throw new InvalidOperationException();
            }
        }

        #region Checkable routines

        public abstract IEnumerable<CheckRoutineBase> GetCheckRoutines();

        #endregion
    }
}
