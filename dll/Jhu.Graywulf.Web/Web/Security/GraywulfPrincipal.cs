using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements a security principal for the Graywulf authentication framework.
    /// </summary>
    public class GraywulfPrincipal : IPrincipal
    {
        private HashSet<string> roles;
        private GraywulfIdentity identity;

        public HashSet<string> Roles
        {
            get { return roles; }
        }

        internal GraywulfPrincipal(GraywulfIdentity identity)
        {
            this.roles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            this.identity = identity;
        }

        /// <summary>
        /// Gets the identity associated with the principal.
        /// </summary>
        IIdentity IPrincipal.Identity
        {
            get { return identity; }
        }

        public GraywulfIdentity Identity
        {
            get { return identity; }
        }

        /// <summary>
        /// Returns truw if the identity is in the specified role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return roles.Contains(role);
        }

        public string UniqueID
        {
            get
            {
                return String.Format("{0}|{1}", identity.AuthorityUri, identity.Identifier);
            }
        }
    }
}
