using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Jhu.Graywulf.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace Jhu.Graywulf.Web.WS
{
    public class Global : ApplicationBase
    {
        protected override void OnUserSignedIn(Security.GraywulfIdentity identity)
        {
            
        }

        protected override void OnUserSignedOut()
        {

        }

        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            RouteTable.Routes.Add(new ServiceRoute("Jobs/", new WebServiceHostFactory(), typeof(QueryJobService)));
            RouteTable.Routes.Add(new ServiceRoute("Schema/", new WebServiceHostFactory(), typeof(SchemaService)));
        }
    }
}