using System;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI
{
    public class Global : FederationApplicationBase
    {
        protected override void RegisterApps()
        {
            base.RegisterApps();

            RegisterApp(typeof(Apps.Common.App));
            RegisterApp(typeof(Apps.Schema.App));
            RegisterApp(typeof(Apps.Query.App));
            RegisterApp(typeof(Apps.Jobs.App));
            RegisterApp(typeof(Apps.MyDB.App));
            RegisterApp(typeof(Apps.Api.App));
            RegisterApp(typeof(Apps.Docs.App));
        }

        protected override void RegisterServices()
        {
            base.RegisterServices();

            RegisterService(typeof(IAuthService));
            RegisterService(typeof(ISchemaService));
            RegisterService(typeof(IJobsService));
            RegisterService(typeof(IDataService));
            RegisterService(typeof(ITestService));
        }
    }
}