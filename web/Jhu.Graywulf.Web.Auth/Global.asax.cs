using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Jhu.Graywulf.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;

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

                Application[Constants.ApplicationShortTitle] = domain.ShortTitle;
                Application[Constants.ApplicationLongTitle] = domain.LongTitle;
            }
        }

        protected override void OnUserSignedIn(GraywulfIdentity identity)
        {
            
        }

        protected override void OnUserSignedOut()
        {
            
        }
    }
}