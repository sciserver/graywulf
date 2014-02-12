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
        /// <summary>
        /// Creates a graywulf principal based on the guid stored in the
        /// forms authentication token.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static GraywulfPrincipal Create(System.Web.Security.FormsIdentity formsIdentity)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameForms,
                Identifier = formsIdentity.Name,
                IsAuthenticated = true,
            };

            identity.UserReference.Name = formsIdentity.Name;
            return new GraywulfPrincipal(identity);
        }

        public static GraywulfPrincipal Create(System.Web.Security.FormsAuthenticationTicket ticket)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameForms,
                Identifier = ticket.Name,
                IsAuthenticated = true,
            };

            identity.UserReference.Name = ticket.Name;
            return new GraywulfPrincipal(identity);
        }

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
    }
}
