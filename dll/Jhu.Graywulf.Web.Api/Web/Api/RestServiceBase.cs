using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api
{
    public abstract class RestServiceBase : ServiceBase, IDisposable
    {
        protected RestServiceBase()
        {
        }

        #region User managemenet functions

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        internal protected override void OnUserArrived(GraywulfPrincipal principal)
        {
            using (var context = CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                // TODO: add this back after factoring API out from web project
                //var udii = new UserDatabaseInstanceInstaller(context);
                //udii.EnsureUserDatabaseInstanceExists(principal.Identity.User, context.Federation.MyDBDatabaseVersion);
            }
        }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        internal protected override void OnUserSignedOut(GraywulfPrincipal principaly)
        {
        }

        #endregion
    }
}