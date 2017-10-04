using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public class App : AppBase
    {
        public override void RegisterButtons(UIApplicationBase application)
        {
            base.RegisterButtons(application);

            var button = new MenuButton()
            {
                Key = "examples",
                Text = "examples",
                NavigateUrl = "~/Assets/Query/Examples/00_index.aspx"
            };
            application.RegisterMenuButton(button);
        }

        public override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new TypeCheck(FederationContext.Federation.QueryFactory));
        }
    }
}