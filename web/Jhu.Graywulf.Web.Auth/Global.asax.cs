using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Auth
{
    public class Global : ApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            // Load domain settings
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var domain = ef.LoadEntity<Domain>(Domain.AppSettings.DomainName);

                Application[Constants.ApplicationShortTitle] = domain.ShortTitle;
                Application[Constants.ApplicatonLongTitle] = domain.LongTitle;
            }
        }
    }
}