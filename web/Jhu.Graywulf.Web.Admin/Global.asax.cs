using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using Jhu.Graywulf.Security;

namespace Jhu.Graywulf.Web.Admin
{
    public class Global : ApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            Application[Web.Constants.ApplicationShortTitle] = "Graywulf admin";
            Application[Web.Constants.ApplicationLongTitle] = "Graywulf admin interface";
        }

        protected override void Session_Start(object sender, EventArgs e)
        {
            base.Session_Start(sender, e);

            var csb = new SqlConnectionStringBuilder(Registry.AppSettings.ConnectionString);
            Session[Web.Constants.SessionRegistryDatabase] = String.Format("{0}\\{1}", csb.DataSource, csb.InitialCatalog);
        }

        protected override void OnUserSignedIn(GraywulfIdentity identity)
        {
            Session[Constants.SessionClusterGuid] = RegistryUser.Domain.Cluster.Guid;
            Session[Constants.SessionDomainGuid] = RegistryUser.Domain.Guid;
        }

        protected override void OnUserSignedOut()
        {
            
        }
    }
}