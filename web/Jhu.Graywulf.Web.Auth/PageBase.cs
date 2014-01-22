using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Security;

namespace Jhu.Graywulf.Web.Auth
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        protected GraywulfPrincipal TemporaryPrincipal
        {
            get { return (GraywulfPrincipal)Session[Constants.SessionTempPrincipal]; }
            set { Session[Constants.SessionTempPrincipal] = value; }
        }
    }
}