using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Security;

namespace Jhu.Graywulf.Web.Auth
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        /// <summary>
        /// Gets or sets the temporary principal that is used when
        /// an authority authorized a user unknown to us.
        /// </summary>
        protected GraywulfPrincipal TemporaryPrincipal
        {
            get { return (GraywulfPrincipal)Session[Constants.SessionTempPrincipal]; }
            set { Session[Constants.SessionTempPrincipal] = value; }
        }
    }
}