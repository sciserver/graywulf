using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry.Check;
using Jhu.Graywulf.Web.Check;


namespace Jhu.Graywulf.Web.Auth
{
    public partial class Check : CheckPageBase
    {
        protected override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new IisCheck());
            checks.Add(new IdentityCheck());

            checks.Add(new DatabaseCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString));

            // TODO: add log writer tests
            //checks.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            checks.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName));
            checks.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.DomainName));

            checks.Add(new EmailCheck(RegistryContext.Domain.ShortTitle, RegistryContext.Domain.Email, RegistryContext.Domain.Email));

            // TODO: Checks.Routines.Add(new IdentityProviderCheck());
        }
    }
}