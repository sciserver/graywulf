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

                Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle] = federation.ShortTitle;
                Application[Jhu.Graywulf.Web.Constants.ApplicationLongTitle] = federation.LongTitle;
                Application[Jhu.Graywulf.Web.Constants.ApplicationCopyright] = federation.Copyright;
            }
        }


        protected override void OnUserSignedIn(GraywulfIdentity identity)
        {
            // TODO: context cannot be created if no user is set
            using (var context = CreateRegistryContext())
            {
                // Check if user's myDB exists, if not, create
                var mydb = context.Federation.MyDBDatabaseVersion.GetUserDatabaseInstance(identity.User);

                if (mydb == null)
                {
                    identity.User.Context = context;
                    var udii = new UserDatabaseInstanceInstaller(identity.User);
                    var udi = udii.CreateUserDatabaseInstance(context.Federation.MyDBDatabaseVersion);
                }

                // Load all datasets at start to be able to display from schema browser
                // Datasets will be cached internally
                using (var registryContext = CreateRegistryContext())
                {
                    var schemaManager = new Jhu.Graywulf.Schema.GraywulfSchemaManager(registryContext, Jhu.Graywulf.Registry.AppSettings.FederationName);
                    schemaManager.Datasets.LoadAll();
                }

                context.CommitTransaction();
            }
        }

        protected override void OnUserSignedOut()
        {
            
        }
    }
}