using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public class App : AppBase
    {
        public override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new TypeCheck(FederationContext.Federation.SchemaManager));
        }
    }
}