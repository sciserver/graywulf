using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class FederationApplicationBase : DomainApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            // Load settings from federation
            using (var context = CreateRegistryContext())
            {
                var federation = context.Federation;

                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle] = federation.ShortTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationLongTitle] = federation.LongTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationCopyright] = federation.Copyright;
            }
        }
    }
}
