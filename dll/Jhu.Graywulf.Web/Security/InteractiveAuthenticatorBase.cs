using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// When implemented in derived classes, perfoms authentication
    /// that requires user interaction
    /// </summary>
    public abstract class InteractiveAuthenticatorBase : IAuthenticator
    {
        private string authorityName;
        private string authorityUri;
        private string displayName;

        public abstract string Protocol { get; }

        /// <summary>
        /// Gets or sets the name of the authority
        /// </summary>
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the URI uniquely identifying the authority
        /// </summary>
        public string AuthorityUri
        {
            get { return authorityUri; }
            set { authorityUri = value; }
        }

        /// <summary>
        /// Gets or sets the display name of the authority
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        protected InteractiveAuthenticatorBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityUri = null;
            this.displayName = null;
        }

        /// <summary>
        /// When implemented in derived classes, authenticates the user.
        /// </summary>
        /// <returns></returns>
        public abstract GraywulfPrincipal Authenticate();

        /// <summary>
        /// When implemented in derived classes, redirects the browser
        /// to the sign in form of the authority.
        /// </summary>
        public abstract void RedirectToLoginPage();
    }
}
