using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Jhu.Graywulf.Security
{
    public class GraywulfPrincipal : IPrincipal
    {
        private GraywulfIdentity identity;

        internal GraywulfPrincipal(GraywulfIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}
