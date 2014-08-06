using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Api.V1;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Web.UI
{
    public class Global : ApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            using (var context = CreateRegistryContext())
            {
                var federation = context.Federation;

                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle] = federation.ShortTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationLongTitle] = federation.LongTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationCopyright] = federation.Copyright;
            }
        }


        protected override void OnUserArrived(GraywulfPrincipal principal)
        {
            using (var context = CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                var udii = new UserDatabaseInstanceInstaller(context);
                udii.EnsureUserDatabaseInstanceExists(principal.Identity.User, context.Federation.MyDBDatabaseVersion);

                // Load all datasets at start to be able to display from schema browser
                // Datasets will be cached internally
                var schemaManager = new Jhu.Graywulf.Schema.GraywulfSchemaManager(context, Jhu.Graywulf.Registry.AppSettings.FederationName);
                schemaManager.Datasets.LoadAll();

                context.CommitTransaction();
            }
        }

        protected override void OnUserLeft(GraywulfPrincipal principal)
        {

        }
    }
}