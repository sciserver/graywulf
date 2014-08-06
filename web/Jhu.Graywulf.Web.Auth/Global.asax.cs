using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Auth
{
    public class Global : ApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            // Load domain settings
            using (var context = CreateRegistryContext())
            {
                var domain = context.Domain;

                Application[Web.UI.Constants.ApplicationShortTitle] = domain.ShortTitle;
                Application[Web.UI.Constants.ApplicationLongTitle] = domain.LongTitle;
            }
        }

        protected override void OnUserArrived(GraywulfPrincipal principal)
        {
            
        }

        protected override void OnUserLeft(GraywulfPrincipal principal)
        {
            
        }
    }
}