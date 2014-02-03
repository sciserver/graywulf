using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Jhu.Graywulf.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Api;
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

            // Add service routes here
            JobService.RegisterRoute();
        }

        protected override void OnUserSignedIn(GraywulfIdentity identity)
        {
            // Check if user's myDB exists, if not, create
            using (var context = CreateRegistryContext())
            {
                var mydb = context.Federation.MyDBDatabaseVersion.GetUserDatabaseInstance(identity.User);

                if (mydb == null)
                {
                    var udii = new UserDatabaseInstanceInstaller(identity.User);
                    var udi = udii.GenerateUserDatabaseInstance(context.Federation.MyDBDatabaseVersion);

                    mydb = udi.DatabaseInstance;
                    mydb.Deploy();
                }
            }
        }

        protected override void OnUserSignedOut()
        {
            
        }
    }
}