using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public class App : AppBase
    {
        public override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new TypeCheck(RegistryContext.Federation.QueryFactory));
        }
    }
}