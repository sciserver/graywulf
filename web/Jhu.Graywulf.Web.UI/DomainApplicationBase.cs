using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Hosting;
using System.IO;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class DomainApplicationBase : ApplicationBase
    {
        public DomainApplicationBase()
        {

            // These few lines below turn on web auto-configure that
            // replaces settings in web.config based on the contents of the
            // registry. Currently not used as needs more testing.
#if FALSE
            if (WebConfigHelper.Configure())
            {
                HttpRuntime.UnloadAppDomain();
            }
#endif

            //var m = (System.Web.Configuration.MachineKeySection)System.Configuration.ConfigurationManager.GetSection("system.web/machineKey");
            //throw new Exception(m.ValidationKey);
        }

        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            // Load domain settings
            using (var context = CreateRegistryContext())
            {
                var domain = context.Domain;

                Application[Web.UI.Constants.ApplicationDomainName] = domain.ShortTitle;
                Application[Web.UI.Constants.ApplicationShortTitle] = domain.ShortTitle;
                Application[Web.UI.Constants.ApplicationLongTitle] = domain.LongTitle;
                Application[Web.UI.Constants.ApplicationCopyright] = domain.Copyright;
            }
        }
    }
}
