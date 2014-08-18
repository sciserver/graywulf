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
    public class Global : DomainApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            var xml = System.IO.File.ReadAllText(@"C:\Users\dobos\project\skyquery-all\graywulf\web\Jhu.Graywulf.Web.Auth\web.config");

            //var m = (System.Web.Configuration.MachineKeySection)System.Configuration.ConfigurationManager.GetSection("system.web/machineKey");
            //var m = (System.Web.Configuration.MachineKeySection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/machineKey");
            //throw new Exception(m.ValidationKey);

            var m = (System.Web.Configuration.MachineKeySection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/machineKey");
            throw new Exception(m.ValidationKey);
        }

    }
}