using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// When overriden in derived classes, performs per request
    /// authentication based on form data, HTTP headers or cookies.
    /// </summary>
    public abstract class Authenticator
    {
        #region Private member variables

        private string authorityName;
        private string authorityUrl;
        private bool isMasterAuthority;
        private string displayName;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the name of the authentication protocol.
        /// </summary>
        public abstract string ProtocolName { get; }

        /// <summary>
        /// Gets the value indicating whether the authenticator support
        /// interactive authentication for web pages.
        /// </summary>
        public abstract bool IsWebInteractive { get; }

        /// <summary>
        /// Gets the value indicating whether
        /// </summary>
        public abstract bool IsWebRequest { get; }
        public abstract bool IsRestRequest { get; }

        /// <summary>
        /// Gets the name of the authentication authority.
        /// </summary>
        [XmlElement]
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the URL of the authority.
        /// </summary>
        [XmlElement]
        public string AuthorityUrl
        {
            get { return authorityUrl; }
            set { authorityUrl = value; }
        }

        /// <summary>
        /// Gets or sets if the authenticator is accepter as a master authority
        /// </summary>
        [XmlElement]
        public bool IsMasterAuthority
        {
            get { return isMasterAuthority; }
            set { isMasterAuthority = value; }
        }

        /// <summary>
        /// Gets or sets the display name of the authority.
        /// </summary>
        [XmlElement]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        #endregion
        #region Constructors and initializers

        protected Authenticator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityName = null;
            this.authorityUrl = null;
            this.isMasterAuthority = false;
            this.displayName = null;
        }

        #endregion

        public virtual void Initialize(Domain domain)
        {
        }

        /// <summary>
        /// Authenticates the user based on data in the HTTP context
        /// and if succeeds, returns a principal identifying the user.
        /// </summary>
        /// <returns></returns>
        public abstract GraywulfPrincipal Authenticate(HttpContext httpContext);

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
                AuthorityUri = authorityUrl,
                IsMasterAuthority = isMasterAuthority,
                IsAuthenticated = false,
            };

            return new GraywulfPrincipal(identity);
        }

        public virtual void RedirectToLoginPage()
        {
            if (!IsWebInteractive)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
