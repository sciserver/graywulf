using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.AccessControl;
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

        public override void OnUserArrived(UIApplicationBase application, GraywulfPrincipal principal)
        {
            base.OnUserArrived(application, principal);

            using (var context = application.CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                var uf = UserDatabaseFactory.Create(context.Federation);
                uf.EnsureUserDatabaseExists(principal.Identity.User);

                context.CommitTransaction();
            }
        }
    }
}