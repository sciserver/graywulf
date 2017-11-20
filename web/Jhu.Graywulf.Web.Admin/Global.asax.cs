using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Admin
{
    public class Global : UIApplicationBase
    {
        protected override void OnApplicationStart()
        {
            base.OnApplicationStart();

            Application[Web.UI.Constants.ApplicationShortTitle] = "Graywulf admin";
            Application[Web.UI.Constants.ApplicationLongTitle] = "Graywulf admin interface";
            Application[Web.UI.Constants.ApplicationCopyright] = Util.AssemblyReflector.GetCopyright();
        }

        protected override void OnSessionStart()
        {
            base.OnSessionStart();

            var csb = new SqlConnectionStringBuilder(Registry.ContextManager.Configuration.ConnectionString);
            Session[Web.UI.Constants.SessionRegistryDatabase] = String.Format("{0}\\{1}", csb.DataSource, csb.InitialCatalog);
        }

        protected override void OnUserArrived(GraywulfPrincipal principal)
        {
            using (var context = CreateRegistryContext())
            {
                RegistryUser.RegistryContext = context;
                Session[Constants.SessionClusterGuid] = RegistryUser.Domain.Cluster.Guid;
                Session[Constants.SessionDomainGuid] = RegistryUser.Domain.Guid;
            }
        }

        protected override void OnUserLeft(GraywulfPrincipal principal)
        {   
        }
    }
}