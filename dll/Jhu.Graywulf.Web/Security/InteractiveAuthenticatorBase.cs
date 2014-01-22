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
        private string authority;
        private string displayName;

        public abstract string Protocol { get; }

        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

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
            this.authority = null;
            this.displayName = null;
        }

        public abstract GraywulfPrincipal Authenticate();

        public abstract void RedirectToLoginPage();
    }
}
