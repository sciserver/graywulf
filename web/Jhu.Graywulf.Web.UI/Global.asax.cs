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
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Web.UI
{
    public class Global : FederationApplicationBase
    {
        protected override void OnUserArrived(GraywulfPrincipal principal)
        {
            using (var context = CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                var uf = UserDatabaseFactory.Create(context.Federation);
                uf.EnsureUserDatabaseExists(principal.Identity.User);

                // Load all datasets at start to be able to display from schema browser
                // Datasets will be cached internally
                var schemaManager = Jhu.Graywulf.Schema.GraywulfSchemaManager.Create(context.Federation);
                schemaManager.Datasets.LoadAll();

                context.CommitTransaction();
            }
        }

        protected override void OnUserLeft(GraywulfPrincipal principal)
        {

        }
    }
}