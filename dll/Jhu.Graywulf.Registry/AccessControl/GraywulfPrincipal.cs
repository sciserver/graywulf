using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;

namespace Jhu.Graywulf.AccessControl
{
    /// <summary>
    /// Implements a security principal for the Graywulf authentication framework.
    /// </summary>
    public class GraywulfPrincipal : Graywulf.AccessControl.Principal, IPrincipal
    {       
        public new GraywulfIdentity Identity
        {
            get { return (GraywulfIdentity)base.Identity; }
        }

        public GraywulfPrincipal(GraywulfIdentity identity)
            :base(identity)
        {
        }

        public string UniqueID
        {
            get
            {
                return String.Format("{0}|{1}", Identity.AuthorityUri, Identity.Identifier);
            }
        }
    }
}
