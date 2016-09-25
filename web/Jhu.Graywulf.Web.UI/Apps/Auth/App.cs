using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.UI.Apps.Auth
{
    public class App : AppBase
    {
        public override void RegisterButtons(UIApplicationBase application)
        {
        }

        public override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new IdentityProviderCheck(RegistryContext));
        }
    }
}