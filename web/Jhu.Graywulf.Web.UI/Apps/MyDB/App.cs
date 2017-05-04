using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public class App : AppBase
    {
        public override void RegisterVirtualPaths(UIApplicationBase application, EmbeddedVirtualPathProvider vpp)
        {
            base.RegisterVirtualPaths(application, vpp);

            using (var context = application.CreateRegistryContext())
            {
                var itf = Jhu.Graywulf.Jobs.ImportTables.ImportTablesJobFactory.Create(context.Federation);

                foreach (var m in itf.EnumerateMethods())
                {
                    m.RegisterVirtualPaths(vpp);
                }

                var etf = Jhu.Graywulf.Jobs.ExportTables.ExportTablesJobFactory.Create(context.Federation);

                foreach (var m in etf.EnumerateMethods())
                {
                    m.RegisterVirtualPaths(vpp);
                }
            }
        }

        public override void RegisterButtons(UIApplicationBase application)
        {
            var button = new Controls.MenuButton()
            {
                Key = Name,
                Text = "my data",
                NavigateUrl = "~/Apps/" + Name + "/Default.aspx"
            };

            application.RegisterMenuButton(button);
        }

        public override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new UserDatabaseCheck(FederationContext));
            checks.Add(new TypeCheck(FederationContext.Federation.FileFormatFactory));
            checks.Add(new TypeCheck(FederationContext.Federation.StreamFactory));
        }

        public override void OnUserArrived(UIApplicationBase application, GraywulfPrincipal principal)
        {
            base.OnUserArrived(application, principal);

            // Check if user database (MYDB) exists, and create it if necessary
            var user = principal.Identity.User;
            var uf = UserDatabaseFactory.Create(FederationContext);

            // This call will make sure all user databases are in place
            uf.GetUserDatabases(user);
        }
    }
}