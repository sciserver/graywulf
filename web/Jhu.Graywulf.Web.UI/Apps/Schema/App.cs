using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public class App : AppBase
    {
        public override void OnUserArrived(UIApplicationBase application, GraywulfPrincipal principal)
        {
            base.OnUserArrived(application, principal);

            using (var context = application.CreateRegistryContext())
            {
                // Load all datasets at start to be able to display from schema browser
                // Datasets will be cached internally
                var schemaManager = GraywulfSchemaManager.Create(context.Federation);
                schemaManager.Datasets.LoadAll();

                context.CommitTransaction();
            }
        }
    }
}