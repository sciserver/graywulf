using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Web.Check;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class Check : CheckPageBase
    {
        protected void Page_Load()
        {
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Registry.AppSettings.ConnectionString));
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.AppSettings.ClusterName));
            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.AppSettings.DomainName));

            Checks.Routines.Add(new EmailCheck(RegistryContext.Domain.Email));
            Checks.Routines.Add(new IdentityProviderCheck());
        }
    }
}